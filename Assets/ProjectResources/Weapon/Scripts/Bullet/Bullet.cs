using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

	public enum BulletType
	{
		Player, // 玩家子弹
		Enemy   // 敌人子弹
	}

	public BulletType bulletType = BulletType.Player;
	public float damage;
	public GameObject hitEffect = null;
	public float pushForce = 0; // 仅敌人子弹使用
	public float lifeTime = 2f; // 子弹生命周期（秒）
	private float lifeTimer = 0f; // 生命周期计时器
	public BulletEffectManager effectManager;

	protected void Awake()
	{
		// effectManager = GetComponent<BulletEffectManager>();
		if (effectManager != null)
		{
			effectManager.Initialize(this);
		}
		
		// 确保子弹有Bullet标签
		gameObject.tag = "Bullet";
		
		// 禁用与其他子弹的碰撞
		Collider2D[] otherBullets = Physics2D.OverlapCircleAll(transform.position, 100f, LayerMask.GetMask("Default"));
		foreach (Collider2D collider in otherBullets)
		{
			if (collider.gameObject != gameObject && collider.gameObject.tag == "Bullet")
			{
				Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider, true);
			}
		}
	}

	protected void Update()
	{
		if (effectManager != null)
		{
			effectManager.Update();
		}
		
		// 更新子弹朝向
		UpdateBulletRotation();
		
		// 自动销毁逻辑
		lifeTimer += Time.deltaTime;
		if (lifeTimer >= lifeTime)
		{
			Destroy(gameObject);
		}
	}
	
	/// <summary>
	/// 更新子弹朝向，使其与运动方向一致
	/// </summary>
	private void UpdateBulletRotation()
	{
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		if (rb != null && rb.linearVelocity.magnitude > 0.1f)
		{
			// 计算运动方向的角度
			Vector2 direction = rb.linearVelocity.normalized;
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			
			// 设置子弹的旋转
			transform.rotation = Quaternion.Euler(0, 0, angle);
		}
	}

	public void OnShoot()
	{
		
		if (effectManager != null)
		{
			// Debug.Log("Bullet OnShoot");
			effectManager.OnShoot();
		}
	}

	protected void DealDamage(Damageable target)
    {
		target.GetDamaged(this.damage);
	}

	private void OnTriggerEnter2D(Collider2D collision) {

		// 先处理效果管理器的碰撞事件
		if (effectManager != null)
		{
			effectManager.OnTriggerEnter2D(collision);
		}

		// 根据子弹类型处理碰撞忽略
		if (bulletType == BulletType.Player)
		{
			// 玩家子弹忽略Player和NPC
			if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "NPC")
			{
				return;
			}
		}
		else if (bulletType == BulletType.Enemy)
		{
			// 敌人子弹忽略Enemy和NPC
			if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("NPC"))
			{
				return;
			}
		}

		// 处理墙壁碰撞
		if (collision.gameObject.tag == "Wall")
		{
			SpawnHitEffect();
			Destroy(gameObject);
		}

		// 处理可伤害目标碰撞
		else if (collision.gameObject.GetComponent<Damageable>())
		{
			SpawnHitEffect();

			// 显示伤害
			GameManager.instance.ShowText((-this.damage).ToString(), 100, Color.red, collision.transform.position + new Vector3(0.5f, 1.75f, 0), Vector3.up, 2.0f);
			
			// 敌人子弹击中目标后销毁
			if (bulletType == BulletType.Enemy)
			{
				Destroy(gameObject);
			}

			DealDamage(collision.gameObject.GetComponent<Damageable>());
		}

		// 处理可破坏物体碰撞
		else if (collision.gameObject.tag == "Destructible")
        {
			Destroy(collision.gameObject);
			Destroy(gameObject);
		}

	}

	// 生成击中效果
	private void SpawnHitEffect()
	{
		if (hitEffect != null)
		{
			GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
			Destroy(effect, 1f);
		}
	}

}
