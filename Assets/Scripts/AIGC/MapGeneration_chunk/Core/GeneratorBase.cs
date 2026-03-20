using UnityEngine;
using System;

/// <summary>
/// 生成器基类（继承MonoBehaviour）
/// </summary>
public abstract class GeneratorBase : MonoBehaviour, IChunkGenerator
{
    [Header("基础配置")]
    public bool isEnabled = true;

    // 接口实现
    public bool IsEnabled { get => isEnabled; set => isEnabled = value; }

    // 抽象方法
    public abstract void Initialize();
    public abstract void Generate(IChunkData chunkData, Action onComplete = null);
    public abstract void Cleanup();
}
