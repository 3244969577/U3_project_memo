﻿using UnityEngine;
using System.Collections;
using GameStatusSystem.PlayerStatus.Events;


[RequireComponent(typeof(EnemyAI), typeof(Rigidbody2D))]
public class Enemy : Character 
{

	public GameObject reward;
	public float dropRate;
	public float attackRate = 0.3f;
	public float pushForce = 3f;
	protected float lastAttack = 0f;
	public int killScore = 100;
	public float minDifficultyLevel = 1f;

	public virtual void MoveTo(Vector3 position)
	{
		if (this.moveAble)
		{
			Vector3 direction = (position - transform.position).normalized;
			Vector2 headingDirection = new Vector2(direction.x * speed, direction.y * speed);
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
		
		// 处理掉落
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

	public override void GetDamaged(float value, GameObject attacker)
	{
		base.GetDamaged(value, attacker);
	}

	// Basic attack, deal damage on touch
	public override void Attack(Character target)
	{
		
		if (Time.time > this.attackRate + this.lastAttack) {
			Debug.Log("Attack to " + target.name);
			this.lastAttack = Time.time;
			AttackTo(target);

			// 打印伤害
			GameManager.Instance.ShowText((-this.damage).ToString(), 100, Color.red, target.transform.position + new Vector3(0.5f, 1.75f, 0), Vector3.up, 2.0f);
		}
	}
	private void AttackTo(Character target)
	{
		Debug.Log("Attack to " + target.name);
		target.GetDamaged(this.damage);

		Vector3 direction = (target.transform.position - transform.position).normalized;
		Vector2 headingDirection = new Vector2(direction.x * speed*2, direction.y * speed*2);

		// 击退
		Rigidbody2D targetRb = target.GetRigidbody();
		if (targetRb != null)
		{
			targetRb.AddForce(direction * pushForce, ForceMode2D.Impulse);
		}

		if (this.moveAble)
		{
			this.rb.linearVelocity = headingDirection;
		}
	}

	protected virtual void OnCollisionStay2D(Collision2D collision)
	{

	}
}
