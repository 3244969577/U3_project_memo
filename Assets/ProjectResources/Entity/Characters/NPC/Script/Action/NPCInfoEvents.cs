using UnityEngine;

namespace NPCInfoEvents
{
    public struct NPCChangeDirEvent : IEvent
    {
        public NPCMovementController npcMovementController;
        public NPCDirection oldDirection;
        public NPCDirection newDirection;
    }

    public struct NPCMoveEvent : IEvent
    {
        public NPCMovementController npcMovementController;
    }
    public struct NPCStopMoveEvent : IEvent
    {
        public NPCMovementController npcMovementController;
    }
}