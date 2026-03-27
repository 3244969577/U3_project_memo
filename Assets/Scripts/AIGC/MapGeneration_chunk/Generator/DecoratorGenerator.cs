using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;

/// <summary>
/// 装饰生成器
/// 根据ground tilemap的瓦片类型在装饰层生成装饰性瓦片
/// </summary>
public class DecoratorGenerator : GeneratorBase
{
    [Header("配置")]
    public Tilemap decorTilemap; // 装饰瓦片地图
    public int maxDecorCount = 10; // 每个区块最大装饰数量
    public float decorProbability = 0.3f; // 生成装饰的概率

    [Header("装饰配置")]
    public List<DecorConfig> decorConfigs = new List<DecorConfig>(); // 装饰配置列表

    /// <summary>
    /// 装饰配置
    /// </summary>
    [System.Serializable]
    public class DecorConfig
    {
        public TerrainType terrainType; // 适用的地形类型
        public TileBase decorTile; // 装饰瓦片
        public float weight = 1f; // 生成权重
    }

    /// <summary>
    /// 初始化生成器
    /// </summary>
    public override void Initialize()
    {
        // 初始化逻辑
    }

    /// <summary>
    /// 生成装饰
    /// </summary>
    /// <param name="chunkData">区块数据</param>
    /// <param name="onComplete">完成回调</param>
    public override void Generate(IChunkData chunkData, Action onComplete = null)
    {
        if (decorTilemap == null)
        {
            Debug.LogError("Decor tilemap is not assigned!");
            onComplete?.Invoke();
            return;
        }

        // 获取区块边界
        BoundsInt chunkBounds = ChunkCoordinateUtility.GetChunkWorldBounds(chunkData.ChunkCoord, false);

        // 生成装饰
        int generatedCount = 0;
        int maxAttempts = maxDecorCount * 3; // 最大尝试次数，避免无限循环
        int attempts = 0;

        while (generatedCount < maxDecorCount && attempts < maxAttempts)
        {
            // 随机生成位置
            int randomX = UnityEngine.Random.Range(chunkBounds.xMin, chunkBounds.xMax);
            int randomY = UnityEngine.Random.Range(chunkBounds.yMin, chunkBounds.yMax);
            Vector3Int position = new Vector3Int(randomX, randomY, 0);

            // 检查位置是否合法
            if (IsValidPosition(position, chunkData))
            {
                // 检查概率
                if (UnityEngine.Random.value > decorProbability)
                {
                    attempts++;
                    continue;
                }

                // 获取地形类型
                Vector2Int pos2D = new Vector2Int(position.x, position.y);
                TerrainType terrainType = chunkData.GetData<TerrainType>(pos2D);
                if (terrainType == TerrainType.Default)
                {
                    attempts++;
                    continue;
                }

                // 筛选适合当前地形的装饰配置
                List<DecorConfig> validConfigs = GetValidDecorConfigs(terrainType);
                if (validConfigs.Count > 0)
                {
                    // 根据权重选择装饰配置
                    DecorConfig selectedConfig = SelectDecorConfigByWeight(validConfigs);
                    if (selectedConfig != null && selectedConfig.decorTile != null)
                    {
                        // 在装饰层绘制装饰瓦片
                        decorTilemap.SetTile(position, selectedConfig.decorTile);
                        generatedCount++;
                    }
                }
            }

            attempts++;
        }

        // 调用完成回调
        onComplete?.Invoke();
    }

    /// <summary>
    /// 获取适合指定地形的装饰配置
    /// </summary>
    /// <param name="terrainType">地形类型</param>
    /// <returns>适合的装饰配置列表</returns>
    private List<DecorConfig> GetValidDecorConfigs(TerrainType terrainType)
    {
        List<DecorConfig> validConfigs = new List<DecorConfig>();
        foreach (var config in decorConfigs)
        {
            if (config.terrainType == terrainType && config.decorTile != null)
            {
                validConfigs.Add(config);
            }
        }
        return validConfigs;
    }

    /// <summary>
    /// 根据权重选择装饰配置
    /// </summary>
    /// <param name="configs">装饰配置列表</param>
    /// <returns>选中的装饰配置</returns>
    private DecorConfig SelectDecorConfigByWeight(List<DecorConfig> configs)
    {
        float totalWeight = 0;
        foreach (var config in configs)
        {
            totalWeight += config.weight;
        }
        
        if (totalWeight <= 0)
        {
            return configs.Count > 0 ? configs[0] : null;
        }
        
        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0;
        
        foreach (var config in configs)
        {
            currentWeight += config.weight;
            if (randomValue <= currentWeight)
            {
                return config;
            }
        }
        
        // 兜底返回第一个配置
        return configs.Count > 0 ? configs[0] : null;
    }

    /// <summary>
    /// 检查位置是否合法
    /// </summary>
    /// <param name="position">要检查的位置</param>
    /// <param name="chunkData">区块数据</param>
    /// <returns>位置是否合法</returns>
    private bool IsValidPosition(Vector3Int position, IChunkData chunkData)
    {
        // 检查是否有地形数据且没有装饰瓦片
        Vector2Int pos2D = new Vector2Int(position.x, position.y);
        TerrainType terrainType = chunkData.GetData<TerrainType>(pos2D);
        TileBase decorTile = decorTilemap.GetTile(position);
        return terrainType != TerrainType.Default && decorTile == null;
    }

    /// <summary>
    /// 清理生成器
    /// </summary>
    public override void Cleanup()
    {
        // 清理逻辑
    }




}
