using UnityEngine;

namespace GlobalEvents
{
    // 敌人被击杀事件
    public struct EnemyKilledEvent : IEvent {
        public GameObject enemy;
        public GameObject killer;
        public int score;
    }

    // 玩家死亡事件
    public struct PlayerDieEvent : IEvent { }

    public struct PlayerHPChangeEvent : IEvent {
        public float currentHealth;
        public float maxHealth;
    }
    // 玩家受击事件
    public struct PlayerHitEvent : IEvent {
        public GameObject attacker;
        public float damage;
        public float currentHealth;
    }
    // 玩家射击事件
    public struct PlayerAttackEvent : IEvent {}

    // 玩家获得装备事件
    public struct ObtainEquipmentEvent : IEvent {
        public GameObject equipment;
    }

    public struct QuitEvent : IEvent { }
    public struct RetryEvent : IEvent { }
    public struct GameOverEvent : IEvent { }
    public struct GameWinEvent : IEvent { }
    public struct GamePauseEvent : IEvent { }
    public struct GameResumeEvent : IEvent { }

    // 生成NPC
    public struct NPCStartGenerateEvent : IEvent { 
        public string name;
        public string personality;
        public string prompt;
    }
    public struct NPCGeneratedEvent : IEvent {
        public GameObject npc;
    }
    public struct NPCRspActionEvent : IEvent {
        public GameObject npc;
        public string action;
    }

    
    // 玩家输入事件
    public struct PlayerInputEvent : IEvent {
        public string input;
        public GameObject target;
    }

    public struct PlayerInteractEvent : IEvent {
        public GameObject target;
    }
    
    // Boss
    public struct BossSpawnEvent : IEvent {
        public GameObject boss;
    }

    public struct BossKilledEvent : IEvent {
        public GameObject boss;
    }


    public struct BulletHitEvent : IEvent {
        public GameObject attacker;
        public GameObject bullet;
        public GameObject target;
        public Vector3 hitPosition;
        public float damage;
    }

    public struct EntityDieEvent : IEvent {
        public GameObject entity;
    }

    public struct PlayerGainExpEvent : IEvent {
        public float exp;
    }
    public struct PlayerLevelUpEvent: IEvent { 
        public int newLevel;
    }
}