using UnityEngine;
using System.Collections;

/// <summary>
/// 实体生成器 - 根据设定的速率持续生成实体
/// - 存储要生成的实体预制体
/// - 可调节的生成速率
/// - 根据时间间隔自动生成实体
/// </summary>
public class Spawner : MonoBehaviour {

	public GameObject entity;

	[Range(1f, 200f)]
	public float rateOfSpawn = 1f;

	private float timer = 0f;
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;

		if (timer >= rateOfSpawn) {
			Instantiate(entity, transform.position, Quaternion.identity);
			timer = 0;
		}
	}
}
