using UnityEngine;

/// <summary>
/// 随机事件基类
/// 所有具体事件都继承自此类
/// </summary>
public abstract class RandomEvent : MonoBehaviour
{
    [Header("事件配置")]
    public string eventName; // 事件名称
    public float baseProbability = 0.1f; // 基础触发概率
    public float cooldownTime = 30f; // 冷却时间（秒）
    public float minDelay = 10f; // 最小延迟时间（秒）
    public float maxDelay = 60f; // 最大延迟时间（秒）

    /// <summary>
    /// 事件是否可以触发
    /// </summary>
    public bool CanTrigger { get; set; } = true;

    /// <summary>
    /// 上次触发时间
    /// </summary>
    public float LastTriggerTime { get; set; } = -Mathf.Infinity;

    /// <summary>
    /// 触发事件
    /// </summary>
    public abstract void Trigger();

    /// <summary>
    /// 检查事件是否可以触发
    /// </summary>
    /// <returns>是否可以触发</returns>
    public virtual bool CheckCanTrigger()
    {
        if (!CanTrigger)
            return false;

        if (Time.time < LastTriggerTime + cooldownTime)
            return false;

        return true;
    }

    /// <summary>
    /// 计算事件触发概率
    /// </summary>
    /// <returns>触发概率</returns>
    public virtual float CalculateProbability()
    {
        return baseProbability;
    }
}
