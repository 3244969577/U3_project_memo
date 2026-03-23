using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 纪念碑配置项
/// </summary>
[System.Serializable]
public class MonumentPrefabConfig
{
    public GameObject Prefab;         // 纪念碑预制体
    public float Weight = 1f;         // 生成权重
    public float RotationVariation;   // 旋转变化范围
    public List<TerrainType> AllowedTerrainTypes; // 允许的地形类型
}

/// <summary>
/// 纪念碑生成器
/// 在山地地形上生成多种类型的纪念碑预制体
/// </summary>
[System.Serializable]
public class MonumentGenerator : PrefabGeneratorBase
{
    [Header("纪念碑配置")]
    public List<MonumentPrefabConfig> MonumentConfigs; // 纪念碑配置列表

    /// <summary>
    /// 初始化生成器
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        
        // 确保允许的地形类型包含山地
        if (AllowedTerrainTypes == null)
        {
            AllowedTerrainTypes = new List<TerrainType>();
        }
        
        if (!AllowedTerrainTypes.Contains(TerrainType.Mountain))
        {
            AllowedTerrainTypes.Add(TerrainType.Mountain);
        }
    }

    /// <summary>
    /// 获取要生成的预制体
    /// </summary>
    protected override GameObject GetPrefabToSpawn(Vector2Int worldPos, IChunkData chunkData)
    {
        if (MonumentConfigs == null || MonumentConfigs.Count == 0)
        {
            Debug.LogWarning("[MonumentGenerator] 预制体配置列表为空");
            return null;
        }
        
        // 获取当前位置的地形类型
        TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
        
        // 筛选适合当前地形的预制体配置
        List<MonumentPrefabConfig> validConfigs = new List<MonumentPrefabConfig>();
        foreach (var config in MonumentConfigs)
        {
            if (config.AllowedTerrainTypes == null || config.AllowedTerrainTypes.Contains(terrain))
            {
                validConfigs.Add(config);
            }
        }
        
        if (validConfigs.Count == 0)
        {
            Debug.LogWarning($"[MonumentGenerator] 没有适合地形 {terrain} 的预制体配置");
            return null;
        }
        
        // 根据权重选择预制体
        GameObject prefab = SelectPrefabByWeight(validConfigs);
        
        // 应用旋转变化
        if (prefab != null)
        {
            MonumentPrefabConfig config = GetConfigForPrefab(prefab);
            if (config != null && config.RotationVariation > 0)
            {
                float randomRotation = UnityEngine.Random.Range(-config.RotationVariation, config.RotationVariation);
                prefab.transform.rotation = Quaternion.Euler(0, 0, randomRotation);
            }
        }
        
        return prefab;
    }

    /// <summary>
    /// 根据权重选择预制体
    /// </summary>
    private GameObject SelectPrefabByWeight(List<MonumentPrefabConfig> configs)
    {
        float totalWeight = 0;
        foreach (var config in configs)
        {
            totalWeight += config.Weight;
        }
        
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0;
        
        foreach (var config in configs)
        {
            currentWeight += config.Weight;
            if (randomValue <= currentWeight)
            {
                return config.Prefab;
            }
        }
        
        // 兜底返回第一个预制体
        return configs[0].Prefab;
    }

    /// <summary>
    /// 获取预制体对应的配置
    /// </summary>
    private MonumentPrefabConfig GetConfigForPrefab(GameObject prefab)
    {
        foreach (var config in MonumentConfigs)
        {
            if (config.Prefab == prefab)
            {
                return config;
            }
        }
        return null;
    }

    /// <summary>
    /// 检查位置是否适合生成
    /// </summary>
    protected override bool IsValidSpawnPosition(Vector2Int worldPos, IChunkData chunkData)
    {
        // 检查地形类型是否在允许列表中
        TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
        return AllowedTerrainTypes != null && AllowedTerrainTypes.Contains(terrain);
    }
}
