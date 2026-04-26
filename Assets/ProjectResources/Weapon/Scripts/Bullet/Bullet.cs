using UnityEngine;
using System.Collections;
using GlobalEvents;
using System;


[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour, IPoolable
{
	[SerializeField] private GameObject prefab;
	public GameObject Prefab { get => prefab; }


	public float damage;
	public float moveSpeed = 10f; // 移动速度
	public float pushForce = 0; 

	public float lifeTime = 2f; // 子弹生命周期（秒）
	public string[] targetTags; // 能够命中的目标标签数组
	public GameObject hitEffect = null;
	public GameObject attacker = null;	// 发射者


#region hook
	public event Action<GameObject> OnBulletHit;

#endregion


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
		// Destroy(gameObject, lifeTime);
		Invoke("Recycle", lifeTime);
	}

	public void Recycle()
	{
		GlobalObjectPool.Instance.Recycle(gameObject);
	}

	protected void Update()
	{
		// 朝子弹朝向移动
		transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
	}

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
		// 处理可伤害目标碰撞
		if (collision.gameObject.GetComponent<Damageable>() && IsTarget(collision.gameObject))
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
			// 触发子弹命中事件
		}
	}

	private void HandleHitTarget(GameObject target)
	{
		SpawnHitEffect();
		// 触发子弹命中事件
		OnBulletHit?.Invoke(target);
		Recycle();
	}

	// 生成击中效果
	private void SpawnHitEffect()
	{
		if (hitEffect != null)
		{
			GameObject effect = GlobalObjectPool.Instance.Spawn(hitEffect, transform.position, Quaternion.identity);
			GlobalObjectPool.Instance.Recycle(effect, hitEffect);
		}
	}


#region helper
	public bool IsTarget(GameObject target)
	{
		return targetTags != null && (
			targetTags.Length == 0 ||
			ContainsTag(target.tag)
		);
	}

	private bool ContainsTag(string tag)
	{
		if (targetTags == null)
		{
			return false;
		}
		
		foreach (string targetTag in targetTags)
		{
			if (tag == targetTag)
			{
				return true;
			}
		}
		
		return false;
	}
#endregion


}
