using UnityEngine;
using GlobalEvents;
using System.Reflection;


public class PlayerLevelSystem : MonoBehaviour {
    public int CurrentLevel {get; private set;} = 1;
    public float CurrentExp {get; private set;} = 0;
    public float NextLevelExp {get; private set;} = 100;
    [SerializeField] private float _expProgress = 1.2f;

    private EventBinding<PlayerGainExpEvent> _playerGainExpEventBinding;


#region hook

    private void Awake()
    {
        _playerGainExpEventBinding = new EventBinding<PlayerGainExpEvent>(HandleGainExpEvent);
    }
#endregion


#region Event Binding
    private void OnEnable()
    {
        EventBus<PlayerGainExpEvent>.Register(_playerGainExpEventBinding);
    }

    private void OnDisable()
    {
        EventBus<PlayerGainExpEvent>.Deregister(_playerGainExpEventBinding);
    }

    private void HandleGainExpEvent(PlayerGainExpEvent e)
    {
        CurrentExp += e.exp;
        // 支持连续升级
        while (CurrentExp >= NextLevelExp)
        {
            CurrentExp -= NextLevelExp;
            CurrentLevel++;
            NextLevelExp *= _expProgress;

            // 触发升级事件
            EventBus<PlayerLevelUpEvent>.Raise(new PlayerLevelUpEvent { newLevel = CurrentLevel });
        }
    }
#endregion


}