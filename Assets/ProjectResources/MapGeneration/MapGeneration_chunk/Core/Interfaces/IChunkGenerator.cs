using System;

/// <summary>
/// 区块生成器接口（定义区块生成的标准行为）
/// </summary>
public interface IChunkGenerator
{
    /// <summary>
    /// 是否启用该生成器
    /// </summary>
    bool IsEnabled { get; set; }

    /// <summary>
    /// 初始化生成器
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// 为指定区块执行生成逻辑
    /// </summary>
    /// <param name="chunkData">目标区块数据</param>
    /// <param name="onComplete">生成完成回调</param>
    void Generate(IChunkData chunkData, Action onComplete = null);
    
    /// <summary>
    /// 清理生成器资源
    /// </summary>
    void Cleanup();
}
