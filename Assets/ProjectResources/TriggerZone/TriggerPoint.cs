using System;
using UnityEngine;

/// <summary>
/// 触发器点 - 当玩家进入时触发事件
/// - 使用BoxCollider2D作为触发器
/// - 支持单次触发或多次触发模式
/// - 触发时会调用OnPlayerEnterTrigger事件
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class TriggerPoint : MonoBehaviour
{
    /*
     * Trigger point to affect a trigger zone
     * If criteria meets, OnPlayerEnterTrigger will be called by trigger zone
     */

    public event EventHandler OnPlayerEnterTrigger;

    public bool triggerOnce = true;
    public enum State
    {
        Untriggered,
        Triggered,
    }

    private State state;


    protected void Awake()
    {
        state = State.Untriggered;
    }

    // Trigger the trigger zone
    protected void InvokeTrigger()
    {
        EventHandler handler = OnPlayerEnterTrigger;
        if (handler != null)
            handler(this, EventArgs.Empty);
    }

    // Trigger on collision
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (state == State.Untriggered || !triggerOnce)
            {
                InvokeTrigger();
                state = State.Triggered;
            }
        }
    }
}
