using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Tilemap操作工具类
/// </summary>
public static class TilemapUtility
{
    /// <summary>
    /// 安全设置Tilemap瓦片（包含空值检查）
    /// </summary>
    public static void SetTileSafe(Tilemap tilemap, Vector3Int pos, TileBase tile)
    {
        if (tilemap == null)
        {
            Debug.LogError("[TilemapUtility] Tilemap引用为空，无法设置瓦片");
            return;
        }
        
        if (tile == null)
        {
            Debug.LogWarning($"[TilemapUtility] 瓦片为空，跳过位置 {pos} 的绘制");
            return;
        }
        
        tilemap.SetTile(pos, tile);
    }

    /// <summary>
    /// 清空Tilemap所有瓦片
    /// </summary>
    public static void ClearTilemapSafe(Tilemap tilemap)
    {
        if (tilemap == null) return;
        tilemap.ClearAllTiles();
    }

    /// <summary>
    /// 创建带排序层级的Tilemap
    /// </summary>
    public static Tilemap CreateTilemap(string name, Transform parent, int sortingOrder)
    {
        GameObject tilemapObj = new GameObject(name);
        tilemapObj.transform.SetParent(parent);
        
        Tilemap tilemap = tilemapObj.AddComponent<Tilemap>();
        TilemapRenderer renderer = tilemapObj.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = sortingOrder;
        
        return tilemap;
    }

    /// <summary>
    /// 为Tilemap添加碰撞体（仅基础地形层使用）
    /// </summary>
    public static void AddCollisionToTilemap(Tilemap tilemap)
    {
        if (tilemap == null) return;
        
        // 添加TilemapCollider2D
        if (!tilemap.gameObject.TryGetComponent<TilemapCollider2D>(out TilemapCollider2D collider))
        {
            collider = tilemap.gameObject.AddComponent<TilemapCollider2D>();
        }
        
        // 启用复合碰撞箱以提高性能
        collider.usedByComposite = true;
        
        // 添加CompositeCollider2D
        if (!tilemap.gameObject.TryGetComponent<CompositeCollider2D>(out CompositeCollider2D compositeCollider))
        {
            compositeCollider = tilemap.gameObject.AddComponent<CompositeCollider2D>();
        }
        
        // 配置CompositeCollider2D
        compositeCollider.geometryType = CompositeCollider2D.GeometryType.Polygons;
        compositeCollider.generationType = CompositeCollider2D.GenerationType.Synchronous;
        
        // 启用碰撞器
        collider.enabled = true;
        compositeCollider.enabled = true;
    }
}
