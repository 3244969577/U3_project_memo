using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家控制器类 - 处理玩家所有核心功能
/// 功能概述：
/// 1. 单例模式管理 - 确保游戏中只有一个玩家实例
/// 2. 背包系统管理 - 初始化和管理玩家背包
/// 3. 输入处理 - 处理键盘输入和鼠标位置
/// 4. 移动控制 - 基于输入控制玩家移动
/// 5. 视角旋转 - 根据鼠标位置控制玩家朝向
/// 6. 武器系统 - 装备、切换和管理武器
/// 7. 物品收集 - 将收集品添加到背包
/// 8. 死亡处理 - 触发死亡动画和场景重试
/// 9. UI交互 - 控制背包UI的显示/隐藏
/// </summary>
public class Player : Character
{

	const string TAG = "Player";      // 玩家标签
	const string DEATH_ANIM = "onDeath";      // 死亡动画名称

	private Vector2 movement;      // 移动方向
	private Weapon weapon;      // 当前装备的武器

	public UI_Inventory uiInventory;      // 背包UI引用

	public static Inventory inventory;      // 玩家背包
	public static Player instance;      // 玩家单例实例

	/// <summary>
	/// 初始化玩家单例实例和背包系统
	/// - 确保游戏中只有一个玩家实例（单例模式）
	/// - 创建新的背包实例
	/// - 设置背包UI并初始隐藏
	/// </summary>
	private void Awake()
	{
		if (Player.instance != null)
		{
			Destroy(gameObject);
		}
		instance = this;

		inventory = new Inventory();
		uiInventory.SetInventory(inventory);
		uiInventory.Hidden();
	}


	/// <summary>
	/// 初始化玩家组件
	/// - 调用基类Start方法初始化基础组件
	/// - 获取子对象中的武器组件
	/// </summary>
	public override void Start()
	{
		base.Start();
		this.weapon = GetComponentInChildren<Weapon>();
	}

	// Update is called once per frame
	public override void Update()
	{
		base.Update();
		this.movement.x = Input.GetAxisRaw("Horizontal");
		this.movement.y = Input.GetAxisRaw("Vertical");
		if (this.moveAble)
		{
			this.Move();
			this.SwitchWeapon();
		}
	}

	/// <summary>
	/// 更新玩家朝向
	/// - 根据鼠标位置计算玩家应该面对的方向
	/// - 当鼠标在玩家右侧时，玩家朝向右侧
	/// - 当鼠标在玩家左侧时，玩家朝向左侧
	/// </summary>
	/// <param name="mousePosition">鼠标在世界空间中的位置</param>
	private void UpdatePlayerRotation(Vector2 mousePosition)
	{
		Vector2 playerPos = rb.transform.position;
		Vector2 lookDirection = mousePosition - playerPos;

		if (lookDirection.x > 0)
		{
			this.transform.localEulerAngles = new Vector3(0, 0, 0);
		}
		else
		{
			this.transform.localEulerAngles = new Vector3(0, 180, 0);
		}
	}

	/// <summary>
	/// 装备武器
	/// - 如果当前已装备武器且不是要装备的武器，则隐藏当前武器
	/// - 装备新武器并设置为激活状态
	/// - 将武器的父对象设置为玩家对象
	/// </summary>
	/// <param name="_weapon">要装备的武器对象</param>
	public void EquipWeapon(Weapon _weapon)
	{
		if (this.weapon != null && !object.ReferenceEquals(this.weapon, _weapon))
		{
			this.weapon.gameObject.SetActive(false);
		}

		this.weapon = _weapon;
		this.weapon.gameObject.SetActive(true);
		_weapon.transform.parent = this.transform;
	}

	/// <summary>
	/// 向背包添加物品
	/// - 将收集到的物品添加到玩家背包
	/// - 更新背包UI以显示新物品
	/// </summary>
	/// <param name="item">要添加到背包的物品</param>
	public void AddItemToInventory(Collectible item)
	{
		Player.inventory.AddItem(item);
		this.uiInventory.SetInventory(Player.inventory);
	}

	/// <summary>
	/// 移动玩家
	/// - 根据输入的移动方向和玩家速度更新玩家位置
	/// - 使用物理刚体的MovePosition方法确保平滑移动
	/// - 更新玩家朝向
	/// </summary>
	public override void Move()
	{
		// 移动玩家
		this.rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
		
		// 更新玩家朝向
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		this.UpdatePlayerRotation(mousePosition);
	}

	/// <summary>
	/// 处理玩家死亡
	/// - 触发死亡动画
	/// - 如果玩家装备了武器，则销毁武器对象
	/// - 调用GameManager的RetryScene方法重新加载当前场景
	/// </summary>
	public override void Die()
	{
		this.animator.SetTrigger(DEATH_ANIM);
		if (this.weapon != null)
		{
			Destroy(this.weapon.gameObject);
		}
		GameManager.instance.RetryScene();
	}

    /// <summary>
	/// 切换武器
	/// - 监听数字键1-4的按下事件
	/// - 根据按下的数字键从背包中获取对应位置的物品
	/// - 显示背包UI
	/// - 如果获取到物品且物品是武器，则装备该武器
	/// </summary>
	protected void SwitchWeapon()
	{
		Collectible item = null;
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			item = Player.inventory.GetItem(0);
			this.uiInventory.Show();
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			item = Player.inventory.GetItem(1);
			this.uiInventory.Show();
		}

		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			item = Player.inventory.GetItem(2);
			this.uiInventory.Show();
		}

		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			item = Player.inventory.GetItem(3);
			this.uiInventory.Show();
		}

		if (item != null)
			this.EquipWeapon((Weapon)item);
	}

	/// <summary>
	/// 设置玩家是否可移动
	/// </summary>
	/// <param name="moveable">是否可移动</param>
	public void SetMoveable(bool moveable)
	{
		this.moveAble = moveable;
	}
}
