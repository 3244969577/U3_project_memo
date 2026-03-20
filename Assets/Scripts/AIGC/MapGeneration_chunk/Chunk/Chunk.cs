using UnityEngine;
using UnityEngine.Tilemaps;
using System;

/// <summary>
/// 区块具体实现
/// </summary>
public class Chunk : IChunk
{
    // 核心属性
    public Vector2Int ChunkCoord { get; private set; }
    public ChunkState State { get; set; }
    public IChunkData ChunkData { get; set; }
    
    // Tilemap引用（使用全局Tilemap）
    public Tilemap BaseTerrainTilemap { get { return TilemapManager.Instance?.BaseTerrainTilemap; } }
    public Tilemap ObstacleTilemap { get { return TilemapManager.Instance?.ObstacleTilemap; } }
    public Tilemap FineStructureTilemap { get { return TilemapManager.Instance?.FineStructureTilemap; } }
    public Tilemap DecorationTilemap { get { return TilemapManager.Instance?.DecorationTilemap; } }
    
    // 内部引用
    private Transform _chunkTransform;

    /// <summary>
    /// 构造函数
    /// </summary>
    public Chunk(Vector2Int chunkCoord)
    {
        ChunkCoord = chunkCoord;
        State = ChunkState.Unloaded;
    }

    /// <summary>
    /// 初始化区块
    /// </summary>
    public void Initialize(Transform parent)
    {
        // 创建区块根物体（用于组织）
        _chunkTransform = new GameObject($"Chunk_{ChunkCoord.x}_{ChunkCoord.y}").transform;
        _chunkTransform.SetParent(parent);
        
        State = ChunkState.Unloaded;
    }

    /// <summary>
    /// 清理区块Tilemap
    /// </summary>
    public void ClearTilemaps()
    {
        // 注意：这里不再清理全局Tilemap，因为其他区块也在使用
        // 如果需要清理整个地图，应该调用TilemapManager.Instance.ClearAllTilemaps()
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        // 不清理全局Tilemap
        
        // 销毁区块根物体
        if (_chunkTransform != null)
        {
            GameObject.Destroy(_chunkTransform.gameObject);
        }
        
        // 释放数据
        ChunkData?.Dispose();
        ChunkData = null;
        
        State = ChunkState.Unloaded;
    }
}
