using UnityEngine;
using System.Collections.Generic;

public class MapGeneratorController : MonoBehaviour
{
    [Header("全局配置")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public int seed = 0;
    public bool useRandomSeed = true; // 是否使用随机种子
    
    [Header("生成层配置（执行顺序 = 列表顺序）")]
    public List<MapGeneratorLayer> generatorLayers; // 生成层列表（可在Inspector拖拽调整顺序）
    
    [Header("可视化组件")]
    public MapVisualizer mapVisualizer; // 拖拽挂载了MapVisualizer的物体

    // 全局数据容器
    private MapGeneratorData mapData;

    private void Awake()
    {
        // 初始化数据容器
        mapData = new MapGeneratorData()
        {
            mapWidth = mapWidth,
            mapHeight = mapHeight
        };

        generatorLayers = new List<MapGeneratorLayer>()
        {
            // new PerlinNoiseLayer()
            new MultiNoiseMapGeneratorLayer()
        };
    }
    private void Start()
    {
        Debug.Log("开始生成地图");
        GenerateMap();
    }

    // 一键生成地图（对外暴露的唯一入口）
    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        // 1. 重置数据
        mapData.Clear();
        
        // 2. 设置种子
        mapData.seed = useRandomSeed ? Random.Range(0, int.MaxValue) : seed;
        
        // 3. 按顺序执行所有生成层
        foreach (var layer in generatorLayers)
        {
            if (layer == null) continue;
            
            // 初始化层
            layer.Initialize(mapData);
            
            // 执行生成
            layer.Generate(mapData);
            
            // 清理层临时数据
            layer.Cleanup();
        }
        
        // 4. 可视化生成结果（瓦片/物体实例化，根据项目需求实现）
        VisualizeMap();
        
        Debug.Log("地图生成完成！");
    }

    // 可视化生成结果（核心逻辑：根据数据容器生成瓦片/物体）
    private void VisualizeMap()
    {
        // 调用可视化层绘制所有层
        if (mapVisualizer == null) {
            Debug.LogError("MapVisualizer 未赋值！请在Inspector中拖拽挂载！");
            return;
        }
        Debug.Log("开始可视化地图");
        mapVisualizer.DrawAllLayers(mapData);
    }

    // 清空地图（用于重新生成）
    [ContextMenu("Clear Map")]
    public void ClearMap()
    {
        mapData.Clear();
        // 销毁所有生成的瓦片/物体
        // ...
        Debug.Log("地图已清空");
    }
}