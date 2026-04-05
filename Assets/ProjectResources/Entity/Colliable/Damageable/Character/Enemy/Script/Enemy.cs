﻿﻿﻿﻿﻿﻿﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 敌人基类 - 所有敌人的基础类
/// - 继承自Character，包含角色的基本属性和行为
/// - 实现敌人的移动、攻击和死亡逻辑
/// </summary>
[RequireComponent(typeof(EnemyAI), typeof(Rigidbody2D))]
public class Enemy : Character 
{

	/// <summary>
	/// 敌人死亡时掉落的奖励物品
	/// - 可以是金币、药水或其他收集品
	/// </summary>
	public GameObject reward;

	/// <summary>
	/// 敌人死亡时掉落奖励的概率
	/// - 范围: 0-1，1表示100%掉落
	/// </summary>
	public float dropRate;

	/// <summary>
	/// 敌人的攻击间隔（秒）
	/// - 控制敌人攻击的频率
	/// </summary>
	public float attackRate = 0.3f;

	/// <summary>
	/// 敌人攻击时对玩家的推力
	/// - 影响攻击时玩家被推开的距离
	/// </summary>
	public float pushForce = 30000f;

	/// <summary>
	/// 上次攻击的时间
	/// - 用于控制攻击间隔
	/// </summary>
	protected float lastAttack = 0f;

	public virtual void MoveTo(Vector3 position)
	{
		if (this.moveAble)
		{
			Vector3 direction = (position - transform.position).normalized;
			Vector2 headingDirection = new Vector2(direction.x * speed, direction.y * speed);
			this.rb.linearVelocity = headingDirection;
		}
	}

	public void AttackTo(Damageable target)
    {
		this.Attack(target);
	}

	protected override void Die()
	{
		if (reward)
        {
			float rand = Random.Range(0f, 1f);
			if (rand <= this.dropRate)
			{
				GameObject obj = Instantiate(reward, transform.position, transform.rotation);
				obj.transform.position = transform.position;
			}
		}
		Destroy(gameObject);
	}

	public override void GetDamaged(float value)
	{
		this.healthBar.UpdateHealth(-value);
	}

	// Basic attack, deal damage on touch
	protected override void Attack(Damageable target)
	{
		if (this.moveAble)
		{
			Vector3 direction = (target.transform.position - transform.position).normalized;
			Vector2 headingDirection = new Vector2(direction.x * speed*2, direction.y * speed*2);
			this.rb.linearVelocity = headingDirection;
		}
	}

	protected virtual void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			Damageable target = collision.gameObject.GetComponent<Damageable>();
			
			if (Time.time > this.attackRate + this.lastAttack)
			{
				target.GetDamaged(this.damage);

				// Print damage
				GameManager.instance.ShowText((-this.damage).ToString(), 100, Color.red, collision.transform.position + new Vector3(0.5f, 1.75f, 0), Vector3.up, 2.0f);

				this.lastAttack = Time.time;
			}
		}
	}
}
