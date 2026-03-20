using UnityEngine;
using UnityEngine.Tilemaps;
using System;

/// <summary>
/// 区块可视化器（单例）
/// </summary>
public class ChunkVisualizer : MonoBehaviour, IChunkVisualizer
{
    [Header("默认瓦片配置")]
    [SerializeField] private TileBase baseTerrainDefaultTile;
    [SerializeField] private TileBase fineStructureDefaultTile;
    [SerializeField] private TileBase decorationDefaultTile;
    
    // 接口属性实现
    public TileBase BaseTerrainDefaultTile { get => baseTerrainDefaultTile; set => baseTerrainDefaultTile = value; }
    public TileBase FineStructureDefaultTile { get => fineStructureDefaultTile; set => fineStructureDefaultTile = value; }
    public TileBase DecorationDefaultTile { get => decorationDefaultTile; set => decorationDefaultTile = value; }

    [Header("地形瓦片配置")]
    public TileBase WaterTile;
    public TileBase GrassTile;
    public TileBase MountainTile;
    public TileBase RockTile;
    public TileBase SandTile;

    [Header("结构瓦片配置")]
    public TileBase HouseTile;
    public TileBase WallTile;
    public TileBase StreamTile;

    [Header("装饰瓦片配置")]
    public TileBase BoatTile;
    public TileBase FlowerTile;
    public TileBase ChestTile;

    // 单例实例
    public static ChunkVisualizer Instance { get; private set; }

    #region 接口实现
    /// <summary>
    /// 绘制指定区块
    /// </summary>
    public void DrawChunk(IChunk chunk)
    {
        if (chunk == null || chunk.ChunkData == null || !chunk.ChunkData.IsGenerated)
        {
            Debug.LogWarning("[ChunkVisualizer] 区块数据无效，无法绘制");
            return;
        }

        DrawChunkData(chunk.ChunkData);
    }

    /// <summary>
    /// 绘制指定区块数据
    /// </summary>
    public void DrawChunkData(IChunkData chunkData)
    {
        if (chunkData == null || !chunkData.IsGenerated)
        {
            Debug.LogWarning("[ChunkVisualizer] 区块数据无效，无法绘制");
            return;
        }

        try
        {
            // 绘制基础地形
            DrawBaseTerrain(chunkData);
            // 绘制精细结构
            DrawFineStructure(chunkData);
            // 绘制装饰
            DrawDecoration(chunkData);
            
            Debug.Log($"[ChunkVisualizer] 区块 {chunkData.ChunkCoord} 绘制完成");
        }
        catch (Exception e)
        {
            Debug.LogError($"[ChunkVisualizer] 绘制失败：{e.Message}");
        }
    }

    /// <summary>
    /// 清理区块可视化内容
    /// </summary>
    public void ClearChunk(IChunk chunk)
    {
        if (chunk == null) return;
        chunk.ClearTilemaps();
    }

    /// <summary>
    /// 世界坐标转换为Tilemap坐标
    /// </summary>
    public Vector3Int WorldToTilemapPos(Vector2Int worldPos)
    {
        return new Vector3Int(worldPos.x, worldPos.y, MapConstants.TilemapZLayer);
    }
    #endregion

    #region 生命周期
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    #endregion

    #region 绘制逻辑
    /// <summary>
    /// 绘制基础地形
    /// </summary>
    private void DrawBaseTerrain(IChunkData chunkData)
    {
        if (TilemapManager.Instance.BaseTerrainTilemap == null || TilemapManager.Instance.ObstacleTilemap == null)
        {
            Debug.LogError("[ChunkVisualizer] BaseTerrainTilemap or ObstacleTilemap not assigned in TilemapManager");
            return;
        }
        
        var terrainData = chunkData.GetAllData<TerrainType>();
        BoundsInt coreBounds = ChunkCoordinateUtility.GetChunkWorldBounds(chunkData.ChunkCoord, false);

        foreach (var kvp in terrainData)
        {
            Vector2Int worldPos = kvp.Key;
            // 仅绘制核心区域（裁剪缓冲区）
            if (!coreBounds.Contains(new Vector3Int(worldPos.x, worldPos.y, 0))) continue;
            
            TerrainType type = kvp.Value;
            Vector3Int tilePos = WorldToTilemapPos(worldPos);
            TileBase tile = GetTerrainTile(type);
            
            // 根据地形类型选择Tilemap层
            Tilemap targetTilemap = IsPassableTerrain(type) ? 
                TilemapManager.Instance.BaseTerrainTilemap : 
                TilemapManager.Instance.ObstacleTilemap;
            
            TilemapUtility.SetTileSafe(targetTilemap, tilePos, tile ?? BaseTerrainDefaultTile);
        }
    }
    
    /// <summary>
    /// 判断地形是否可通行
    /// </summary>
    private bool IsPassableTerrain(TerrainType type)
    {
        // 可通行的地形类型
        return type == TerrainType.Grass || type == TerrainType.Sand || type == TerrainType.Water;
    }

    /// <summary>
    /// 绘制精细结构
    /// </summary>
    private void DrawFineStructure(IChunkData chunkData)
    {
        if (TilemapManager.Instance.FineStructureTilemap == null)
        {
            Debug.LogError("[ChunkVisualizer] FineStructureTilemap not assigned in TilemapManager");
            return;
        }
        
        var structureData = chunkData.GetAllData<FineStructureType>();
        BoundsInt coreBounds = ChunkCoordinateUtility.GetChunkWorldBounds(chunkData.ChunkCoord, false);

        foreach (var kvp in structureData)
        {
            Vector2Int worldPos = kvp.Key;
            if (!coreBounds.Contains(new Vector3Int(worldPos.x, worldPos.y, 0))) continue;
            
            FineStructureType type = kvp.Value;
            Vector3Int tilePos = WorldToTilemapPos(worldPos);
            TileBase tile = GetStructureTile(type);
            
            TilemapUtility.SetTileSafe(TilemapManager.Instance.FineStructureTilemap, tilePos, tile ?? FineStructureDefaultTile);
        }
    }

    /// <summary>
    /// 绘制装饰
    /// </summary>
    private void DrawDecoration(IChunkData chunkData)
    {
        if (TilemapManager.Instance.DecorationTilemap == null)
        {
            Debug.LogError("[ChunkVisualizer] DecorationTilemap not assigned in TilemapManager");
            return;
        }
        
        var decorationData = chunkData.GetAllData<DecorationType>();
        BoundsInt coreBounds = ChunkCoordinateUtility.GetChunkWorldBounds(chunkData.ChunkCoord, false);

        foreach (var kvp in decorationData)
        {
            Vector2Int worldPos = kvp.Key;
            if (!coreBounds.Contains(new Vector3Int(worldPos.x, worldPos.y, 0))) continue;
            
            DecorationType type = kvp.Value;
            Vector3Int tilePos = WorldToTilemapPos(worldPos);
            TileBase tile = GetDecorationTile(type);
            
            TilemapUtility.SetTileSafe(TilemapManager.Instance.DecorationTilemap, tilePos, tile ?? DecorationDefaultTile);
        }
    }
    #endregion

    #region 瓦片匹配逻辑
    /// <summary>
    /// 获取地形对应的瓦片
    /// </summary>
    private TileBase GetTerrainTile(TerrainType type)
    {
        return type switch
        {
            TerrainType.Water => WaterTile,
            TerrainType.Grass => GrassTile,
            TerrainType.Mountain => MountainTile,
            TerrainType.Rock => RockTile,
            TerrainType.Sand => SandTile,
            _ => BaseTerrainDefaultTile
        };
    }

    /// <summary>
    /// 获取结构对应的瓦片
    /// </summary>
    private TileBase GetStructureTile(FineStructureType type)
    {
        return type switch
        {
            FineStructureType.House => HouseTile,
            FineStructureType.Wall => WallTile,
            FineStructureType.Stream => StreamTile,
            _ => FineStructureDefaultTile
        };
    }

    /// <summary>
    /// 获取装饰对应的瓦片
    /// </summary>
    private TileBase GetDecorationTile(DecorationType type)
    {
        return type switch
        {
            DecorationType.Boat => BoatTile,
            DecorationType.Flower => FlowerTile,
            DecorationType.Chest => ChestTile,
            _ => DecorationDefaultTile
        };
    }
    #endregion
}
