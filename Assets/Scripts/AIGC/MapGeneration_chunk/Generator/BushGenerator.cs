using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 灌木丛配置项
/// </summary>
[System.Serializable]
public class BushPrefabConfig
{
    public GameObject Prefab;         // 灌木丛预制体
    public float Weight = 1f;         // 生成权重
    public float RotationVariation;   // 旋转变化范围
}

/// <summary>
/// 灌木丛生成器
/// 在草地地形上生成多种类型的灌木丛预制体
/// </summary>
[System.Serializable]
public class BushGenerator : PrefabGeneratorBase
{
    [Header("灌木丛配置")]
    public List<BushPrefabConfig> BushConfigs; // 灌木丛配置列表
    
    [Header("群生配置")]
    public float Radius = 2f; // 生成半径
    public float WithinRadiusProbability = 0.5f; // 范围内生成概率

    /// <summary>
    /// 初始化生成器
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        
        // 确保允许的地形类型包含草地
        if (AllowedTerrainTypes == null)
        {
            AllowedTerrainTypes = new List<TerrainType>();
        }
        
        if (!AllowedTerrainTypes.Contains(TerrainType.Grass))
        {
            AllowedTerrainTypes.Add(TerrainType.Grass);
        }
    }

    /// <summary>
    /// 生成预制体
    /// </summary>
    public override void Generate(IChunkData chunkData, System.Action onComplete = null)
    {
        if (!IsEnabled || chunkData == null)
        {
            onComplete?.Invoke();
            return;
        }

        try
        {
            Vector2Int chunkCoord = chunkData.ChunkCoord;
            BoundsInt chunkBounds = ChunkCoordinateUtility.GetChunkWorldBounds(chunkCoord, false); // 只处理核心区域
            
            // 设置区块种子
            int chunkSeed = ChunkCoordinateUtility.GetChunkSeed(chunkCoord);
            UnityEngine.Random.InitState(chunkSeed);
            
            // 存储已生成位置，用于检查间距
            List<Vector2Int> spawnedPositions = new List<Vector2Int>();
            
            // 遍历区块核心区域，寻找中心点
            for (int x = chunkBounds.xMin; x < chunkBounds.xMax; x++)
            {
                for (int y = chunkBounds.yMin; y < chunkBounds.yMax; y++)
                {
                    Vector2Int worldPos = new Vector2Int(x, y);
                    
                    // 检查地形类型是否允许
                    TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
                    if (AllowedTerrainTypes == null || !AllowedTerrainTypes.Contains(terrain))
                    {
                        continue;
                    }
                    
                    // 检查位置是否适合生成
                    if (!IsValidSpawnPosition(worldPos, chunkData)) continue;
                    
                    // 检查间距
                    bool tooClose = false;
                    foreach (var pos in spawnedPositions)
                    {
                        if (Vector2.Distance(worldPos, pos) < MinDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }
                    if (tooClose) continue;
                    
                    // 按概率选择中心点
                    if (UnityEngine.Random.value > SpawnProbability) continue;
                    
                    // 在中心点生成一个灌木丛
                    SpawnBushAtPosition(worldPos, chunkData, spawnedPositions);
                    
                    // 在半径范围内生成更多灌木丛
                    SpawnBushesInRadius(worldPos, chunkData, spawnedPositions);
                }
            }

            Debug.Log($"[BushGenerator] 区块 {chunkCoord} 生成了 {spawnedPositions.Count} 个灌木丛");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[BushGenerator] 生成失败：{e.Message}");
        }
        finally
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 在指定位置生成灌木丛
    /// </summary>
    private void SpawnBushAtPosition(Vector2Int worldPos, IChunkData chunkData, List<Vector2Int> spawnedPositions)
    {
        // 获取预制体并实例化
        GameObject prefab = GetPrefabToSpawn(worldPos, chunkData);
        if (prefab != null)
        {
            Vector3 spawnPos = new Vector3(worldPos.x + 0.5f, worldPos.y + 0.5f, 0);
            GameObject instance = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
            instance.name = $"{prefab.name}_{worldPos.x}_{worldPos.y}";
            
            // 设置父对象
            if (_generatorParent != null)
            {
                instance.transform.SetParent(_generatorParent.transform);
            }
            
            // 存储生成位置
            spawnedPositions.Add(worldPos);
        }
    }

    /// <summary>
    /// 在半径范围内生成灌木丛
    /// </summary>
    private void SpawnBushesInRadius(Vector2Int centerPos, IChunkData chunkData, List<Vector2Int> spawnedPositions)
    {
        int radiusInt = Mathf.CeilToInt(Radius);
        
        // 遍历半径范围内的所有位置
        for (int x = centerPos.x - radiusInt; x <= centerPos.x + radiusInt; x++)
        {
            for (int y = centerPos.y - radiusInt; y <= centerPos.y + radiusInt; y++)
            {
                Vector2Int worldPos = new Vector2Int(x, y);
                
                // 跳过中心点
                if (worldPos == centerPos) continue;
                
                // 检查是否在半径范围内
                if (Vector2.Distance(worldPos, centerPos) > Radius) continue;
                
                // 检查地形类型是否允许
                TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
                if (AllowedTerrainTypes == null || !AllowedTerrainTypes.Contains(terrain))
                {
                    continue;
                }
                
                // 检查位置是否适合生成
                if (!IsValidSpawnPosition(worldPos, chunkData)) continue;
                
                // 检查间距
                bool tooClose = false;
                foreach (var pos in spawnedPositions)
                {
                    if (Vector2.Distance(worldPos, pos) < MinDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) continue;
                
                // 按概率生成
                if (UnityEngine.Random.value > WithinRadiusProbability) continue;
                
                // 生成灌木丛
                SpawnBushAtPosition(worldPos, chunkData, spawnedPositions);
            }
        }
    }

    /// <summary>
    /// 获取要生成的预制体
    /// </summary>
    protected override GameObject GetPrefabToSpawn(Vector2Int worldPos, IChunkData chunkData)
    {
        if (BushConfigs == null || BushConfigs.Count == 0)
        {
            Debug.LogWarning("[BushGenerator] 预制体配置列表为空");
            return null;
        }
        
        // 根据权重选择预制体
        GameObject prefab = SelectPrefabByWeight();
        
        // 应用旋转变化
        if (prefab != null)
        {
            BushPrefabConfig config = GetConfigForPrefab(prefab);
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
    private GameObject SelectPrefabByWeight()
    {
        float totalWeight = 0;
        foreach (var config in BushConfigs)
        {
            totalWeight += config.Weight;
        }
        
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0;
        
        foreach (var config in BushConfigs)
        {
            currentWeight += config.Weight;
            if (randomValue <= currentWeight)
            {
                return config.Prefab;
            }
        }
        
        // 兜底返回第一个预制体
        return BushConfigs[0].Prefab;
    }

    /// <summary>
    /// 获取预制体对应的配置
    /// </summary>
    private BushPrefabConfig GetConfigForPrefab(GameObject prefab)
    {
        foreach (var config in BushConfigs)
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
        // 检查地形类型是否为草地
        TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
        return terrain == TerrainType.Grass;
    }
}