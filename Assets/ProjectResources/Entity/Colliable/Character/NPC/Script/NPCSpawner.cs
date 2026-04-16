using UnityEngine;

/// <summary>
/// NPC生成器
/// 在游戏开始时在Player附近生成一个NPC
/// </summary>
public class NPCSpawner : MonoBehaviour
{
    [Header("NPC配置")]
    public GameObject NPCPrefab; // NPC预制体
    public float SpawnDistance = 3f; // 生成距离
    public float SpawnDistanceVariation = 1f; // 距离变化范围

    private void Start()
    {
        // 等待Player实例创建
        StartCoroutine(SpawnNPCWhenPlayerReady());
    }

    private System.Collections.IEnumerator SpawnNPCWhenPlayerReady()
    {
        // 等待Player实例可用
        while (Player.instance == null)
        {
            yield return null;
        }

        // 在Player附近生成NPC
        SpawnNPC();
    }

    private void SpawnNPC()
    {
        if (NPCPrefab == null || Player.instance == null)
        {
            Debug.LogWarning("[NPCSpawner] NPC预制体或Player实例不存在");
            return;
        }

        // 计算生成位置
        Vector3 playerPosition = Player.instance.transform.position;
        float randomDistance = SpawnDistance + Random.Range(-SpawnDistanceVariation, SpawnDistanceVariation);
        float randomAngle = Random.Range(0, Mathf.PI * 2);
        
        Vector3 spawnPosition = new Vector3(
            playerPosition.x + Mathf.Cos(randomAngle) * randomDistance,
            playerPosition.y + Mathf.Sin(randomAngle) * randomDistance,
            playerPosition.z
        );

        // 生成NPC
        GameObject npcInstance = Instantiate(NPCPrefab, spawnPosition, Quaternion.identity);
        npcInstance.name = $"NPC_{spawnPosition.x}_{spawnPosition.y}";

        Debug.Log($"[NPCSpawner] 在Player附近生成了NPC，位置: {spawnPosition}");
    }
}