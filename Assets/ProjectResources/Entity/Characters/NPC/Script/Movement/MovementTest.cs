using UnityEngine;
using GlobalEvents;
using NPCInfoEvents;
using NPCActionEvents;

public class MovementTest : MonoBehaviour {
    public GameObject npc;
    private NPCMovementController npcMovementController;
    private LocalEventBus localEventBus;
    private void Awake()
    {
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            npcMovementController = npc.GetComponent<NPCMovementController>();
            if (npcMovementController != null) {
                npcMovementController.SetTargetPosition(Player.instance.transform.position);
            }
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            RaiseEmotionEvent();
        }
    }
    public void RaiseFollowEvent()
    {
        localEventBus = npc.GetComponent<NPC_Genearted>().NPC_EventBus;
        localEventBus.Raise<NPCFollowEvent>(new NPCFollowEvent{npc = npc, target = Player.instance.gameObject});
    }

    public void RaiseEscapeEvent()
    {
        localEventBus = npc.GetComponent<NPC_Genearted>().NPC_EventBus;
        localEventBus.Raise<NPCEscapeEvent>(new NPCEscapeEvent{npc = npc, target = Player.instance.gameObject});
    }

    public void RaiseStandbyEvent()
    {
        localEventBus = npc.GetComponent<NPC_Genearted>().NPC_EventBus;
        localEventBus.Raise<NPCStandbyEvent>(new NPCStandbyEvent{npc = npc, timeout = 5f});
    }


    public void RaiseEmotionEvent()
    {
        localEventBus = npc.GetComponent<NPC_Genearted>().NPC_EventBus;
        localEventBus.Raise<NPCEmotionEvent>(new NPCEmotionEvent{npc = npc, emotionName = "HappyGiggle"});
    }
}