using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 随机事件管理器
/// 负责管理所有随机事件的触发和调度
/// </summary>
public class RandomEventManager : MonoBehaviour
{
    [Header("管理器配置")]
    public float checkInterval = 5f; // 检查间隔（秒）
    public bool isEnabled = true; // 是否启用

    private List<RandomEvent> events = new List<RandomEvent>();
    private float lastCheckTime = 0f;

    public static RandomEventManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // 自动注册场景中的所有随机事件
        RegisterAllEvents();
    }

    private void Update()
    {
        if (!isEnabled)
            return;

        if (Time.time >= lastCheckTime + checkInterval)
        {
            CheckAndTriggerEvents();
            lastCheckTime = Time.time;
        }
    }

    /// <summary>
    /// 注册所有事件
    /// </summary>
    private void RegisterAllEvents()
    {
        // 查找场景中所有的随机事件
        RandomEvent[] eventComponents = FindObjectsOfType<RandomEvent>();
        foreach (var ev in eventComponents)
        {
            RegisterEvent(ev);
        }
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="randomEvent">要注册的事件</param>
    public void RegisterEvent(RandomEvent randomEvent)
    {
        if (!events.Contains(randomEvent))
        {
            events.Add(randomEvent);
            Debug.Log($"已注册事件: {randomEvent.eventName}");
        }
    }

    /// <summary>
    /// 注销事件
    /// </summary>
    /// <param name="randomEvent">要注销的事件</param>
    public void UnregisterEvent(RandomEvent randomEvent)
    {
        if (events.Contains(randomEvent))
        {
            events.Remove(randomEvent);
            Debug.Log($"已注销事件: {randomEvent.eventName}");
        }
    }

    /// <summary>
    /// 检查并触发事件
    /// </summary>
    private void CheckAndTriggerEvents()
    {
        foreach (var ev in events)
        {
            if (ev.CheckCanTrigger())
            {
                float probability = ev.CalculateProbability();
                if (Random.value <= probability)
                {
                    // 随机延迟触发
                    float delay = Random.Range(ev.minDelay, ev.maxDelay);
                    StartCoroutine(TriggerEventWithDelay(ev, delay));
                }
            }
        }
    }

    /// <summary>
    /// 延迟触发事件
    /// </summary>
    /// <param name="ev">要触发的事件</param>
    /// <param name="delay">延迟时间</param>
    private System.Collections.IEnumerator TriggerEventWithDelay(RandomEvent ev, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // 再次检查是否可以触发（避免在延迟期间状态发生变化）
        if (ev.CheckCanTrigger())
        {
            ev.Trigger();
            ev.LastTriggerTime = Time.time;
            Debug.Log($"触发事件: {ev.eventName}");
        }
    }

    /// <summary>
    /// 手动触发事件
    /// </summary>
    /// <param name="eventName">事件名称</param>
    public void TriggerEvent(string eventName)
    {
        foreach (var ev in events)
        {
            if (ev.eventName == eventName && ev.CheckCanTrigger())
            {
                ev.Trigger();
                ev.LastTriggerTime = Time.time;
                Debug.Log($"手动触发事件: {ev.eventName}");
                break;
            }
        }
    }

    /// <summary>
    /// 获取所有注册的事件
    /// </summary>
    /// <returns>事件列表</returns>
    public List<RandomEvent> GetAllEvents()
    {
        return new List<RandomEvent>(events);
    }
}
