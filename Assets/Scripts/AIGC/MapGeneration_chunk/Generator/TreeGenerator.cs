using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 树木生成器
/// </summary>
[Serializable]
public class TreeGenerator : PrefabGeneratorBase
{
    [Header("树木配置")]
    public List<GameObject> TreePrefabs;  // 树木预制体列表
    public float TreeHeightVariation = 0.2f;  // 高度变化范围

    /// <summary>
    /// 获取要生成的树木预制体
    /// </summary>
    protected override GameObject GetPrefabToSpawn(Vector2Int worldPos, IChunkData chunkData)
    {
        if (TreePrefabs == null || TreePrefabs.Count == 0)
        {
            Debug.LogWarning("[TreeGenerator] 未设置树木预制体");
            return null;
        }
        
        // 随机选择一个树木预制体
        return TreePrefabs[UnityEngine.Random.Range(0, TreePrefabs.Count)];
    }

    /// <summary>
    /// 检查位置是否适合生成树木
    /// </summary>
    protected override bool IsValidSpawnPosition(Vector2Int worldPos, IChunkData chunkData)
    {
        // 只在草地上生成
        return chunkData.GetData<TerrainType>(worldPos) == TerrainType.Grass;
    }
}
