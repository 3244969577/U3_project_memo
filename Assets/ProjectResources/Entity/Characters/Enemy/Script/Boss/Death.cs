using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEvents;

/// <summary>
/// Death Boss类 - 游戏中的死亡Boss
/// - 继承自Boss类，包含Boss的基本属性和行为
/// - 实现多阶段攻击模式，随着生命值降低切换不同攻击阶段
/// - 具有 teleport（传送）能力
/// - 不同阶段使用不同的子弹和射击力量
/// </summary>
public class Death : Boss
{
    /// <summary>
    /// 不同阶段的射击力量
    /// - 索引0: 第一阶段
    /// - 索引1: 第二阶段
    /// - 索引2: 第三阶段
    /// </summary>
    public float[] shootingForces;

    /// <summary>
    /// 不同阶段的子弹预制体
    /// - 索引0: 第一阶段
    /// - 索引1: 第二阶段
    /// - 索引2: 第三阶段
    /// </summary>
    public GameObject[] bulletPrefabs;

    /// <summary>
    /// 当前Boss的攻击阶段
    /// </summary>
	PHRASE phrase;

    /// <summary>
    /// 上一次的攻击阶段
    /// - 用于检测阶段切换
    /// </summary>
	PHRASE lastPhrase;

    /// <summary>
    /// Boss是否能够攻击
    /// - true: 可以攻击
    /// - false: 不能攻击
    /// </summary>
	bool ableToAttack=true;

    /// <summary>
    /// 死亡动画触发名称
    /// </summary>
	const string DEATH_ANIM = "onDeath";

    /// <summary>
    /// 阶段切换动画触发名称
    /// </summary>
	const string PHRASE_ANIM = "onPhrase";

    /// <summary>
    /// 传送动画触发名称
    /// </summary>
	const string TELEPORT_ANIM = "onTeleport";

    /// <summary>
    /// 攻击动画触发名称
    /// </summary>
	const string ATTACK_ANIM = "onAttack";

    /// <summary>
    /// Boss的攻击阶段枚举
    /// - ONE: 第一阶段（高生命值）
    /// - TWO: 第二阶段（中等生命值）
    /// - THREE: 第三阶段（低生命值）
    /// - DEATH: 死亡阶段
    /// </summary>
	public enum PHRASE
    {
		ONE,
		TWO,
		THREE,
		DEATH
	}

    /// <summary>
    /// 不同阶段的生命值阈值
    /// - 索引0: 第一阶段到第二阶段的阈值（60%生命值）
    /// - 索引1: 第二阶段到第三阶段的阈值（15%生命值）
    /// </summary>
	public float[] HEALTH_PHRASE = { 0.6f, 0.15f };



    /// <summary>
    /// 初始化Boss状态
    /// - 设置初始攻击阶段为第一阶段
    /// - 初始化上一次攻击阶段
    /// </summary>
	protected override void Awake()
    {
		base.Awake();
		
		phrase = PHRASE.ONE;
		lastPhrase = PHRASE.ONE;
		killScore = 1000;
	}

    /// <summary>
    /// 启动Boss
    /// - 调用父类Start方法
    /// </summary>
	public override void Start()
	{
		base.Start();
	}

    /// <summary>
    /// 阶段攻击方法
    /// - 根据当前阶段的子弹和射击力量进行攻击
    /// - 随机偏移攻击方向，增加攻击难度
    /// - 计算子弹角度并设置
    /// - 发射子弹并设置生命周期
    /// </summary>
    /// <param name="target">攻击目标</param>
	protected void PhraseAttack(Character target)
	{
		if (Time.time > this.attackRate + this.lastAttack && ableToAttack)
		{
			this.animator.SetTrigger(ATTACK_ANIM);

			Vector2 direction = (target.transform.position - transform.position);
			direction += new Vector2(Random.Range(-4.0f, 4.0f), Random.Range(-4.0f, 4.0f));

			direction = direction.normalized;
			// GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
			GameObject newBullet = GlobalObjectPool.Instance.Spawn(bulletPrefab, transform.position, transform.rotation);

			float n = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			if (n < 0) n += 360;

			newBullet.transform.eulerAngles = new Vector3(0, 0, n);
			newBullet.GetComponent<Rigidbody2D>().AddForce(shootingForce * direction, ForceMode2D.Impulse);
			Destroy(newBullet, 2f);
			lastAttack = Time.time;
		}
	}

    /// <summary>
    /// 攻击方法
    /// - 检查当前攻击阶段
    /// - 处理阶段切换
    /// - 根据不同阶段执行不同的攻击行为
    ///   - 第一阶段：使用第一阶段子弹和射击力量
    ///   - 第二阶段：使用第二阶段子弹和射击力量，并可能传送
    ///   - 第三阶段：使用第三阶段子弹和射击力量，并可能传送
    ///   - 死亡阶段：不执行攻击
    /// </summary>
    /// <param name="target">攻击目标</param>
	public override void Attack(Character target)
	{

		phrase = CheckPhrase();
		CheckPhraseTransition();
		
		switch (phrase)
        {
			case PHRASE.ONE:
				this.bulletPrefab = this.bulletPrefabs[0];
				this.shootingForce = this.shootingForces[0];
				PhraseAttack(target);
				break;

			case PHRASE.TWO:
				this.bulletPrefab = this.bulletPrefabs[1];
				this.shootingForce = this.shootingForces[1];
				PhraseAttack(target);
				Teleport();
				break;

			case PHRASE.THREE:
				this.bulletPrefab = this.bulletPrefabs[2];
				this.shootingForce = this.shootingForces[2];
				PhraseAttack(target);
				Teleport();
				break;
			case PHRASE.DEATH:
				break;
		}
	}

    /// <summary>
    /// 传送方法
    /// - 根据当前阶段决定传送概率
    /// - 第二阶段：20%概率
    /// - 第三阶段：35%概率
    /// - 如果随机数小于阈值，则触发传送动画
    /// </summary>
    private void Teleport()
    {
		float rand = Random.Range(0, 1f);
		float thresh = -1f;
		if (phrase == PHRASE.TWO)
			thresh = 0.2f;
		else if (phrase == PHRASE.THREE)
			thresh = 0.35f;
			
		if (rand < thresh)
        {
			this.animator.SetTrigger(TELEPORT_ANIM);
		}
    }

    /// <summary>
    /// 随机传送方法
    /// - 生成随机方向
    /// - 向随机方向添加力，实现传送效果
    /// </summary>
	public void RandomTeleport()
	{
		// Random direction
		Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

		// Add force
		this.rb.AddForce(direction * 1000f);
	}

    /// <summary>
    /// 检查阶段切换
    /// - 如果当前阶段与上一次阶段不同，则触发阶段切换动画
    /// - 更新上一次阶段为当前阶段
    /// </summary>
    private void CheckPhraseTransition()
    {
		if (lastPhrase != phrase)
		{
			this.animator.SetTrigger(PHRASE_ANIM);
			lastPhrase = phrase;
		}

	}

    /// <summary>
    /// 检查当前攻击阶段
    /// - 根据当前生命值百分比确定攻击阶段
    /// - 生命值低于15%: 第三阶段
    /// - 生命值低于60%: 第二阶段
    /// - 生命值高于60%: 第一阶段
    /// </summary>
    /// <returns>当前攻击阶段</returns>
    protected PHRASE CheckPhrase()
    {
		float currentHPPercent = this.GetHealth() / this.GetMaxHealth();

		if (currentHPPercent < HEALTH_PHRASE[1])
			return PHRASE.THREE;

		if (currentHPPercent < HEALTH_PHRASE[0])
			return PHRASE.TWO;

		return PHRASE.ONE;
	}

    /// <summary>
    /// 移动方法
    /// - 根据目标位置计算移动方向
    /// - 根据移动方向调整精灵朝向
    /// - 应用线性速度实现移动
    /// </summary>
    /// <param name="position">目标位置</param>
	public override void MoveTo(Vector3 position)
	{
		if (this.moveAble)
		{
			Vector3 direction = (position - transform.position).normalized;
			Vector2 headingDirection = new Vector2(direction.x * speed, direction.y * speed);

			if (headingDirection.x >= 0)
			{
				spriteRenderer.flipX = true;
			}
			else
			{
				spriteRenderer.flipX = false;
			}
			this.rb.linearVelocity = headingDirection;
		}
	}

	public override void Die()
	{
		// 触发敌人击杀事件
		GameObject killer = GetLastAttacker();
		EnemyKilledEvent killedEvent = new EnemyKilledEvent {
			enemy = gameObject,
			killer = killer,
			score = killScore
		};
		EventBus<EnemyKilledEvent>.Raise(killedEvent);
		
		this.animator.SetTrigger(DEATH_ANIM);

		float delay = 0f;
		this.phrase = PHRASE.DEATH;

		InvokeTrigger();
		Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
	}

    public override void GetDamaged(float value, GameObject attacker)
	{
		base.GetDamaged(value, attacker);
	}
}
