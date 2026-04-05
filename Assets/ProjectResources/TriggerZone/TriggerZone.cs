using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 触发器区域基类 - 响应TriggerPoint的触发事件
/// - 持有一个TriggerPoint引用
/// - 订阅TriggerPoint的OnPlayerEnterTrigger事件
/// - 提供虚方法TriggerFunction作为事件处理的默认实现
/// </summary>
public class TriggerZone : MonoBehaviour
{
    /*
     * Zone affected by a trigger point
     */

    public TriggerPoint triggerPoint;

    protected virtual void Start()
    {
        triggerPoint.OnPlayerEnterTrigger += TriggerFunction;
    }

    protected virtual void TriggerFunction(object sender, System.EventArgs e) { }

}
