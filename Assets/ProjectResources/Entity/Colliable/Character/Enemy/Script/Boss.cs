using System;
using UnityEngine;
using GlobalEvents;

public class Boss : Enemy
{
    public event EventHandler OnDeathTrigger;
	const string DEATH_ANIM = "onDeath";
	public float shootingForce = 20f;
	public GameObject bulletPrefab;
	
	public override void Start()
    {
		base.Start();
		SoundManager.Instance.PlaySong("BattleTheme");
    }

	// Trigger the trigger zone
	public void InvokeTrigger()
	{
		EventHandler handler = OnDeathTrigger;
		if (handler != null)
			handler(this, EventArgs.Empty);
		else
			Debug.LogWarning("Boss enemy has no OnDeathTrigger event handler.");

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
		if (reward)
		{
			float rand = UnityEngine.Random.Range(0f, 1f);
			if (rand <= this.dropRate)
			{
				GameObject obj = Instantiate(reward, transform.position, transform.rotation);
				obj.transform.position = transform.position;
			}	
		}
		SoundManager.Instance.StopSound("BattleTheme");
		
		// 触发Boss击杀事件
		EventBus<BossKilledEvent>.Raise(new BossKilledEvent{boss = gameObject});

		InvokeTrigger();
		Destroy(gameObject, 1f);
	}

	public override void Attack(Character target)
	{
		if (Time.time > this.attackRate + this.lastAttack)
		{
			GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
			Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();

			Vector3 direction = (target.transform.position - transform.position).normalized;

			rb.AddForce(shootingForce * direction, ForceMode2D.Impulse);
			Destroy(newBullet, 5f);
			lastAttack = Time.time;
		}
	}
}
