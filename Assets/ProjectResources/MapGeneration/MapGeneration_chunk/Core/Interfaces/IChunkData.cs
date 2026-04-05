using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 区块数据核心接口（定义区块数据的存取规范）
/// </summary>
public interface IChunkData : IDisposable
{
    /// <summary>
    /// 区块坐标
    /// </summary>
    Vector2Int ChunkCoord { get; }
    
    /// <summary>
    /// 生成完成标记
    /// </summary>
    bool IsGenerated { get; set; }

    /// <summary>
    /// 设置指定世界坐标的任意类型数据
    /// </summary>
    /// <typeparam name="T">数据类型（TerrainType/FineStructureType/DecorationType）</typeparam>
    void SetData<T>(Vector2Int worldPos, T data) where T : Enum;
    
    /// <summary>
    /// 获取指定世界坐标的任意类型数据
    /// </summary>
    T GetData<T>(Vector2Int worldPos) where T : Enum;
    
    /// <summary>
    /// 获取该区块内指定类型的所有数据
    /// </summary>
    Dictionary<Vector2Int, T> GetAllData<T>() where T : Enum;
    
    /// <summary>
    /// 清空区块所有数据
    /// </summary>
    void Clear();
    
    /// <summary>
    /// 检查世界坐标是否属于该区块（可选包含缓冲区）
    /// </summary>
    bool ContainsWorldPos(Vector2Int worldPos, bool includeBuffer = true);
}
