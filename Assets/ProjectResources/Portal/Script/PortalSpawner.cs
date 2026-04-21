using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameStatusSystem.DifficultyApply;

public class PortalSpawner : MonoBehaviour
{
    [Header("生成配置")]
    public GameObject portalPrefab; // Portal预制体
    public GameObject bossPortalPrefab; // Boss Portal预制体
    public float spawnInterval = 30f; // 生成间隔（秒）
    public float spawnDistance = 10f; // 生成距离（玩家周围的圆周半径）
    public KeyCode spawnKey = KeyCode.P; // 手动触发按键
    
    private float spawnTimer = 0f;
    private GameObject player;
    private int portalWaveCount = 0; // 传送门波数计数器

    private void Start()
    {
        // 找到玩家对象
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("未找到玩家对象！");
        }
    }

    private void Update()
    {
        // 定时生成逻辑
        if (player != null)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnPortals();
                spawnTimer = 0f;
            }

            // 手动触发逻辑
            if (Input.GetKeyDown(spawnKey))
            {
                SpawnPortals();
                Debug.Log("手动触发Portal生成");
            }
        }
    }

    private void SpawnPortals()
    {
        if (portalPrefab == null)
        {
            Debug.LogError("Portal预制体未设置！");
            return;
        }

        // 增加波数计数
        portalWaveCount++;
        Debug.Log($"当前波数: {portalWaveCount}");

        // 检查是否需要生成Boss传送门
        if (portalWaveCount % 5 == 0)
        {
            // 每5波生成Boss传送门
            SpawnBossPortals();
        }
        else
        {
            // 生成普通传送门
            int portalCount = CalculatePortalCount();
            Debug.Log($"当前难度下生成{portalCount}个传送门");
            
            for (int i = 0; i < portalCount; i++)
            {
                Vector3 spawnPosition = GetRandomPositionOnCircle(player.transform.position, spawnDistance);
                GameObject portal = Instantiate(portalPrefab, spawnPosition, Quaternion.identity);
                Debug.Log($"生成Portal在位置: {spawnPosition}");
            }
        }
    }

    private void SpawnBossPortals()
    {
        if (bossPortalPrefab == null)
        {
            Debug.LogError("Boss Portal预制体未设置！");
            return;
        }

        // 根据难度决定生成Boss传送门的数量
        float difficulty = GlobalDifficultyLevel.CurrentDifficultyLevel;
        int bossPortalCount = difficulty >= 4 ? 2 : 1;
        
        Debug.Log($"生成{bossPortalCount}个Boss传送门");
        
        for (int i = 0; i < bossPortalCount; i++)
        {
            Vector3 spawnPosition = GetRandomPositionOnCircle(player.transform.position, spawnDistance);
            GameObject bossPortal = Instantiate(bossPortalPrefab, spawnPosition, Quaternion.identity);
            Debug.Log($"生成Boss Portal在位置: {spawnPosition}");
        }
    }

    private int CalculatePortalCount()
    {
        // 根据难度等级计算传送门数量上限N
        float difficulty = GlobalDifficultyLevel.CurrentDifficultyLevel;    // 1~5
        int maxPortalCount = Mathf.Max(1, Mathf.FloorToInt(difficulty));
        
        // 实际数量为N/2~N之间的随机值
        int minCount = Mathf.Max(1, maxPortalCount / 2);
        return Random.Range(minCount, maxPortalCount + 1);
    }

    private Vector3 GetRandomPositionOnCircle(Vector3 center, float radius)
    {
        // 生成随机角度
        float angle = Random.Range(0f, Mathf.PI * 2f);
        
        // 计算圆周上的位置
        float x = center.x + Mathf.Cos(angle) * radius;
        float y = center.y + Mathf.Sin(angle) * radius;
        
        return new Vector3(x, y, center.z);
    }
}
