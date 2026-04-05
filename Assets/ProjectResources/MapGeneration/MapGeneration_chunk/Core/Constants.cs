using UnityEngine;

/// <summary>
/// 全局地图常量配置
/// </summary>
public static class MapConstants
{
    public const int ChunkSize = 64;               // 区块基础大小（瓦片数）
    public const int ChunkBufferSize = 2;          // 区块缓冲区大小（用于平滑）
    public const int LoadRange = 3;                // 玩家周围加载区块范围
    public const int UnloadRange = 5;              // 玩家周围卸载区块范围
    public static int GlobalSeed = 12345;          // 全局种子（保证生成一致性）
    public const int TilemapZLayer = 0;            // Tilemap的Z轴层级
    public const float ChunkLoadCheckInterval = 0.5f; // 玩家位置检查间隔
}
