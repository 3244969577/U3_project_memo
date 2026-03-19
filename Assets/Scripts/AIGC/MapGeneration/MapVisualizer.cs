
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapVisualizer : MonoBehaviour
{
    [Header("Tilemap 引用（按层级赋值）")]
    public Tilemap baseTerrainTilemap;    // 基础地形层
    public Tilemap fineStructureTilemap;  // 精细结构层
    public Tilemap decorationTilemap;     // 装饰层

    
    [Header("基础地形瓦片配置")]
    public RuleTile waterTile;
    public RuleTile grassTile;
    public RuleTile mountainTile;
    public RuleTile rockTile;
    public RuleTile sandTile;
    // 基础地形默认瓦片（新增）
    public RuleTile baseTerrainDefaultTile; 

    [Header("精细结构瓦片配置")]
    public Tile houseTile;
    public Tile wallTile;
    public Tile streamTile;
    // 精细结构默认瓦片（新增）
    public Tile fineStructureDefaultTile; 

    [Header("装饰瓦片配置")]
    public Tile boatTile;
    public Tile flowerTile;
    public Tile chestTile;
    // 装饰层默认瓦片（新增）
    public Tile decorationDefaultTile; 


    // 坐标转换工具方法（统一处理 2D→3D 转换）
    private Vector3Int ConvertToTilemapPos(Vector2Int pos2D, int zLayer)
    {
        // 根据 Tilemap 配置调整 Y 轴方向（若 Tilemap 翻转则取反）
        return new Vector3Int(pos2D.x, pos2D.y, zLayer);
    }

    // 一键绘制所有层
    public void DrawAllLayers(MapGeneratorData data)
    {
        // 先清空所有 Tilemap
        ClearAllTilemaps();
        
        // 绘制基础地形层
        DrawBaseTerrainLayer(data);
        // 绘制精细结构层
        DrawFineStructureLayer(data);
        // 绘制装饰层
        DrawDecorationLayer(data);
        
        Debug.Log("所有层级绘制完成！");
    }

    // 封装：设置基础地形瓦片（带默认兜底）
    private void SetBaseTerrainTile(Vector3Int tilemapPos, TerrainType terrainType)
    {
        Debug.Log($"设置基础地形瓦片：{terrainType} 在位置 {tilemapPos}");
        RuleTile targetTile = baseTerrainDefaultTile; // 默认瓦片
        string typeName = terrainType.ToString();

        // 匹配对应瓦片
        switch (terrainType)
        {
            case TerrainType.Water:
                targetTile = waterTile;
                break;
            case TerrainType.Grass:
                targetTile = grassTile;
                break;
            case TerrainType.Mountain:
                targetTile = mountainTile;
                break;
            case TerrainType.Rock:
                targetTile = rockTile;
                break;
            case TerrainType.Sand:
                targetTile = sandTile;
                break;
            default:
                // 未匹配到类型时，打印警告日志
                Debug.LogWarning($"[MapVisualizer] 未找到地形类型 {typeName} 对应的瓦片，使用默认瓦片");
                break;
        }

        // 绘制瓦片（兼容RuleTile和普通Tile，RuleTile可直接赋值给Tilemap）
        baseTerrainTilemap.SetTile(tilemapPos, targetTile);
    }

    // 封装：设置精细结构瓦片（带默认兜底）
    private void SetFineStructureTile(Vector3Int tilemapPos, FineStructureType structureType)
    {
        Tile targetTile = fineStructureDefaultTile;
        string typeName = structureType.ToString();

        switch (structureType)
        {
            case FineStructureType.House:
                targetTile = houseTile;
                break;
            case FineStructureType.Wall:
                targetTile = wallTile;
                break;
            case FineStructureType.Stream:
                targetTile = streamTile;
                break;
            default:
                Debug.LogWarning($"[MapVisualizer] 未找到精细结构类型 {typeName} 对应的瓦片，使用默认瓦片");
                break;
        }

        fineStructureTilemap.SetTile(tilemapPos, targetTile);
    }

    
    // 封装：设置装饰瓦片（带默认兜底）
    private void SetDecorationTile(Vector3Int tilemapPos, DecorationType decorationType)
    {
        Tile targetTile = decorationDefaultTile;
        string typeName = decorationType.ToString();

        switch (decorationType)
        {
            case DecorationType.Boat:
                targetTile = boatTile;
                break;
            case DecorationType.Flower:
                targetTile = flowerTile;
                break;
            default:
                Debug.LogWarning($"[MapVisualizer] 未找到装饰类型 {typeName} 对应的瓦片，使用默认瓦片");
                break;
        }

        decorationTilemap.SetTile(tilemapPos, targetTile);
    }

    // 绘制基础地形层（简化版）
    private void DrawBaseTerrainLayer(MapGeneratorData data)
    {
        Debug.Log($"开始绘制基础地形层，地图大小：{data.mapWidth}x{data.mapHeight}");
        foreach (var kvp in data.baseTerrainData)
        {
            Vector2Int pos2D = kvp.Key;
            TerrainType type = kvp.Value;
            Vector3Int pos3D = ConvertToTilemapPos(pos2D, data.tilemapZLayer);

            // 调用封装方法，自动处理匹配和默认兜底
            SetBaseTerrainTile(pos3D, type);
        }
    }

    // 绘制精细结构层（简化版）
    private void DrawFineStructureLayer(MapGeneratorData data)
    {
        Debug.Log($"开始绘制精细结构层，地图大小：{data.mapWidth}x{data.mapHeight}");
        foreach (var kvp in data.fineStructureData)
        {
            Vector2Int pos2D = kvp.Key;
            FineStructureType type = kvp.Value;
            Vector3Int pos3D = ConvertToTilemapPos(pos2D, data.tilemapZLayer);

            SetFineStructureTile(pos3D, type);
        }
    }

    // 绘制装饰层（简化版）
    private void DrawDecorationLayer(MapGeneratorData data)
    {
        Debug.Log($"开始绘制装饰层，地图大小：{data.mapWidth}x{data.mapHeight}");
        foreach (var kvp in data.decorationData)
        {
            Vector2Int pos2D = kvp.Key;
            DecorationType type = kvp.Value;
            Vector3Int pos3D = ConvertToTilemapPos(pos2D, data.tilemapZLayer);

            SetDecorationTile(pos3D, type);
        }
    }

    // 清空所有 Tilemap
    public void ClearAllTilemaps()
    {
        baseTerrainTilemap.ClearAllTiles();
        fineStructureTilemap.ClearAllTiles();
        decorationTilemap.ClearAllTiles();
    }
}