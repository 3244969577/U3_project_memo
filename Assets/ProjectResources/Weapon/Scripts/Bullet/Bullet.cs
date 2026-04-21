using UnityEngine;
using System.Collections;
using GlobalEvents;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour {

	public string[] targetTags; // 能够命中的目标标签数组
	public float damage;
	public GameObject hitEffect = null;
	public float pushForce = 0; 
	public float lifeTime = 2f; // 子弹生命周期（秒）
	// private float lifeTimer = 0f; // 生命周期计时器
	public GameObject attacker = null;	// 发射者

	protected void Awake()
	{
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
		Destroy(gameObject, lifeTime);
	}

	protected void Update()
	{
		// 更新子弹朝向
		UpdateBulletRotation();
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

	private void OnTriggerEnter2D(Collider2D collision) 
	{
		// 检查是否是目标标签
		bool isTarget = true; // 默认所有可伤害对象都能命中
		if (targetTags != null && targetTags.Length > 0)
		{
			isTarget = false;
			foreach (string tag in targetTags)
			{
				if (collision.CompareTag(tag))
				{
					isTarget = true;
					break;
				}
			}
		}

		// 处理可伤害目标碰撞
		if (collision.gameObject.GetComponent<Damageable>() && isTarget)
		{
			HandleHitTarget(collision.gameObject);
			// 触发子弹命中事件
			EventBus<BulletHitEvent>.Raise(new BulletHitEvent {
				attacker = attacker,
				bullet = gameObject,
				target = collision.gameObject,
				hitPosition = transform.position,
				damage = this.damage
			});
			// SpawnHitEffect();
			// Destroy(gameObject);
			// 显示伤害
			// GameManager.Instance.ShowText((-this.damage).ToString(), 100, Color.white, collision.transform.position + new Vector3(0.5f, 1.75f, 0), Vector3.up, 2.0f);
			
			// 击中目标后销毁
			
			// DealDamage(collision.gameObject.GetComponent<Damageable>());
		}

	}

	private void HandleHitTarget(GameObject target)
	{
		SpawnHitEffect();
		Destroy(gameObject);
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
