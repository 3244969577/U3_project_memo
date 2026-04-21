using UnityEngine;
using GlobalEvents;
using NPCActionEvents;
using NPCInfoEvents;

public class TipListener : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private EventBinding<NPCGeneratedEvent> npcGeneratedEventBinding;

    public TipManager tipManager;
    
    private void Awake()
    {
        npcGeneratedEventBinding = new EventBinding<NPCGeneratedEvent>(OnNPCGenerated);
        
    }
    private void OnEnable()
    {
        EventBus<NPCGeneratedEvent>.Register(npcGeneratedEventBinding);
    }
    private void OnDisable()
    {
        EventBus<NPCGeneratedEvent>.Deregister(npcGeneratedEventBinding);
    }

    private void OnNPCGenerated(NPCGeneratedEvent e)
    {
        tipManager.ShowTip("Generated!");
        Debug.Log("NPCGeneratedEvent raised");
    }
}
