using UnityEngine;
using GlobalEvents;
using NPCActionEvents;
using NPCInfoEvents;
using EasyUI.Toast;
public class TipListener : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private EventBinding<NPCGeneratedEvent> npcGeneratedEventBinding;
    private EventBinding<NPCStartGenerateEvent> npcStartGenerateEventBinding;
    private EventBinding<BossKilledEvent> bossKilledEventBinding;
    private EventBinding<PlayerLevelUpEvent> playerLevelUpEventBinding;

    public float toastDuration = 2f;
    public ToastPosition toastPosition = ToastPosition.TopRight;

    public TipManager tipManager;
    
    private void Awake()
    {
        npcGeneratedEventBinding = new EventBinding<NPCGeneratedEvent>(OnNPCGenerated);
        npcStartGenerateEventBinding = new EventBinding<NPCStartGenerateEvent>(OnNPCStartGenerate);
        bossKilledEventBinding = new EventBinding<BossKilledEvent>(OnBossKilled);
        playerLevelUpEventBinding = new EventBinding<PlayerLevelUpEvent>(OnPlayerLevelUp);
    }
    private void OnEnable()
    {
        EventBus<NPCGeneratedEvent>.Register(npcGeneratedEventBinding);
        EventBus<NPCStartGenerateEvent>.Register(npcStartGenerateEventBinding);
        EventBus<BossKilledEvent>.Register(bossKilledEventBinding);
        EventBus<PlayerLevelUpEvent>.Register(playerLevelUpEventBinding);
    }
    private void OnDisable()
    {
        EventBus<NPCGeneratedEvent>.Deregister(npcGeneratedEventBinding);
        EventBus<NPCStartGenerateEvent>.Deregister(npcStartGenerateEventBinding);
        EventBus<BossKilledEvent>.Deregister(bossKilledEventBinding);
        EventBus<PlayerLevelUpEvent>.Deregister(playerLevelUpEventBinding);
    }

    private void OnNPCGenerated(NPCGeneratedEvent e)
    {
        Toast.Show($"{e.npc.name} Generated!", toastDuration, toastPosition);
    }
    private void OnNPCStartGenerate(NPCStartGenerateEvent e)
    {
        Toast.Show($" StartGenerate {e.name}!", toastDuration, toastPosition);
    }
    private void OnBossKilled(BossKilledEvent e)
    {
        Toast.Show($"{e.boss.name} Killed!", toastDuration, toastPosition);
    }
    private void OnPlayerLevelUp(PlayerLevelUpEvent e)
    {
        Toast.Show($"LevelUp {e.newLevel}!", toastDuration, toastPosition);
    }
}
