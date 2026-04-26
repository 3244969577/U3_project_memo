using UnityEngine;
using GlobalEvents;
using NPCActionEvents;

public class NPCAction : MonoBehaviour {
    private NPC_Genearted _npcGenerated;
    private EventBinding<NPCRspActionEvent> _npcRspActionEventBinding;
    private void Awake()
    {
        _npcGenerated = GetComponent<NPC_Genearted>();
        _npcRspActionEventBinding = new EventBinding<NPCRspActionEvent>(HandleNPCRspAction);
    }
    private void OnEnable()
    {
        EventBus<NPCRspActionEvent>.Register(_npcRspActionEventBinding);
    }
    private void OnDisable()
    {
        EventBus<NPCRspActionEvent>.Deregister(_npcRspActionEventBinding);
    }

    private void HandleNPCRspAction(NPCRspActionEvent e)
    {
        if (e.npc != gameObject)
        {
            return;
        }
        switch (e.action)
        {
            case "escape":
                // _npcGenerated.EnableMovement();
                _npcGenerated.NPC_EventBus.Raise<NPCEscapeEvent>(
                    new NPCEscapeEvent(){npc = gameObject, target = GameManager.Instance.player}
                );
                break;
            case "standby":
                _npcGenerated.NPC_EventBus.Raise<NPCStandbyEvent>(
                    new NPCStandbyEvent(){npc = gameObject, timeout = 5f}
                );
                break;
            case "follow":
                _npcGenerated.NPC_EventBus.Raise<NPCFollowEvent>(
                    new NPCFollowEvent(){npc = gameObject, target = GameManager.Instance.player}
                );
                break;
            case "give_potion":
                _npcGenerated.NPC_EventBus.Raise<NPCGivePotionEvent>(
                    new NPCGivePotionEvent(){npc = gameObject, amount = 1}
                );
                break;
            case "give_coin":
                _npcGenerated.NPC_EventBus.Raise<NPCGiveCoinEvent>(
                    new NPCGiveCoinEvent(){npc = gameObject, amount = 5}
                );
                break;
            default:
                break;
        }
    }
}