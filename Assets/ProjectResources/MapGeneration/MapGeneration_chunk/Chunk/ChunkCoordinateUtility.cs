using UnityEngine;

/// <summary>
/// 区块坐标转换工具类（静态工具）
/// </summary>
public static class ChunkCoordinateUtility
{
    /// <summary>
    /// 世界瓦片坐标转换为区块坐标
    /// </summary>
    public static Vector2Int WorldToChunkCoord(Vector2Int worldPos)
    {
        int chunkX = Mathf.FloorToInt((float)worldPos.x / MapConstants.ChunkSize);
        int chunkY = Mathf.FloorToInt((float)worldPos.y / MapConstants.ChunkSize);
        return new Vector2Int(chunkX, chunkY);
    }

    /// <summary>
    /// 世界瓦片坐标转换为区块内局部坐标
    /// </summary>
    public static Vector2Int WorldToLocalCoord(Vector2Int worldPos)
    {
        int localX = worldPos.x % MapConstants.ChunkSize;
        int localY = worldPos.y % MapConstants.ChunkSize;
        
        // 处理负数取模问题
        if (localX < 0) localX += MapConstants.ChunkSize;
        if (localY < 0) localY += MapConstants.ChunkSize;
        
        return new Vector2Int(localX, localY);
    }

    /// <summary>
    /// 区块坐标+局部坐标转换为世界瓦片坐标
    /// </summary>
    public static Vector2Int ChunkToWorldCoord(Vector2Int chunkCoord, Vector2Int localCoord)
    {
        int worldX = chunkCoord.x * MapConstants.ChunkSize + localCoord.x;
        int worldY = chunkCoord.y * MapConstants.ChunkSize + localCoord.y;
        return new Vector2Int(worldX, worldY);
    }

    /// <summary>
    /// 获取区块的世界坐标范围（BoundsInt）
    /// </summary>
    public static BoundsInt GetChunkWorldBounds(Vector2Int chunkCoord, bool includeBuffer = true)
    {
        int buffer = includeBuffer ? MapConstants.ChunkBufferSize : 0;
        int startX = chunkCoord.x * MapConstants.ChunkSize - buffer;
        int startY = chunkCoord.y * MapConstants.ChunkSize - buffer;
        int sizeX = MapConstants.ChunkSize + buffer * 2;
        int sizeY = MapConstants.ChunkSize + buffer * 2;
        
        return new BoundsInt(startX, startY, MapConstants.TilemapZLayer, sizeX, sizeY, 1);
    }

    /// <summary>
    /// 生成区块专属种子（保证可复现）
    /// </summary>
    public static int GetChunkSeed(Vector2Int chunkCoord)
    {
        // 使用大质数避免种子碰撞
        return MapConstants.GlobalSeed + chunkCoord.x * 1000003 + chunkCoord.y * 10000033;
    }
}
