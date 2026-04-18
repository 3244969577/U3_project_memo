using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  提供ShootBullet方法，用于发射子弹
public class Shootable : MonoBehaviour
{
	public Transform firePoint;
	public GameObject bulletPrefab;

	public float shootingForce = 20f;
	public float fireRate = 0.3f;

	private float lastShot = 0f;

	
	public void ShootBullet()
	{

		if (Time.time > fireRate + lastShot)
		{
			GameObject newBullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
			SoundManager.Instance.PlaySound("Laser1");
			Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
			rb.AddForce(shootingForce * firePoint.right, ForceMode2D.Impulse);
			
			// 调用子弹的射击效果
			// Bullet bullet = newBullet.GetComponent<Bullet>();
			// if (bullet != null)
			// {
			// 	bullet.OnShoot();
			// }
			
			Destroy(newBullet, 2f);
			lastShot = Time.time;
		}
	}
}