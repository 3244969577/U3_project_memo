using System.Collections.Generic;
using UnityEngine;

// 地形类型枚举（基础地形）
public enum TerrainType { Water, Grass, Mountain, Rock, Sand }

// 精细结构类型枚举
public enum FineStructureType { House, Wall, Stream, Sign, Chest }

// 装饰类型枚举
public enum DecorationType { Flower, Boat, Stone, Tree }

// 全局数据容器（所有生成层共享，核心交互媒介）
[System.Serializable]
public class MapGeneratorData
{
    // 基础地形数据：坐标 → 地形类型
    public Dictionary<Vector2Int, TerrainType> baseTerrainData = new Dictionary<Vector2Int, TerrainType>();
    
    // 精细结构数据：坐标 → 精细结构类型
    public Dictionary<Vector2Int, FineStructureType> fineStructureData = new Dictionary<Vector2Int, FineStructureType>();
    
    // 装饰数据：坐标 → 装饰类型
    public Dictionary<Vector2Int, DecorationType> decorationData = new Dictionary<Vector2Int, DecorationType>();
    
    // 地图基础配置
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int seed; // 全局种子，保证所有层生成结果一致
    public int tilemapZLayer = 0; // 所有生成的瓦片地图Z轴位置（默认0）
    
    // 清空数据（重新生成时调用）
    public void Clear()
    {
        baseTerrainData.Clear();
        fineStructureData.Clear();
        decorationData.Clear();
    }
}