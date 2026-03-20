using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 区块可视化接口（定义区块绘制的标准行为）
/// </summary>
public interface IChunkVisualizer
{
    /// <summary>
    /// 基础地形默认瓦片
    /// </summary>
    TileBase BaseTerrainDefaultTile { get; set; }
    
    /// <summary>
    /// 精细结构默认瓦片
    /// </summary>
    TileBase FineStructureDefaultTile { get; set; }
    
    /// <summary>
    /// 装饰默认瓦片
    /// </summary>
    TileBase DecorationDefaultTile { get; set; }

    /// <summary>
    /// 绘制指定区块
    /// </summary>
    void DrawChunk(IChunk chunk);
    
    /// <summary>
    /// 清理指定区块的可视化内容
    /// </summary>
    void ClearChunk(IChunk chunk);
    
    /// <summary>
    /// 世界坐标转换为Tilemap坐标
    /// </summary>
    Vector3Int WorldToTilemapPos(Vector2Int worldPos);
}
