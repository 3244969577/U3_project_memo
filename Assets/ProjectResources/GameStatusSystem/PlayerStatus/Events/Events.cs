using UnityEngine;
namespace GameStatusSystem.PlayerStatus.Events
{
    // 敌人被击杀事件
    public struct EnemyKilledEvent : IEvent {
        public GameObject enemy;
        public GameObject killer;
        public int score;
    }

    // 玩家受击事件
    public struct PlayerHitEvent : IEvent {
        public GameObject attacker;
        public float damage;
        public float currentHealth;
    }
    // 玩家射击事件
    public struct PlayerAttackEvent : IEvent {}

    // 玩家命中敌人事件
    public struct PlayerHitEnemyEvent : IEvent {
        public GameObject enemy;
        public float damage;
    }

    // 玩家子弹未命中事件
    public struct PlayerBulletMissEvent : IEvent {}

    // 玩家获得装备事件
    public struct ObtainEquipmentEvent : IEvent {
        public GameObject equipment;
    }
}

