using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Collidable
{
    [Header("敌人生成配置")]
    public GameObject[] enemyPrefabs; // 敌人预制体数组
    public float spawnInterval = 2f; // 生成间隔（秒）
    public int maxEnemies = 10; // 最大生成敌人数量
    public float spawnRadius = 2f; // 生成半径
    
    private int spawnedEnemies = 0;
    private float spawnTimer = 0f;
    private bool isSpawning = false;

    protected void Start()
    {
        isSpawning = true;
    }

    protected void Update()
    {
        if (isSpawning && spawnedEnemies < maxEnemies)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnEnemy();
                spawnTimer = 0f;
            }
        }
        else if (spawnedEnemies >= maxEnemies && isSpawning)
        {
            isSpawning = false;
            Debug.Log("敌人生成完成，传送门关闭");
            // 可以在这里添加关闭动画或销毁传送门的逻辑
            OnDestroy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("敌人预制体数组为空！");
            return;
        }

        // 随机选择一个敌人预制体
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];

        // 随机生成位置（在传送门周围）
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

        // 生成敌人
        GameObject enemy = Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
        spawnedEnemies++;
        GameManager.Instance.ShowText($"{spawnedEnemies}/{maxEnemies}", 100, Color.yellow, randomPosition, Vector3.up, 2.0f);

        Debug.Log($"生成敌人 {enemy.name}，已生成 {spawnedEnemies}/{maxEnemies}");
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家进入传送门区域");
            // 可以在这里添加玩家进入的逻辑
        }
    }

    protected void OnDestroy()
    {
        Debug.Log("传送门关闭");
        Destroy(gameObject);
    }
}
