using UnityEngine;
using System.Collections;
using GlobalEvents;
using GameStatusSystem.DifficultyApply;

[RequireComponent(typeof(EnemyAI), typeof(Rigidbody2D))]
public class Enemy : Character
{
	[SerializeField] private GameObject _prefab;
	public LocalEventBus localEventBus;

	public float attackRate = 0.3f;
	public float pushForce = 3f;
	protected float lastAttack = 0f;
	public int killScore = 100;
	public float minDifficultyLevel = 1f;


#region hook
	protected override void Awake()
	{
		base.Awake();
		localEventBus = new LocalEventBus();
	}

	public override void Start()
	{
		// base.Start();
		healthBar.Initialize(maxHealth * GlobalDifficultyLevel.CurrentDifficultyLevel);
	}
#endregion


#region override methods
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
		
		Destroy(gameObject);
	}

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
#endregion


	public virtual void MoveTo(Vector3 position)
	{
		if (this.moveAble)
		{
			Vector3 direction = (position - transform.position).normalized;
			Vector2 headingDirection = new Vector2(direction.x * speed, direction.y * speed);
			this.rb.linearVelocity = headingDirection;
		}
	}

	
	private void AttackTo(Character target)
	{
		Debug.Log("Attack to " + target.name);
		target.GetDamaged(this.damage, gameObject);

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
}
