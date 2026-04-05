using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 物品掉落配置
/// </summary>
[System.Serializable]
public class ItemDropConfig
{
    public GameObject itemPrefab; // 物品预制体
    public float weight = 1f; // 掉落权重
}

/// <summary>
/// 物品掉落事件
/// 在随机位置生成物品
/// </summary>
public class ItemDropEvent : RandomEvent
{
    [Header("物品掉落配置")]
    public List<ItemDropConfig> itemConfigs = new List<ItemDropConfig>(); // 物品配置列表
    public int minItemCount = 1; // 最小物品数量
    public int maxItemCount = 3; // 最大物品数量
    public float spawnRadius = 8f; // 生成半径
    public float dropInterval = 0.5f; // 物品掉落时间间隔（秒）
    public GameObject dropAnimationPrefab; // 掉落动画预制体

    public override void Trigger()
    {
        if (itemConfigs.Count == 0 || Player.instance == null)
            return;

        // 显示事件消息
        GameManager.instance.ShowText("物品掉落！", 120, Color.green, Player.instance.transform.position + new Vector3(0, 2, 0), Vector3.up, 3.0f);

        // 启动协程生成物品
        StartCoroutine(SpawnItems());
    }

    /// <summary>
    /// 协程：依次生成物品
    /// </summary>
    /// <returns></returns>
    private System.Collections.IEnumerator SpawnItems()
    {
        Vector3 playerPos = Player.instance.transform.position;
        int itemCount = Random.Range(minItemCount, maxItemCount + 1);

        for (int i = 0; i < itemCount; i++)
        {
            // 随机选择物品
            GameObject selectedItem = GetRandomItem();
            if (selectedItem == null)
                continue;

            // 随机生成位置
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float distance = Random.Range(3f, spawnRadius);
            Vector3 spawnPos = playerPos + new Vector3(
                Mathf.Cos(angle) * distance,
                Mathf.Sin(angle) * distance,
                0f
            );

            // 播放掉落动画
            if (dropAnimationPrefab != null)
            {
                GameObject animationObj = Instantiate(dropAnimationPrefab, spawnPos, Quaternion.identity);
                
                Destroy(animationObj, .6f);
                // 生成物品
                Instantiate(selectedItem, spawnPos, Quaternion.identity);

                // 等待指定的时间间隔
                yield return new WaitForSeconds(dropInterval);
            }
            else {
                // 生成物品
                Instantiate(selectedItem, spawnPos, Quaternion.identity);
                // 等待指定的时间间隔
                yield return new WaitForSeconds(dropInterval);
            }

            
        }
    }

    /// <summary>
    /// 根据权重随机选择物品
    /// </summary>
    /// <returns>选中的物品预制体</returns>
    private GameObject GetRandomItem()
    {
        if (itemConfigs.Count == 0)
            return null;

        // 计算总权重
        float totalWeight = 0f;
        foreach (var config in itemConfigs)
        {
            totalWeight += config.weight;
        }

        // 随机选择
        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var config in itemConfigs)
        {
            currentWeight += config.weight;
            if (randomValue <= currentWeight)
            {
                return config.itemPrefab;
            }
        }

        return itemConfigs[0].itemPrefab; // 保底返回第一个物品
    }

    public override float CalculateProbability()
    {
        // 可以根据玩家物品数量调整概率
        return baseProbability;
    }
}
