using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStatusSystem.DifficultyApply;

public class Portal : Collidable
{
    [Header("敌人生成配置")]
    public GameObject[] enemyPrefabs; // 敌人预制体数组
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    public float spawnInterval = 2f; // 生成间隔（秒）
    // 使用全局难度系统中的最大敌人数量
    private int maxEnemies = 2;
    public float spawnRadius = 2f; // 生成半径
    
    [Header("动画配置")]
    public Animator animator; // 动画控制器
    public const string closeAnimationParam = "isClosing"; // 关闭动画触发参数
    public const string openAnimationParam = "opened"; // 开启动画触发参数
    
    private int spawnedEnemies = 0;
    private float spawnTimer = 0f;
    private bool isSpawning = false;

    protected void Start()
    {
        // isSpawning = true;
        // 更新参数配置
        maxEnemies = GlobalDifficultyLevel.MaxEnemies;
        Debug.Log($"最大敌人数量：{maxEnemies}");

        // 确保有动画控制器
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // 筛选出当前难度可以生成的敌人
        float currentDifficulty = GlobalDifficultyLevel.CurrentDifficultyLevel;
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            Enemy enemyComponent = enemyPrefab.GetComponent<Enemy>();
            if (enemyComponent != null && enemyComponent.minDifficultyLevel <= currentDifficulty)
            {
                activeEnemies.Add(enemyPrefab);
            }
        }

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
            // 触发关闭动画
            if (animator != null)
            {
                animator.SetBool(closeAnimationParam, true);
            }
            // ProcessClose();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("敌人预制体数组为空！");
            return;
        }

        // 筛选出当前难度可以生成的敌人
        // List<GameObject> availableEnemies = new List<GameObject>();
        

        // 如果没有可用的敌人，返回
        if (activeEnemies.Count == 0)
        {
            Debug.LogWarning("没有当前难度可以生成的敌人！");
            return;
        }

        // 从可用敌人中随机选择一个
        int randomIndex = Random.Range(0, activeEnemies.Count);
        GameObject selectedEnemyPrefab = activeEnemies[randomIndex];

        // 随机生成位置（在传送门周围）
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

        // 生成敌人
        GameObject enemy = Instantiate(selectedEnemyPrefab, randomPosition, Quaternion.identity);
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

    public void processOpen() {
        if (animator != null)
        {
            animator.SetBool(openAnimationParam, true);
        }
        isSpawning = true;
    }
    public void ProcessClose()
    {
        Debug.Log("传送门销毁");
        // 可以在这里添加销毁前的清理逻辑
        Destroy(gameObject);
    }
}
