using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  提供ShootBullet方法，用于发射子弹
public class Shootable : MonoBehaviour
{
	// public Transform firePoint;
	[SerializeField] private WeightedRandomList<GameObject> bulletPrefabs = new WeightedRandomList<GameObject>();

	public float bulletSpeed = 20f;
	public float fireRate = 0.3f;
	public float spreadAngle = 0f; // 散射角度（度）

	private float lastShot = 0f;
	private bool CanShoot() => Time.time > fireRate + lastShot;
	
	public void ShootBullet(Vector3 targetPosition)
	{
		if (CanShoot())
		{
			Shoot(targetPosition);
			lastShot = Time.time;
		}
	}

	private void Shoot(Vector3 targetPosition)
	{
		GameObject bulletPrefab = bulletPrefabs.GetRandomItem();
		if (bulletPrefab != null)
		{
			GameObject newBullet = GlobalObjectPool.Instance.Spawn(bulletPrefab, transform.position, transform.rotation);
			SoundManager.Instance.PlaySound("Laser1");
			newBullet.GetComponent<Bullet>().attacker = Player.instance.gameObject;
			newBullet.GetComponent<Bullet>().moveSpeed = bulletSpeed;
			
			// 设置子弹初始朝向
			Vector3 lookDirection = targetPosition - newBullet.transform.position;
			
			// 应用散射
			if (spreadAngle > 0f)
			{
				// 生成随机散射角度
				float randomAngle = UnityEngine.Random.Range(-spreadAngle, spreadAngle) * Mathf.Deg2Rad;
				// 绕Z轴旋转方向向量
				lookDirection = Quaternion.Euler(0, 0, randomAngle * Mathf.Rad2Deg) * lookDirection;
			}
			
			lookDirection.Normalize();
			
			// 计算初始朝向角度
			float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
			newBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
		}
	}
}