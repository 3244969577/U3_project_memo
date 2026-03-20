using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 区块数据具体实现（线程安全）
/// </summary>
public class ChunkData : IChunkData
{
    // 数据存储字典（按类型分类）
    private readonly Dictionary<Type, Dictionary<Vector2Int, Enum>> _dataDict = new();
    
    // 基础属性
    public Vector2Int ChunkCoord { get; private set; }
    public bool IsGenerated { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public ChunkData(Vector2Int chunkCoord)
    {
        ChunkCoord = chunkCoord;
        IsGenerated = false;
        
        // 初始化各类型数据字典
        _dataDict[typeof(TerrainType)] = new Dictionary<Vector2Int, Enum>();
        _dataDict[typeof(FineStructureType)] = new Dictionary<Vector2Int, Enum>();
        _dataDict[typeof(DecorationType)] = new Dictionary<Vector2Int, Enum>();
    }

    /// <summary>
    /// 设置指定世界坐标的任意类型数据
    /// </summary>
    public void SetData<T>(Vector2Int worldPos, T data) where T : Enum
    {
        if (!_dataDict.ContainsKey(typeof(T)))
        {
            Debug.LogError($"[ChunkData] 不支持的数据类型：{typeof(T).Name}");
            return;
        }

        lock (_dataDict) // 线程安全锁
        {
            _dataDict[typeof(T)][worldPos] = data;
        }
    }

    /// <summary>
    /// 获取指定世界坐标的任意类型数据
    /// </summary>
    public T GetData<T>(Vector2Int worldPos) where T : Enum
    {
        if (!_dataDict.ContainsKey(typeof(T)))
        {
            Debug.LogError($"[ChunkData] 不支持的数据类型：{typeof(T).Name}");
            return default;
        }

        lock (_dataDict)
        {
            if (_dataDict[typeof(T)].TryGetValue(worldPos, out Enum value))
            {
                return (T)value;
            }
            
            // 返回该类型的默认值（兜底）
            return (T)Enum.Parse(typeof(T), nameof(TerrainType.Default));
        }
    }

    /// <summary>
    /// 获取该区块内指定类型的所有数据
    /// </summary>
    public Dictionary<Vector2Int, T> GetAllData<T>() where T : Enum
    {
        if (!_dataDict.ContainsKey(typeof(T)))
        {
            Debug.LogError($"[ChunkData] 不支持的数据类型：{typeof(T).Name}");
            return new Dictionary<Vector2Int, T>();
        }

        lock (_dataDict)
        {
            var result = new Dictionary<Vector2Int, T>();
            foreach (var kvp in _dataDict[typeof(T)])
            {
                result[kvp.Key] = (T)kvp.Value;
            }
            return result;
        }
    }

    /// <summary>
    /// 清空区块所有数据
    /// </summary>
    public void Clear()
    {
        lock (_dataDict)
        {
            foreach (var dict in _dataDict.Values)
            {
                dict.Clear();
            }
            IsGenerated = false;
        }
    }

    /// <summary>
    /// 检查世界坐标是否属于该区块
    /// </summary>
    public bool ContainsWorldPos(Vector2Int worldPos, bool includeBuffer = true)
    {
        BoundsInt bounds = ChunkCoordinateUtility.GetChunkWorldBounds(ChunkCoord, includeBuffer);
        return bounds.Contains(new Vector3Int(worldPos.x, worldPos.y, MapConstants.TilemapZLayer));
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Clear();
        _dataDict.Clear();
    }
}
