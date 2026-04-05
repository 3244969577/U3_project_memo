using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 岩石生成器
/// </summary>
[Serializable]
public class RockGenerator : PrefabGeneratorBase
{
    [Header("岩石配置")]
    public List<GameObject> RockPrefabs;  // 岩石预制体列表
    public float RockRotationVariation = 45f;  // 旋转变化范围

    /// <summary>
    /// 获取要生成的岩石预制体
    /// </summary>
    protected override GameObject GetPrefabToSpawn(Vector2Int worldPos, IChunkData chunkData)
    {
        if (RockPrefabs == null || RockPrefabs.Count == 0)
        {
            Debug.LogWarning("[RockGenerator] 未设置岩石预制体");
            return null;
        }
        
        // 随机选择一个岩石预制体
        GameObject prefab = RockPrefabs[UnityEngine.Random.Range(0, RockPrefabs.Count)];
        
        // 随机旋转
        if (prefab != null)
        {
            float randomRotation = UnityEngine.Random.Range(-RockRotationVariation, RockRotationVariation);
            prefab.transform.rotation = Quaternion.Euler(0, 0, randomRotation);
        }
        
        return prefab;
    }

    /// <summary>
    /// 检查位置是否适合生成岩石
    /// </summary>
    protected override bool IsValidSpawnPosition(Vector2Int worldPos, IChunkData chunkData)
    {
        // 只在山地和岩石地形上生成
        TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
        return terrain == TerrainType.Mountain || terrain == TerrainType.Rock;
    }
}
