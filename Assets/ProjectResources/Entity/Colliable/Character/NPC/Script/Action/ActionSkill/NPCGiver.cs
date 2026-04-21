using UnityEngine;
// using GameStatusSystem.PlayerStatus.Events;
using NPCActionEvents;

public class NPCGiver : MonoBehaviour {
    private NPC_Genearted npcGenearted;

    [Header("物品预制体")]
    public GameObject coin_prefab;
    public GameObject potion_prefab;

    private EventBinding<NPCGivePotionEvent> _givePotionEventBinding;
    private EventBinding<NPCGiveCoinEvent> _giveCoinEventBinding;
    private void Awake()
    {
        _givePotionEventBinding = new EventBinding<NPCGivePotionEvent>(HandleGivePotion);
        _giveCoinEventBinding = new EventBinding<NPCGiveCoinEvent>(HandleGiveCoin);
        npcGenearted = GetComponent<NPC_Genearted>();
    }
    private void OnEnable()
    {
        npcGenearted.NPC_EventBus.Register(_givePotionEventBinding);
        npcGenearted.NPC_EventBus.Register(_giveCoinEventBinding);
    }
    private void OnDisable()
    {
        npcGenearted.NPC_EventBus.Deregister(_givePotionEventBinding);
        npcGenearted.NPC_EventBus.Deregister(_giveCoinEventBinding);
    }

    private void HandleGivePotion(NPCGivePotionEvent e)
    {
        Debug.Log($"Give potion");
        if (e.npc != gameObject)
        {
            return;
        }
        Instantiate(potion_prefab, GenerateRandomPosition(), Quaternion.identity);
    }
    private void HandleGiveCoin(NPCGiveCoinEvent e)
    {
        Debug.Log($"Give coin {e.amount}");
        if (e.npc != gameObject)
        {
            return;
        }
        for (int i = 0; i < e.amount; i++)
        {
            GameObject coin = Instantiate(coin_prefab, GenerateRandomPosition(), Quaternion.identity);
        }
    }

    // 生成附近的随机位置
    private Vector3 GenerateRandomPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * 2f;
        randomPosition.z = gameObject.transform.position.z;

        return transform.position + randomPosition;
    }
}


