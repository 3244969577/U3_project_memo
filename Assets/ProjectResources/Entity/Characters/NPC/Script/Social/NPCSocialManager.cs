using UnityEngine;
using System.Collections.Generic;
using NPCSocialEvents;
using Newtonsoft.Json;
using GlobalEvents;

// 管理NPC的社交网络
// 使用NPC -> 好感度 的结构
// 好感度范围为[-100, 100]
// 默认为0，表示认识但不熟悉
// 社交关系中没有的就是陌生人
public class NPCSocialManager : Singleton<NPCSocialManager> {
    public Dictionary<string, Dictionary<string, int>> socialRelation = new Dictionary<string, Dictionary<string, int>>();

    public int defaultSocialRelation = -100;

#region event binding
    private EventBinding<NPCGeneratedEvent> npcGeneratedEventBinding;
    private EventBinding<NPCAwareEvent> npcAwareEventBinding;
    private EventBinding<NPCSocialRelationChangeEvent> npcSocialRelationChangeEventBinding;

    private void Awake()
    {
        Debug.Log("NPCSocialManager Awake");
        npcGeneratedEventBinding = new EventBinding<NPCGeneratedEvent>(OnNPCGeneratedEvent);    
        npcAwareEventBinding = new EventBinding<NPCAwareEvent>(OnNPCAwareEvent);
        npcSocialRelationChangeEventBinding = new EventBinding<NPCSocialRelationChangeEvent>(OnNPCSocialRelationChangeEvent);
    }
    private void OnEnable()
    {
        EventBus<NPCGeneratedEvent>.Register(npcGeneratedEventBinding);
        EventBus<NPCAwareEvent>.Register(npcAwareEventBinding);
        EventBus<NPCSocialRelationChangeEvent>.Register(npcSocialRelationChangeEventBinding);
    }
    private void OnDisable()
    {
        EventBus<NPCGeneratedEvent>.Deregister(npcGeneratedEventBinding);
        EventBus<NPCAwareEvent>.Deregister(npcAwareEventBinding);
        EventBus<NPCSocialRelationChangeEvent>.Deregister(npcSocialRelationChangeEventBinding);
    }

    private void OnNPCGeneratedEvent(NPCGeneratedEvent e)
    {
        Dictionary<string, int> relation = new Dictionary<string, int>();
        socialRelation[e.npc.name] = relation;
    }

    private void OnNPCAwareEvent(NPCAwareEvent e)
    {
        Debug.Log(JsonConvert.SerializeObject(socialRelation[e.npc.name]));
        if (!IsInSocialRelationOf(e.npc.name, e.target.name))
        {
            // 新结识的NPC，好感度默认为0
            socialRelation[e.npc.name][e.target.name] = defaultSocialRelation;
            Debug.Log($"Social: NPC {e.npc.name} 新认识了 {e.target.name}");
        }
        else 
        {
            socialRelation[e.npc.name][e.target.name] += 5;
            Debug.Log($"Social: NPC {e.npc.name} 已经认识了 {e.target.name}");
        }
    }

    private void OnNPCSocialRelationChangeEvent(NPCSocialRelationChangeEvent e)
    {
        if (!IsInSocialRelationOf(e.npc, e.target))
        {
            return;
        }
        socialRelation[e.npc][e.target] += e.delta;
        if (socialRelation[e.npc][e.target] > 100)
        {
            socialRelation[e.npc][e.target] = 100;
        }
        else if (socialRelation[e.npc][e.target] < -100)
        {
            socialRelation[e.npc][e.target] = -100;
        }
        Debug.Log($"Social: NPC {e.npc} 对 {e.target} 的好感度增加了 {e.delta}，当前好感度为 {socialRelation[e.npc][e.target]}");
    }

#endregion

#region public methods
    public Dictionary<string, int> GetSocialRelationOf(string npc)
    {
        if (socialRelation.TryGetValue(npc, out Dictionary<string, int> relation))
        {
            return relation;
        }
        return null;
    }

    public int GetSocialRelationOf(string npc, string target)
    {
        if (socialRelation.TryGetValue(npc, out Dictionary<string, int> relation))
        {
            if (relation.TryGetValue(target, out int value))
            {
                return value;
            }
        }
        return 0;
    }

    public bool IsInSocialRelationOf(string npc, string target)
    {
        // 如果返回false则说明不相认识
        return socialRelation.TryGetValue(npc, out Dictionary<string, int> relation) && relation.TryGetValue(target, out int value);
    }

#endregion
}