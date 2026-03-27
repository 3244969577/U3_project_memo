using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 预制体生成器基类
/// </summary>
[Serializable]
public abstract class PrefabGeneratorBase : GeneratorBase
{
    [Header("基础配置")]
    public float SpawnProbability = 0.1f;           // 生成概率
    public Vector2Int SpawnCountRange = new Vector2Int(1, 3);   // 每个区块生成数量范围
    public float MinDistance = 2f;                  // 预制体最小间距
    public List<TerrainType> AllowedTerrainTypes;   // 允许生成的地形类型
    public GameObject ParentObject;                 // 手动指定的父对象

    // 构造函数
    public PrefabGeneratorBase()
    {
        isEnabled = true;
    }

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
    /// 生成预制体
    /// </summary>
    public override void Generate(IChunkData chunkData, Action onComplete = null)
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
            
            // 计算生成数量
            int spawnCount = UnityEngine.Random.Range(SpawnCountRange.x, SpawnCountRange.y + 1);
            int spawned = 0;
            
            // 存储已生成位置，用于检查间距
            List<Vector2Int> spawnedPositions = new List<Vector2Int>();
            
            // 遍历区块核心区域
            for (int x = chunkBounds.xMin; x < chunkBounds.xMax; x++)
            {
                for (int y = chunkBounds.yMin; y < chunkBounds.yMax; y++)
                {
                    if (spawned >= spawnCount) break;
                    
                    Vector2Int worldPos = new Vector2Int(x, y);
                    
                    // 检查地形类型是否允许
                    TerrainType terrain = chunkData.GetData<TerrainType>(worldPos);
                    if (AllowedTerrainTypes == null || !AllowedTerrainTypes.Contains(terrain))
                    {
                        Debug.Log($"地形类型 {terrain} 不在允许列表中或允许列表未设置");
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
                    if (UnityEngine.Random.value > SpawnProbability) continue;
                    Debug.Log($"概率命中, 位置 {worldPos}");
                    
                    // 获取预制体并从对象池获取实例
                    GameObject prefab = GetPrefabToSpawn(worldPos, chunkData);
                    if (prefab != null)
                    {
                        Vector3 spawnPos = new Vector3(worldPos.x + 0.5f, worldPos.y + 0.5f, 0);
                        GameObject instance;
                        
                        // 使用对象池获取对象
                        if (ObjectPoolManager.Instance != null)
                        {
                            instance = ObjectPoolManager.Instance.GetObject(prefab, spawnPos, Quaternion.identity);
                        }
                        else
                        {
                            // 如果对象池不可用，使用传统实例化
                            instance = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
                        }
                        
                        instance.name = $"{prefab.name}_{worldPos.x}_{worldPos.y}";
                        
                        // 设置父对象为手动指定的父对象
                        if (_generatorParent != null)
                        {
                            instance.transform.SetParent(_generatorParent.transform);
                        }
                        else
                        {
                            Debug.LogWarning($"[{GetType().Name}] 未指定父对象，预制体将没有父对象");
                        }
                        
                        // 存储生成位置
                        spawnedPositions.Add(worldPos);
                        spawned++;
                    } else {
                        Debug.Log($"[{GetType().Name}] 在位置 {worldPos} 获取预制体失败");
                    }
                }
            }

            Debug.Log($"[{GetType().Name}] 区块 {chunkCoord} 生成了 {spawned} 个预制体");
        }
        catch (Exception e)
        {
            Debug.LogError($"[{GetType().Name}] 生成失败：{e.Message}");
        }
        finally
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 清理生成器
    /// </summary>
    public override void Cleanup()
    {
        // 清理逻辑
    }

    /// <summary>
    /// 获取要生成的预制体
    /// </summary>
    protected abstract GameObject GetPrefabToSpawn(Vector2Int worldPos, IChunkData chunkData);
    
    /// <summary>
    /// 检查位置是否适合生成
    /// </summary>
    protected abstract bool IsValidSpawnPosition(Vector2Int worldPos, IChunkData chunkData);
}
