using UnityEngine;
using GameStatusSystem.DifficultyCalculation;
using GameStatusSystem.PlayerStatus;
using GameStatusSystem.DifficultyApply;

namespace GameStatusSystem.DifficultySystem
{
    public class DifficultySystem : MonoBehaviour
    {
        [Header("难度评估配置")]
        public DifficultySettings settings;

        private PlayerEventWatcher _playerStatus;
        private float _currentDifficulty = 1.0f;

        private void Start()
        {
            _playerStatus = PlayerEventWatcher.Instance;

            // 每5秒评估一次
            InvokeRepeating(nameof(UpdateDifficulty), 1f, 5f);
        }

        private void UpdateDifficulty()
        {
            Debug.Log("开始评估难度");
            // 1. 获取快照（第一层）
            var snapshot = _playerStatus.GetSnapshot();

            // 2. 难度评估（第二层）
            var evaluation = DifficultyEvaluator.Evaluate(snapshot, settings);

            // 3. 平滑更新难度（第三层）
            _currentDifficulty = Mathf.Lerp(
                _currentDifficulty,
                evaluation.RecommendedDifficulty,
                0.3f
            );

            Debug.Log($"难度评估：{evaluation.TotalScore:0} → 难度：{_currentDifficulty:0.00}");
            
            // 4. 更新全局难度参数（第四层）
            GlobalDifficultyLevel.UpdateDifficulty(_currentDifficulty);
        }
    }
}