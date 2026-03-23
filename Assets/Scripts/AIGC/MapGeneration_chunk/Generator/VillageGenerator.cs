using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 村庄生成器
/// 负责生成建筑群和NPC
/// </summary>
[System.Serializable]
public class VillageGenerator : GeneratorBase
{
    [Header("村庄配置")]
    public float GenerationProbability = 0.1f; // 村庄生成概率
    public Vector2Int VillageSize = new Vector2Int(5, 5); // 村庄大小（宽和高）
    public int MaxHouses = 5; // 最大房屋数量
    public int MaxNPCs = 3; // 最大NPC数量
    public float HouseProbability = 0.7f; // 房屋生成概率
    public float NPCProbability = 0.5f; // NPC生成概率
    public int MaxRetries = 5; // 最大重试次数
    public float MinHouseDistance = 2f; // 房屋最小间距

    [Header("预制体配置")]
    public List<GameObject> HousePrefabs; // 房屋预制体列表
    public List<GameObject> NPCPrefabs; // NPC预制体列表

    public GameObject ParentObject; // 父对象

    // 生成器专用父对象
    protected GameObject _generatorParent;

    /// <summary>
    /// 初始化生成器
    /// </summary>
    public override void Initialize()
    {
        // 使用手动指定的父对象
        _generatorParent = ParentObject;
    }

    /// <summary>
    /// 生成村庄
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
            
            // 首先根据概率确定是否允许生成
            if (UnityEngine.Random.value > GenerationProbability)
            {
                Debug.Log($"[VillageGenerator] 区块 {chunkCoord} 生成概率未命中，跳过生成");
                onComplete?.Invoke();
                return;
            }
            
            // 尝试生成村庄，最多重试 MaxRetries 次
            int retryCount = 0;
            while (retryCount < MaxRetries)
            {
                // 存储已生成位置
                List<Vector2Int> spawnedPositions = new List<Vector2Int>();
                
                // 随机选取一个点作为基准点
                Vector2Int basePoint = GetRandomPointInBounds(chunkBounds);
                
                // 判断该点是否适合作为基准点
                if (IsSuitableBasePoint(basePoint, chunkData))
                {
                    // 计算村庄边界
                    BoundsInt villageBounds = GetVillageBounds(basePoint);
                    
                    // 生成房屋
                    int housesSpawned = SpawnHouses(villageBounds, chunkData, spawnedPositions);
                    
                    // 生成NPC
                    int npcsSpawned = SpawnNPCs(villageBounds, chunkData, spawnedPositions);
                    
                    Debug.Log($"[VillageGenerator] 区块 {chunkCoord} 生成了 {housesSpawned} 个房屋和 {npcsSpawned} 个NPC");
                    onComplete?.Invoke();
                    return;
                }
                
                retryCount++;
                Debug.Log($"[VillageGenerator] 区块 {chunkCoord} 基准点 {basePoint} 不适合，重试 {retryCount}/{MaxRetries}");
            }
            
            // 所有重试都失败
            Debug.Log($"[VillageGenerator] 区块 {chunkCoord} 无法找到合适的基准点，跳过生成");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[VillageGenerator] 生成失败：{e.Message}");
        }
        finally
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 在边界内随机获取一个点
    /// </summary>
    /// <param name="bounds">边界</param>
    /// <returns>随机点</returns>
    private Vector2Int GetRandomPointInBounds(BoundsInt bounds)
    {
        int x = UnityEngine.Random.Range(bounds.xMin, bounds.xMax);
        int y = UnityEngine.Random.Range(bounds.yMin, bounds.yMax);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 判断点是否适合作为基准点
    /// </summary>
    /// <param name="basePoint">基准点</param>
    /// <param name="chunkData">区块数据</param>
    /// <returns>是否适合</returns>
    private bool IsSuitableBasePoint(Vector2Int basePoint, IChunkData chunkData)
    {
        // 计算村庄边界
        BoundsInt villageBounds = GetVillageBounds(basePoint);
        
        // 存储已尝试的位置
        List<Vector2Int> triedPositions = new List<Vector2Int>();
        int successfulAttempts = 0;
        int maxAttempts = MaxHouses * 2; // 尝试次数为最大房屋数的2倍
        
        // 尝试生成房屋，判断是否适合
        for (int i = 0; i < maxAttempts; i++)
        {
            // 在村庄区域内随机选择一个点
            Vector2Int randomPos = GetRandomPointInBounds(villageBounds);
            
            // 检查是否已尝试过
            if (triedPositions.Contains(randomPos)) continue;
            triedPositions.Add(randomPos);
            
            // 检查地形类型是否适合
            TerrainType terrain = chunkData.GetData<TerrainType>(randomPos);
            if (terrain != TerrainType.Grass && terrain != TerrainType.Sand)
            {
                continue;
            }
            
            // 检查与其他房屋的距离
            bool tooClose = false;
            foreach (var pos in triedPositions)
            {
                if (Vector2.Distance(randomPos, pos) < MinHouseDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;
            
            // 尝试成功
            successfulAttempts++;
            if (successfulAttempts >= MaxHouses / 2) // 至少能生成一半的房屋
            {
                return true;
            }
        }
        
        // 尝试失败
        return false;
    }

    /// <summary>
    /// 获取村庄边界
    /// </summary>
    /// <param name="basePoint">基准点</param>
    /// <returns>村庄边界</returns>
    private BoundsInt GetVillageBounds(Vector2Int basePoint)
    {
        int halfWidth = VillageSize.x / 2;
        int halfHeight = VillageSize.y / 2;
        
        return new BoundsInt(
            basePoint.x - halfWidth,
            basePoint.y - halfHeight,
            0,
            VillageSize.x,
            VillageSize.y,
            1
        );
    }

    /// <summary>
    /// 生成房屋
    /// </summary>
    /// <param name="bounds">村庄边界</param>
    /// <param name="chunkData">区块数据</param>
    /// <param name="spawnedPositions">已生成位置</param>
    /// <returns>生成的房屋数量</returns>
    private int SpawnHouses(BoundsInt bounds, IChunkData chunkData, List<Vector2Int> spawnedPositions)
    {
        if (HousePrefabs == null || HousePrefabs.Count == 0)
        {
            Debug.LogWarning("[VillageGenerator] 房屋预制体列表为空");
            return 0;
        }
        
        int spawned = 0;
        
        // 遍历村庄区域
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (spawned >= MaxHouses) break;
                
                Vector2Int worldPos = new Vector2Int(x, y);
                
                // 检查地形类型是否适合
                TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
                if (terrain != TerrainType.Grass && terrain != TerrainType.Sand)
                {
                    continue;
                }
                
                // 检查是否已生成
                if (spawnedPositions.Contains(worldPos)) continue;
                
                // 检查与其他房屋的距离
                bool tooClose = false;
                foreach (var pos in spawnedPositions)
                {
                    if (Vector2.Distance(worldPos, pos) < MinHouseDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) continue;
                
                // 按概率生成
                if (UnityEngine.Random.value > HouseProbability) continue;
                
                // 随机选择房屋预制体
                GameObject prefab = HousePrefabs[UnityEngine.Random.Range(0, HousePrefabs.Count)];
                if (prefab != null)
                {
                    Vector3 spawnPos = new Vector3(worldPos.x + 0.5f, worldPos.y + 0.5f, 0);
                    GameObject instance = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
                    instance.name = $"House_{worldPos.x}_{worldPos.y}";
                    
                    // 设置父对象
                    if (_generatorParent != null)
                    {
                        instance.transform.SetParent(_generatorParent.transform);
                    }
                    
                    // 标记位置
                    spawnedPositions.Add(worldPos);
                    spawned++;
                }
            }
        }
        
        return spawned;
    }

    /// <summary>
    /// 生成NPC
    /// </summary>
    /// <param name="bounds">村庄边界</param>
    /// <param name="chunkData">区块数据</param>
    /// <param name="spawnedPositions">已生成位置（包含房屋位置）</param>
    /// <returns>生成的NPC数量</returns>
    private int SpawnNPCs(BoundsInt bounds, IChunkData chunkData, List<Vector2Int> spawnedPositions)
    {
        if (NPCPrefabs == null || NPCPrefabs.Count == 0)
        {
            Debug.LogWarning("[VillageGenerator] NPC预制体列表为空");
            return 0;
        }
        
        int spawned = 0;
        
        // 遍历村庄区域
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                if (spawned >= MaxNPCs) break;
                
                Vector2Int worldPos = new Vector2Int(x, y);
                
                // 检查地形类型是否适合
                TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
                if (terrain != TerrainType.Grass && terrain != TerrainType.Sand)
                {
                    continue;
                }
                
                // 检查是否已生成（避免与房屋重叠）
                if (spawnedPositions.Contains(worldPos)) continue;
                
                // 按概率生成
                if (UnityEngine.Random.value > NPCProbability) continue;
                
                // 随机选择NPC预制体
                GameObject prefab = NPCPrefabs[UnityEngine.Random.Range(0, NPCPrefabs.Count)];
                if (prefab != null)
                {
                    Vector3 spawnPos = new Vector3(worldPos.x + 0.5f, worldPos.y + 0.5f, 0);
                    GameObject instance = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
                    instance.name = $"NPC_{worldPos.x}_{worldPos.y}";
                    
                    // 设置父对象
                    if (_generatorParent != null)
                    {
                        instance.transform.SetParent(_generatorParent.transform);
                    }
                    
                    // 标记位置
                    spawnedPositions.Add(worldPos);
                    spawned++;
                }
            }
        }
        
        return spawned;
    }

    /// <summary>
    /// 清理生成器
    /// </summary>
    public override void Cleanup()
    {
        // 清理逻辑
    }

    /// <summary>
    /// 检查两个 BoundsInt 是否相交
    /// </summary>
    /// <param name="bounds1">第一个边界</param>
    /// <param name="bounds2">第二个边界</param>
    /// <returns>是否相交</returns>
    private bool DoBoundsIntersect(BoundsInt bounds1, BoundsInt bounds2)
    {
        return !(bounds2.xMax < bounds1.xMin || bounds2.xMin > bounds1.xMax ||
                 bounds2.yMax < bounds1.yMin || bounds2.yMin > bounds1.yMax);
    }
}