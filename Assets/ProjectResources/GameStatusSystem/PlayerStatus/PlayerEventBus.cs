using System;
using UnityEngine;
using GlobalEvents;

namespace GameStatusSystem.PlayerStatus
{
    public class PlayerEventBus : MonoBehaviour {

        public static PlayerEventBus Instance { get; private set; }

        private CoreData _coreData;       ////// 玩家核心数据
        private CombatStats _combatStats; ////// 玩家战斗状态
        private StatsRecorder _statsRecorder; // 实时数据

        // 外部只读访问
        public CoreData CoreData => _coreData;
        public CombatStats CombatStats => _combatStats;
        public StatsRecorder Recorder => _statsRecorder;

        public PlayerStatusSnapshot GetSnapshot()
        {
            var snapshot = new PlayerStatusSnapshot();
            snapshot.MaxHealth = _coreData.maxHealth;
            snapshot.CurrentHealth = _combatStats.currentHealth;
            snapshot.KillStreak = _combatStats.killStreak;
            snapshot.HitRate = _statsRecorder.GetHitRate();
            snapshot.EquipmentScore = _coreData.EquipmentScore;
            snapshot.TotalScore = _combatStats.totalScore;

            return snapshot;
            // return new PlayerStatusSnapshot(
            //     // Level = _coreData.Level,
            //     MaxHealth = _coreData.maxHealth,
            //     // EquipmentScore = _coreData.EquipmentScore,
            //     CurrentHealth = _combatStats.currentHealth,
            //     KillStreak = _combatStats.killStreak,
            //     // TotalDeaths = _combatStats.totalDeaths,
            //     HitRate = _statsRecorder.GetHitRate()
            // );
        }

        void Start()
        {
            Instance = this;
            Initialize();
        
        }

        private void Initialize()
        {
            _coreData = new CoreData();
            _combatStats = new CombatStats();
            _statsRecorder = new StatsRecorder();

            _coreData.maxHealth = Player.instance.maxHealth;
            // 注册游戏事件监听器
            RegisterGameEventListeners();
        }


        private void RegisterGameEventListeners()
        {
            // 注册游戏中相关的事件监听器
            // EventBus<PlayerEvent>.Register(new EventBinding<PlayerEvent>(HandlePlayerEvent));
            EventBus<EnemyKilledEvent>.Register(new EventBinding<EnemyKilledEvent>(HandleEnemyKilledEvent));
            EventBus<PlayerAttackEvent>.Register(new EventBinding<PlayerAttackEvent>(HandlePlayerAttackEvent));
            EventBus<PlayerHitEvent>.Register(new EventBinding<PlayerHitEvent>(HandlePlayerHitEvent));
            EventBus<PlayerHitEnemyEvent>.Register(new EventBinding<PlayerHitEnemyEvent>(HandlePlayerHitEnemyEvent));
            // EventBus<PlayerBulletMissEvent>.Register(new EventBinding<PlayerBulletMissEvent>(HandlePlayerBulletMissEvent));
            EventBus<ObtainEquipmentEvent>.Register(new EventBinding<ObtainEquipmentEvent>(HandleObtainEquipmentEvent));
        }

        // 事件处理函数
        private void HandleObtainEquipmentEvent(ObtainEquipmentEvent e)
        {
            Debug.Log($"ObtainEquipmentEvent: {e.equipment.name}");
            _coreData.EquipmentScore += e.equipment.GetComponent<Weapon>().score;
        }

        // 事件处理函数
        // private void HandlePlayerEvent(PlayerEvent e)
        // {
        //     Debug.Log($"PlayerEvent");
        // }

        private void HandleEnemyKilledEvent(EnemyKilledEvent e)
        {
            Debug.Log($"EnemyKilledEvent");
            _combatStats.killStreak++;
            _combatStats.totalScore += e.score;
        }

        private void HandlePlayerAttackEvent(PlayerAttackEvent e)
        {
            Debug.Log($"PlayerAttackEvent");
            _combatStats.totalShots++;
            _statsRecorder.RecordShot();
        }

        private void HandlePlayerHitEvent(PlayerHitEvent e)
        {
            Debug.Log($"PlayerHitEvent: hp={e.currentHealth}");
            _combatStats.totalHits++;
            _combatStats.currentHealth = e.currentHealth;
        }

        private void HandlePlayerHitEnemyEvent(PlayerHitEnemyEvent e)
        {
            Debug.Log($"PlayerHitEnemyEvent");
            _combatStats.totalHits++;
            _statsRecorder.RecordHit();
        }

        // private void HandlePlayerBulletMissEvent(PlayerBulletMissEvent e)
        // {
        //     Debug.Log($"PlayerBulletMissEvent");
        //     // _statsRecorder.RecordMiss();
        // }
    }
}
