using System;
using UnityEngine;

/// <summary>
/// Boss敌人类 - 游戏中的大型敌人
/// - 继承自Enemy，包含敌人的基本属性和行为
/// - 实现特殊的攻击方式和死亡事件
/// </summary>
public class Boss : Enemy
{
    /// <summary>
    /// Boss死亡时触发的事件
    /// - 可以用于触发传送门生成或其他游戏事件
    /// </summary>
    public event EventHandler OnDeathTrigger;

	/// <summary>
	/// 死亡动画触发名称
	/// </summary>
	const string DEATH_ANIM = "onDeath";

	/// <summary>
	/// 子弹发射的力量
	/// - 影响子弹的飞行速度和距离
	/// </summary>
	public float shootingForce = 20f;

	/// <summary>
	/// 子弹预制体
	/// - Boss攻击时发射的子弹类型
	/// </summary>
	public GameObject bulletPrefab;
	
	protected override void Start()
    {
		base.Start();
		SoundManager.instance.PlaySong("BattleTheme");
    }

	// Trigger the trigger zone
	protected void InvokeTrigger()
	{
		EventHandler handler = OnDeathTrigger;
		if (handler != null)
			handler(this, EventArgs.Empty);
		else
			Debug.LogWarning("Boss enemy has no OnDeathTrigger event handler.");

	}

    protected override void Die()
	{
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
		SoundManager.instance.StopSound("BattleTheme");

		InvokeTrigger();
		Destroy(gameObject);
	}

	protected override void Attack(Damageable target)
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
