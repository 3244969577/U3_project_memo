using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

/// <summary>
/// 全局区块管理器（单例）
/// </summary>
public class ChunkManager : MonoBehaviour
{
    [Header("基础配置")]
    public int GlobalSeed = 12345;
    public bool UseRandomSeed = false; // 是否使用随机种子
    
    [Header("生成器配置")]
    public GeneratorBase[] generators;
    public GameObject GeneratorsParent; // 生成器父物体，当generators数组为空时使用

    // 内部缓存
    private readonly Dictionary<Vector2Int, IChunkData> _generatedData = new();
    private readonly Queue<Vector2Int> _generationQueue = new();
    private bool _isGenerating = false;
    private List<GeneratorBase> _chunkGenerators = new();

    // 单例实例
    public static ChunkManager Instance { get; private set; }
    
    // 公共属性，用于暴露已生成的区块坐标
    public IEnumerable<Vector2Int> GeneratedChunkCoords => _generatedData.Keys;

    #region 生命周期
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        // 根据设置选择种子
        if (UseRandomSeed)
        {
            GlobalSeed = UnityEngine.Random.Range(0, int.MaxValue);
            Debug.Log($"[ChunkManager] 使用随机种子: {GlobalSeed}");
        }
        MapConstants.GlobalSeed = GlobalSeed;
        
        
        
        // 初始化生成器
        InitializeGenerators();
    }

    private void OnDestroy()
    {
        foreach (var data in _generatedData.Values)
        {
            data.Dispose();
        }
        
        _generatedData.Clear();
    }
    #endregion

    #region 核心API
    /// <summary>
    /// 请求加载指定区块
    /// </summary>
    public void RequestLoadChunk(Vector2Int chunkCoord)
    {
        if (_generatedData.ContainsKey(chunkCoord)) return;
        
        // 加入生成队列
        if (!_generationQueue.Contains(chunkCoord))
        {
            _generationQueue.Enqueue(chunkCoord);
            StartCoroutine(ProcessGenerationQueue());
        }
    }

    /// <summary>
    /// 请求卸载指定区块
    /// </summary>
    public void RequestUnloadChunk(Vector2Int chunkCoord)
    {
        if (!_generatedData.TryGetValue(chunkCoord, out IChunkData data)) return;
        
        // 清理区块内的预制体
        CleanupChunkPrefabs(chunkCoord);
        
        data.Dispose();
        _generatedData.Remove(chunkCoord);
        
        Debug.Log($"[ChunkManager] 区块 {chunkCoord} 已卸载");
    }

    /// <summary>
    /// 清理区块内的预制体
    /// </summary>
    private void CleanupChunkPrefabs(Vector2Int chunkCoord)
    {
        BoundsInt chunkBounds = ChunkCoordinateUtility.GetChunkWorldBounds(chunkCoord, false);
        
        // 遍历所有生成器，清理它们的父对象中的预制体
        foreach (var generator in _chunkGenerators)
        {
            if (generator is PrefabGeneratorBase prefabGenerator && prefabGenerator.ParentObject != null)
            {
                CleanupPrefabsInParent(prefabGenerator.ParentObject.transform, chunkBounds);
            }
        }
    }

    /// <summary>
    /// 清理指定父对象下的预制体
    /// </summary>
    private void CleanupPrefabsInParent(Transform parent, BoundsInt bounds)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            
            Vector3 pos = child.position;
            // 检查对象是否在区块范围内
            if (bounds.Contains(new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), 0)))
            {
                GameObject.Destroy(child.gameObject);
            }
            else if (child.childCount > 0)
            {
                // 递归清理子父对象
                CleanupPrefabsInParent(child, bounds);
            }
        }
    }

    /// <summary>
    /// 获取已生成的区块数据
    /// </summary>
    public IChunkData GetChunkData(Vector2Int chunkCoord)
    {
        _generatedData.TryGetValue(chunkCoord, out IChunkData data);
        return data;
    }
    #endregion

    #region 内部逻辑
    /// <summary>
    /// 初始化所有生成器
    /// </summary>
    private void InitializeGenerators()
    {
        // 清空内部生成器列表
        _chunkGenerators.Clear();
        
        if (generators != null && generators.Length > 0)
        {
            // 从数组中提取生成器实例
            foreach (var generator in generators)
            {
                if (generator != null)
                {
                    // 如果生成器没有父物体，将其设置为当前物体的子物体
                    if (generator.transform.parent == null)
                    {
                        generator.transform.SetParent(transform);
                    }
                    _chunkGenerators.Add(generator);
                }
            }
        }
        else if (GeneratorsParent != null)
        {
            // 从GeneratorsParent的子物体中加载生成器
            foreach (Transform child in GeneratorsParent.transform)
            {
                GeneratorBase generator = child.GetComponent<GeneratorBase>();
                if (generator != null)
                {
                    _chunkGenerators.Add(generator);
                    Debug.Log($"从GeneratorsParent加载生成器: {generator.name}");
                }
            }
        }
        
        // 按顺序执行，不使用优先级排序
        
        // 初始化每个生成器
        foreach (var generator in _chunkGenerators)
        {
            if (generator != null)
            {
                generator.Initialize();
            }
        }
    }

    /// <summary>
    /// 处理生成队列
    /// </summary>
    private IEnumerator ProcessGenerationQueue()
    {
        if (_isGenerating || _generationQueue.Count == 0) yield break;
        
        _isGenerating = true;
        
        // 处理队列中的区块，逐个处理以避免阻塞
        while (_generationQueue.Count > 0)
        {
            Vector2Int chunkCoord = _generationQueue.Dequeue();
            if (_generatedData.ContainsKey(chunkCoord)) continue;
            
            // 生成区块
            yield return StartCoroutine(GenerateChunk(chunkCoord));
            
            // 每处理一个区块后等待一帧，避免阻塞
            yield return null;
        }
        
        _isGenerating = false;
    }

    /// <summary>
    /// 生成单个区块
    /// </summary>
    private IEnumerator GenerateChunk(Vector2Int chunkCoord)
    {
        // 创建区块数据
        IChunkData chunkData = new ChunkData(chunkCoord);
        _generatedData[chunkCoord] = chunkData;

        // 按顺序执行生成器
        foreach (var generator in _chunkGenerators)
        {
            if (generator == null || !generator.IsEnabled) continue;
            
            // 等待生成器完成
            bool isComplete = false;
            generator.Generate(chunkData, () => isComplete = true);
            
            // 每帧检查一次，避免完全阻塞
            while (!isComplete)
            {
                yield return null;
            }
        }

        // 标记生成完成
        chunkData.IsGenerated = true;
        
        // 绘制区块（通过可视化器）
        ChunkVisualizer.Instance?.DrawChunkData(chunkData);
        
        // 稍微延迟，避免一次性生成过多
        yield return new WaitForSeconds(0.05f);
    }
    #endregion
}
