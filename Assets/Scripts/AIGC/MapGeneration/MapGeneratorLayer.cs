using UnityEngine;
using System.Collections.Generic;

// 所有生成层的抽象基类（核心接口）
public abstract class MapGeneratorLayer // : ScriptableObject
{
    [Header("基础配置")]
    public string layerName; // 层名称（便于管理）
    public bool isEnabled = true; // 是否启用该层
    
    // 核心生成方法（所有层必须实现）
    public abstract void Generate(MapGeneratorData data);
    
    // 可选：层初始化（如预加载资源）
    public virtual void Initialize(MapGeneratorData data) { }
    
    // 可选：层清理（如销毁临时对象）
    public virtual void Cleanup() { }
}