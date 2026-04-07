using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawner : MonoBehaviour
{
    [Header("生成配置")]
    public GameObject portalPrefab; // Portal预制体
    public float spawnInterval = 30f; // 生成间隔（秒）
    public float spawnDistance = 10f; // 生成距离（玩家周围的圆周半径）
    public KeyCode spawnKey = KeyCode.P; // 手动触发按键
    
    private float spawnTimer = 0f;
    private GameObject player;

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
                SpawnPortal();
                spawnTimer = 0f;
            }

            // 手动触发逻辑
            if (Input.GetKeyDown(spawnKey))
            {
                SpawnPortal();
                Debug.Log("手动触发Portal生成");
            }
        }
    }

    private void SpawnPortal()
    {
        if (portalPrefab == null)
        {
            Debug.LogError("Portal预制体未设置！");
            return;
        }

        // 计算玩家附近圆周上的随机位置
        Vector3 spawnPosition = GetRandomPositionOnCircle(player.transform.position, spawnDistance);

        // 生成Portal
        GameObject portal = Instantiate(portalPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"生成Portal在位置: {spawnPosition}");
    }

    /// <summary>
    /// 在指定圆心和半径的圆周上随机生成一个位置
    /// </summary>
    /// <param name="center">圆心位置</param>
    /// <param name="radius">半径</param>
    /// <returns>圆周上的随机位置</returns>
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
