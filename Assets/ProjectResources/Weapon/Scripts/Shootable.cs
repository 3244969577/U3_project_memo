using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  提供ShootBullet方法，用于发射子弹
public class Shootable : MonoBehaviour
{
	// public Transform firePoint;
	public GameObject bulletPrefab;

	public float shootingForce = 20f;
	public float fireRate = 0.3f;

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
		GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
		SoundManager.Instance.PlaySound("Laser1");
		newBullet.GetComponent<Bullet>().attacker = Player.instance.gameObject;
		
		
		// 施加初始速度
		Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
		// rb.AddForce(shootingForce * firePoint.right, ForceMode2D.Impulse);
		Vector3 lookDirection = targetPosition - newBullet.transform.position;
		
		lookDirection.Normalize();
		rb.AddForce(shootingForce * lookDirection, ForceMode2D.Impulse);
	}
}