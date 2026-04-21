using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStatusSystem.DifficultyApply;
using GlobalEvents;

public class BossPortal : Collidable
{
    [Header("Boss生成配置")]
    public GameObject[] bossPrefabs; // Boss预制体数组
    public float spawnRadius = 2f; // 生成半径
    
    [Header("动画配置")]
    public Animator animator; // 动画控制器
    public const string closeAnimationParam = "isClosing"; // 关闭动画触发参数
    public const string openAnimationParam = "opened"; // 开启动画触发参数
    
    private GameObject spawnedBoss; // 已生成的Boss
    private bool hasSpawnedBoss = false; // 是否已生成Boss
    private bool isSpawning = false;

    protected void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    protected void Update()
    {
        // 检查Boss是否已被击杀
        if (hasSpawnedBoss && spawnedBoss == null)
        {
            // Boss已被击杀，关闭传送门
            ProcessClose();
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefabs == null || bossPrefabs.Length == 0)
        {
            Debug.LogError("Boss预制体数组为空！");
            return;
        }

        // 随机选择一个Boss预制体
        int randomIndex = Random.Range(0, bossPrefabs.Length);
        GameObject selectedBossPrefab = bossPrefabs[randomIndex];

        // 在传送门周围生成Boss
        Vector2 randomPosition = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;

        // 生成Boss
        spawnedBoss = Instantiate(selectedBossPrefab, randomPosition, Quaternion.identity);
        hasSpawnedBoss = true;

        // 触发Boss生成事件
        EventBus<BossSpawnEvent>.Raise(new BossSpawnEvent{boss = spawnedBoss});

        Debug.Log($"生成Boss {spawnedBoss.name}");
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("玩家进入Boss传送门区域");
        }
    }

    public void ProcessOpen()
    {
        if (animator != null)
        {
            animator.SetBool(openAnimationParam, true);
        }
        isSpawning = true;
        
        // 立即生成Boss
        SpawnBoss();
    }

    public void ProcessClose()
    {
        Debug.Log("Boss传送门关闭");
        if (animator != null)
        {
            animator.SetBool(closeAnimationParam, true);
        }
        isSpawning = false;
    }
}
