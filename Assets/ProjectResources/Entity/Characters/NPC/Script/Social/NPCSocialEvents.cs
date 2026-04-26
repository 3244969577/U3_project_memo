using UnityEngine;

namespace NPCSocialEvents 
{
    // NPC之间相互感知-新结识的NPC
    public struct NPCAwareEvent: IEvent
    {
        public GameObject npc;
        public GameObject target;
    }

    // 好感事件-好感度改变
    public struct NPCSocialRelationChangeEvent: IEvent
    {
        public string npc;
        public string target;
        public int delta;
    }
}