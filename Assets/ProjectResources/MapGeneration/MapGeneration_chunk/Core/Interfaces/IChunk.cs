using UnityEngine;
using UnityEngine.Tilemaps;
using System;

/// <summary>
/// 区块核心接口（定义区块的基础行为）
/// </summary>
public interface IChunk : IDisposable
{
    /// <summary>
    /// 区块坐标
    /// </summary>
    Vector2Int ChunkCoord { get; }
    
    /// <summary>
    /// 区块状态
    /// </summary>
    ChunkState State { get; set; }
    
    /// <summary>
    /// 区块数据引用
    /// </summary>
    IChunkData ChunkData { get; set; }
    
    /// <summary>
    /// 基础地形Tilemap（可通行）
    /// </summary>
    Tilemap BaseTerrainTilemap { get; }
    
    
    
    /// <summary>
    /// 精细结构Tilemap
    /// </summary>
    Tilemap FineStructureTilemap { get; }
    
    /// <summary>
    /// 装饰Tilemap
    /// </summary>
    Tilemap DecorationTilemap { get; }

    /// <summary>
    /// 初始化区块（创建Tilemap等资源）
    /// </summary>
    /// <param name="parent">父Transform</param>
    void Initialize(Transform parent);
    
    /// <summary>
    /// 清理区块Tilemap（保留数据）
    /// </summary>
    void ClearTilemaps();
}
