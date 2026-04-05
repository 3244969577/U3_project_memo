using UnityEngine;

/// <summary>
/// 基础地形类型枚举
/// </summary>
public enum TerrainType
{
    Water, 
    Grass, 
    Mountain, 
    Rock, 
    Sand, 
    Default // 兜底类型
}

/// <summary>
/// 精细结构类型枚举
/// </summary>
public enum FineStructureType
{
    House, 
    Wall, 
    Stream, 
    Default
}

/// <summary>
/// 装饰类型枚举
/// </summary>
public enum DecorationType
{
    Boat, 
    Flower, 
    Chest, 
    Default
}

/// <summary>
/// 区块状态枚举
/// </summary>
public enum ChunkState
{
    Unloaded,    // 未加载（无数据、无Tilemap）
    Generating,  // 生成中（避免重复请求）
    Loaded,      // 已加载（数据+Tilemap就绪）
    Unloading    // 卸载中（清理Tilemap）
}
