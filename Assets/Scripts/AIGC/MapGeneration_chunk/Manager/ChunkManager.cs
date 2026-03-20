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
    
    [Header("生成器配置")]
    public GeneratorBase[] generators;

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
        
        data.Dispose();
        _generatedData.Remove(chunkCoord);
        
        Debug.Log($"[ChunkManager] 区块 {chunkCoord} 已卸载");
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
        
        if (generators == null || generators.Length == 0) return;
        
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
        
        // 按数组顺序执行，不使用优先级排序
        
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
        
        // 处理所有队列中的区块
        while (_generationQueue.Count > 0)
        {
            Vector2Int chunkCoord = _generationQueue.Dequeue();
            if (_generatedData.ContainsKey(chunkCoord)) continue;
            
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
                
                while (!isComplete)
                {
                    yield return null;
                }
            }

            // 标记生成完成
            chunkData.IsGenerated = true;
            
            // 绘制区块（通过可视化器）
            ChunkVisualizer.Instance?.DrawChunkData(chunkData);
            
            yield return new WaitForSeconds(0.1f); // 稍微延迟，避免一次性生成过多
        }
        
        _isGenerating = false;
    }
    #endregion
}
