using UnityEngine;
using System;

/// <summary>
/// 调试生成器（用于打印生成过程中的相关数据）
/// </summary>
[Serializable]
public class DebugGenerator : GeneratorBase
{
    [Header("调试配置")]
    public bool PrintChunkInfo = true; // 是否打印区块信息
    public bool PrintDataInfo = true;  // 是否打印数据信息
    public bool PrintTimingInfo = true; // 是否打印时间信息

    // 构造函数
    public DebugGenerator()
    {
        isEnabled = true;
    }

    /// <summary>
    /// 初始化生成器
    /// </summary>
    public override void Initialize()
    {
        Debug.Log("[DebugGenerator] 初始化调试生成器");
    }

    /// <summary>
    /// 生成区块（仅打印调试信息）
    /// </summary>
    public override void Generate(IChunkData chunkData, Action onComplete = null)
    {
        if (!IsEnabled || chunkData == null)
        {
            onComplete?.Invoke();
            return;
        }

        try
        {
            float startTime = Time.time;
            Vector2Int chunkCoord = chunkData.ChunkCoord;

            if (PrintChunkInfo)
            {
                Debug.Log($"[DebugGenerator] 处理区块: {chunkCoord}");
                Debug.Log($"[DebugGenerator] 区块生成状态: {chunkData.IsGenerated}");
            }

            if (PrintDataInfo)
            {
                // 尝试获取不同类型的数据数量
                try
                {
                    var terrainData = chunkData.GetAllData<TerrainType>();
                    Debug.Log($"[DebugGenerator] 地形数据数量: {terrainData.Count}");
                }
                catch (Exception e)
                {
                    Debug.Log($"[DebugGenerator] 获取地形数据时出错: {e.Message}");
                }

                try
                {
                    var structureData = chunkData.GetAllData<FineStructureType>();
                    Debug.Log($"[DebugGenerator] 结构数据数量: {structureData.Count}");
                }
                catch (Exception e)
                {
                    Debug.Log($"[DebugGenerator] 获取结构数据时出错: {e.Message}");
                }

                try
                {
                    var decorationData = chunkData.GetAllData<DecorationType>();
                    Debug.Log($"[DebugGenerator] 装饰数据数量: {decorationData.Count}");
                }
                catch (Exception e)
                {
                    Debug.Log($"[DebugGenerator] 获取装饰数据时出错: {e.Message}");
                }
            }

            if (PrintTimingInfo)
            {
                float elapsedTime = Time.time - startTime;
                Debug.Log($"[DebugGenerator] 处理时间: {elapsedTime:F4} 秒");
            }

            Debug.Log($"[DebugGenerator] 调试生成完成: {chunkCoord}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DebugGenerator] 生成过程中出错: {e.Message}");
        }
        finally
        {
            onComplete?.Invoke();
        }
    }

    /// <summary>
    /// 清理生成器
    /// </summary>
    public override void Cleanup()
    {
        Debug.Log("[DebugGenerator] 清理调试生成器");
    }
}