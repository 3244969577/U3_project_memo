using UnityEngine;
using NPCActionEvents;
using GlobalEvents;

public class NPCEmotion : MonoBehaviour
{
    private NPC_Genearted npcGenerator;
    private EventBinding<NPCEmotionEvent> npcEmotionEventBinding;
    private EventBinding<PlayerInputEvent> playerInputEventBinding;
    [SerializeField] private EmotionManager emotionManager;

    private void Awake()
    {
        npcGenerator = GetComponent<NPC_Genearted>();
        npcEmotionEventBinding = new EventBinding<NPCEmotionEvent>(OnNPCEmotion);
        playerInputEventBinding = new EventBinding<PlayerInputEvent>(OnPlayerInput);
    }
    private void OnEnable()
    {
        npcGenerator.NPC_EventBus.Register<NPCEmotionEvent>(npcEmotionEventBinding);
        EventBus<PlayerInputEvent>.Register(playerInputEventBinding);
    }
    private void OnDisable()
    {
        npcGenerator.NPC_EventBus.Deregister<NPCEmotionEvent>(npcEmotionEventBinding);
        EventBus<PlayerInputEvent>.Deregister(playerInputEventBinding);
    }

    private void OnNPCEmotion(NPCEmotionEvent e)
    {
        if (e.npc != gameObject)
        {
            return;
        }

        Debug.Log($"收到Emotion事件：{e.emotionName}");

        if (e.emotionName == "None")
        {
            return;
        }

        emotionManager.ShowEmotion(e.emotionName);
    }
    private void OnPlayerInput(PlayerInputEvent e)
    {
        if (e.target != gameObject)
        {
            return;
        }

        emotionManager.ShowEmotion("Think");
    }
}
