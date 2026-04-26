using UnityEngine;
using System.Collections;
using GlobalEvents;


public class Player : Character
{
	const string TAG = "Player";      // 玩家标签
	private Vector2 movement;      // 移动方向
	private Weapon weapon;      // 当前装备的武器

	public UI_Inventory uiInventory;      // 背包UI引用
	public static Inventory inventory;      // 玩家背包
	
	public static Player instance;      // 玩家单例实例

	private EventBinding<ObtainEquipmentEvent> obtainEquipmentEventBinding;
	private EventBinding<BulletHitEvent> bulletHitEventBinding;


#region hook
	protected override void Awake()
	{
		base.Awake();
		if (Player.instance != null)
		{
			Destroy(gameObject);
		}
		instance = this;

		inventory = new Inventory();
		uiInventory.SetInventory(inventory);
		uiInventory.Hidden();

		obtainEquipmentEventBinding = new EventBinding<ObtainEquipmentEvent>(OnObtainEquipment);
		bulletHitEventBinding = new EventBinding<BulletHitEvent>(OnBulletHit);
	}
	
	public override void Start()
	{
		base.Start();
		this.weapon = GetComponentInChildren<Weapon>();

		// 初始化玩家生命值事件
		EventBus<PlayerHPChangeEvent>.Raise( new PlayerHPChangeEvent {
			currentHealth = this.healthBar.GetHealth(),
			maxHealth = this.maxHealth
		});
	}

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
#endregion


#region EventBindings
	private void OnEnable()
	{
		EventBus<ObtainEquipmentEvent>.Register(obtainEquipmentEventBinding);
		EventBus<BulletHitEvent>.Register(bulletHitEventBinding);
	}

	private void OnDisable()
	{
		EventBus<ObtainEquipmentEvent>.Deregister(obtainEquipmentEventBinding);
		EventBus<BulletHitEvent>.Deregister(bulletHitEventBinding);
	}

	private void OnObtainEquipment(ObtainEquipmentEvent e)
	{
		Debug.Log("获得装备");
		Weapon weapon = e.equipment.GetComponent<Weapon>();
		EquipWeapon(weapon);
		AddItemToInventory(weapon);
	}

	private void OnBulletHit(BulletHitEvent e)
	{
		if (e.target == this.gameObject)
		{
			Debug.Log($"子弹命中目标: {e.target.name}, 伤害: {e.damage}");
			GetDamaged(e.damage, e.attacker);
		}
	}
#endregion


#region override methods
	public override void GetDamaged(float value, GameObject attacker)
	{
		base.GetDamaged(value, attacker);

		EventBus<PlayerHitEvent>.Raise( new PlayerHitEvent {
			attacker = attacker,
			damage = value
		});
		EventBus<PlayerHPChangeEvent>.Raise( new PlayerHPChangeEvent {
			currentHealth = this.healthBar.GetHealth(),
			maxHealth = this.maxHealth
		});
	}

	public override void RestoreHealth(float value)
    {
        base.RestoreHealth(value);
		
        EventBus<PlayerHPChangeEvent>.Raise( new PlayerHPChangeEvent {
			currentHealth = this.healthBar.GetHealth(),
			maxHealth = this.maxHealth
		});
    }
	
	public override void Move()
	{
		// 移动玩家
		this.rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
		
		// 更新玩家朝向
		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		this.UpdatePlayerRotation(mousePosition);
	}

	public override void Die()
	{
		// this.animator.SetTrigger(DEATH_ANIM);
		if (this.weapon != null)
		{
			Destroy(this.weapon.gameObject);
		}
		SetMoveable(false);
		Debug.Log($"Player {this.name} died!");
		EventBus<PlayerDieEvent>.Raise(new PlayerDieEvent());
	}
#endregion


#region methods
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

	public void AddItemToInventory(Collectible item)
	{
		Player.inventory.AddItem(item);
		this.uiInventory.SetInventory(Player.inventory);
	}

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

	public void SetMoveable(bool moveable)
	{
		this.moveAble = moveable;
	}
#endregion


}
