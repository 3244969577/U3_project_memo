using UnityEngine;

namespace NPCActionEvents
{
    public struct NPCEscapeEvent: IEvent {
        public GameObject npc;
        public GameObject target;
    }

    public struct NPCStandbyEvent: IEvent {
        public GameObject npc;
        public float timeout;
    }

    public struct NPCFollowEvent: IEvent {
        public GameObject npc;
        public GameObject target;
    }

    public struct NPCGivePotionEvent: IEvent {
        public GameObject npc;
        public int amount;
    }

    public struct NPCGiveCoinEvent: IEvent {
        public GameObject npc;
        public int amount;
    }

    public struct NPCEmotionEvent: IEvent {
        public GameObject npc;
        public string emotionName;
    }
}