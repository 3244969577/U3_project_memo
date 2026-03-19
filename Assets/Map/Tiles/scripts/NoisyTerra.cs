// using UnityEngine;
// using UnityEngine.Tilemaps;

// // 封装地形类型的枚举（可根据需求扩展）
// public enum TerrainType
// {
//     Water,      // 水域
//     Grass,      // 草地
//     Mountain,   // 山脉
//     Rock,       // 岩石（扩展示例）
//     Sand        // 沙滩（扩展示例）
// }

// /// <summary>
// /// 柏林噪声2D瓦片地图生成器（完善版）
// /// 特性：枚举封装地形、可视化阈值调节、可扩展地形类型
// /// </summary>
// [ExecuteInEditMode] // 在编辑模式下也能实时预览调整效果
// public class AdvancedPerlinNoiseMap : MonoBehaviour
// {
//     [Header("Tilemap核心配置")]
//     public Tilemap terrainTilemap; // 目标瓦片地图
//     public int mapWidth = 100;     // 地图宽度（瓦片数）
//     public int mapHeight = 100;    // 地图高度（瓦片数）

//     [Header("柏林噪声参数")]
//     public float noiseScale = 0.02f; // 噪声缩放（越小地形越平滑）
//     public int seed = 12345;         // 随机种子（固定则地图不变）
//     public bool useMultiLayerNoise = true; // 是否启用多层噪声叠加

//     [Header("地形阈值配置（按从小到大排序）")]
//     [Tooltip("水域上限阈值（0~1）")]
//     [Range(0f, 1f)] public float waterThreshold = 0.3f;
    
//     [Tooltip("草地上限阈值（需大于水域阈值）")]
//     [Range(0f, 1f)] public float grassThreshold = 0.6f;
    
//     [Tooltip("山脉上限阈值（需大于草地阈值）")]
//     [Range(0f, 1f)] public float mountainThreshold = 0.85f;
    
//     [Tooltip("岩石上限阈值（需大于山脉阈值）")]
//     [Range(0f, 1f)] public float rockThreshold = 0.95f;

//     [Header("地形瓦片映射")]
//     public TileBase waterTile;    // 水域瓦片
//     public TileBase grassTile;    // 草地瓦片
//     public TileBase mountainTile; // 山脉瓦片
//     public TileBase rockTile;     // 岩石瓦片
//     public TileBase sandTile;     // 沙滩瓦片（默认作为兜底）

//     void Start()
//     {
//         GenerateMap();
//     }

//     /// <summary>
//     /// 生成噪声地图（核心方法）
//     /// </summary>
//     [ContextMenu("一键生成地图")] // 右键菜单快速生成
//     public void GenerateMap()
//     {
//         if (terrainTilemap == null)
//         {
//             Debug.LogError("请先为terrainTilemap赋值目标瓦片地图！");
//             return;
//         }

//         // 校验阈值合法性（避免顺序错乱）
//         ValidateThresholds();

//         // 清空原有瓦片
//         terrainTilemap.ClearAllTiles();

//         // 遍历每个瓦片坐标生成地形
//         for (int x = 0; x < mapWidth; x++)
//         {
//             for (int y = 0; y < mapHeight; y++)
//             {
//                 // 1. 采样柏林噪声值
//                 float noiseValue = SamplePerlinNoise(x, y);
                
//                 // 2. 根据噪声值获取地形类型
//                 TerrainType terrainType = GetTerrainTypeByNoiseValue(noiseValue);
                
//                 // 3. 根据地形类型获取对应瓦片
//                 TileBase targetTile = GetTileByTerrainType(terrainType);
                
//                 // 4. 绘制瓦片（居中显示地图）
//                 Vector3Int tilePos = new Vector3Int(
//                     x - mapWidth / 2,
//                     y - mapHeight / 2,
//                     0
//                 );
//                 terrainTilemap.SetTile(tilePos, targetTile);
//             }
//         }
        
//         Debug.Log($"地图生成完成！种子：{seed}，地形分布：水域<{waterThreshold} | 草地<{grassThreshold} | 山脉<{mountainThreshold} | 岩石<{rockThreshold}>");
//     }

//     /// <summary>
//     /// 采样柏林噪声值（支持单层/多层叠加）
//     /// </summary>
//     private float SamplePerlinNoise(int x, int y)
//     {
//         float sampleX = (x + seed) * noiseScale;
//         float sampleY = (y + seed) * noiseScale;

//         float noiseValue = 0;

//         if (useMultiLayerNoise)
//         {
//             // 多层噪声叠加（更自然的地形）
//             // 第一层：低频大尺度地形（权重最高）
//             noiseValue += Mathf.PerlinNoise(sampleX * 1f, sampleY * 1f) * 0.7f;
//             // 第二层：中频细节
//             noiseValue += Mathf.PerlinNoise(sampleX * 4f, sampleY * 4f) * 0.2f;
//             // 第三层：高频纹理
//             noiseValue += Mathf.PerlinNoise(sampleX * 8f, sampleY * 8f) * 0.1f;
//             // 归一化到0~1区间
//             noiseValue = Mathf.Clamp01(noiseValue);
//         }
//         else
//         {
//             // 单层噪声
//             noiseValue = Mathf.PerlinNoise(sampleX, sampleY);
//         }

//         return noiseValue;
//     }

//     /// <summary>
//     /// 根据噪声值匹配地形类型（核心逻辑，可扩展）
//     /// </summary>
//     private TerrainType GetTerrainTypeByNoiseValue(float noiseValue)
//     {
//         if (noiseValue <= waterThreshold)
//             return TerrainType.Water;
//         else if (noiseValue <= grassThreshold)
//             return TerrainType.Grass;
//         else if (noiseValue <= mountainThreshold)
//             return TerrainType.Mountain;
//         else if (noiseValue <= rockThreshold)
//             return TerrainType.Rock;
//         else
//             return TerrainType.Sand; // 兜底地形
//     }

//     /// <summary>
//     /// 根据地形类型获取对应瓦片（解耦地形类型与瓦片）
//     /// </summary>
//     private TileBase GetTileByTerrainType(TerrainType terrainType)
//     {
//         switch (terrainType)
//         {
//             case TerrainType.Water:
//                 return waterTile;
//             case TerrainType.Grass:
//                 return grassTile;
//             case TerrainType.Mountain:
//                 return mountainTile;
//             case TerrainType.Rock:
//                 return rockTile;
//             case TerrainType.Sand:
//             default:
//                 return sandTile;
//         }
//     }

//     /// <summary>
//     /// 校验地形阈值合法性（避免阈值顺序错乱）
//     /// </summary>
//     private void ValidateThresholds()
//     {
//         // 确保阈值按 水域 < 草地 < 山脉 < 岩石 的顺序排列
//         waterThreshold = Mathf.Clamp01(waterThreshold);
//         grassThreshold = Mathf.Max(waterThreshold + 0.01f, Mathf.Clamp01(grassThreshold));
//         mountainThreshold = Mathf.Max(grassThreshold + 0.01f, Mathf.Clamp01(mountainThreshold));
//         rockThreshold = Mathf.Max(mountainThreshold + 0.01f, Mathf.Clamp01(rockThreshold));
//     }

//     /// <summary>
//     /// 随机生成新种子并重新绘制地图（可绑定UI按钮）
//     /// </summary>
//     public void RegenerateMapWithRandomSeed()
//     {
//         seed = Random.Range(0, 99999);
//         GenerateMap();
//     }

//     // 编辑模式下修改参数时自动刷新地图
//     private void OnValidate()
//     {
//         if (terrainTilemap != null && Application.isEditor)
//         {
//             ValidateThresholds(); // 实时校验阈值
//             GenerateMap();        // 实时预览效果
//         }
//     }
// }

// // using UnityEngine;
// // using UnityEngine.Tilemaps;


// // public class PerlinNoiseMapGenerator : MonoBehaviour
// // {
// //     [Header("Tilemap配置")]
// //     public Tilemap terrainTilemap; // 拖拽你的Tilemap
// //     public TileBase waterTile;         // 水域瓦片 
// //     public TileBase grassTile;         // 草地瓦片
// //     public TileBase mountainTile;      // 山脉瓦片

// //     [Header("噪声参数")]
// //     public int mapWidth = 100;     // 地图宽度（瓦片数）
// //     public int mapHeight = 100;    // 地图高度（瓦片数）
// //     public float noiseScale = 0.02f; // 噪声缩放（越小地形越平滑）
// //     public int seed = 12345;       // 随机种子（固定则地图不变）

// //     void Start()
// //     {
// //         GeneratePerlinNoiseMap();

// //         seed = Random.Range(0, 99999);
// //     }

// //     /// <summary>
// //     /// 生成柏林噪声地图
// //     /// </summary>
// //     void GeneratePerlinNoiseMap()
// //     {
// //         // 清空原有瓦片
// //         terrainTilemap.ClearAllTiles();

// //         // 遍历每个瓦片坐标
// //         for (int x = 0; x < mapWidth; x++)
// //         {
// //             for (int y = 0; y < mapHeight; y++)
// //             {
// //                 // 1. 计算采样坐标（加seed避免每次生成相同地图）
// //                 float sampleX = (x + seed) * noiseScale;
// //                 float sampleY = (y + seed) * noiseScale;

// //                 // 2. 采样柏林噪声值（0~1）
// //                 float noiseValue = Mathf.PerlinNoise(sampleX, sampleY);

// //                 // 3. 映射噪声值到地形类型
// //                 TileBase currentTile = grassTile;
// //                 if (noiseValue < 0.3f)      // 低噪声值=水域
// //                     currentTile = waterTile;
// //                 else if (noiseValue > 0.7f) // 高噪声值=山脉
// //                     currentTile = mountainTile;

// //                 // 4. 绘制瓦片（居中显示地图）
// //                 Vector3Int tilePos = new Vector3Int(
// //                     x - mapWidth / 2, 
// //                     y - mapHeight / 2, 
// //                     0
// //                 );
// //                 terrainTilemap.SetTile(tilePos, currentTile);
// //             }
// //         }
// //     }

// //     // 可选：点击按钮重新生成（绑定到UI按钮）
// //     public void RegenerateMap()
// //     {
// //         seed = Random.Range(0, 99999); // 随机新种子
// //         GeneratePerlinNoiseMap();
// //     }
// // }