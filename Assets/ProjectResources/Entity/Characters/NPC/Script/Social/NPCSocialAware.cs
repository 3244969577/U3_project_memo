using UnityEngine;
using NPCSocialEvents;
using GlobalEvents;
using NPCActionEvents;
public class NPCSocialAware : MonoBehaviour {
    private NPC_Genearted npcGenearted;
    private LocalEventBus localEventBus;
    [SerializeField] private GameObject npc;
    
    private void Awake()
    {
        npcGenearted = npc.GetComponent<NPC_Genearted>();
        localEventBus = npcGenearted.NPC_EventBus;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.GetComponent<NPCSocialAware>())
        {
            GameObject target = collision.transform.parent.gameObject;
            Debug.Log($"SocialAware: {npc.name} 发现了 {target.name}");
            EventBus<NPCAwareEvent>.Raise(
                new NPCAwareEvent {
                    npc = npc,
                    target = target
                }
            );
            HandleAware(target);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            EventBus<NPCAwareEvent>.Raise(
                new NPCAwareEvent {
                    npc = npc,
                    target = player
                }
            );
            HandleAware(player);
        }
    }

    private void HandleAware(GameObject target)
    {
        int socialRelation = NPCSocialManager.Instance.GetSocialRelationOf(npc.name, target.name);
        Debug.Log($"SocialAware: {npc.name} 对 {target.name} 的好感度为 {socialRelation}");
        if (socialRelation <= -50)
        {
            // 敌对关系
            npcGenearted.NPC_EventBus.Raise<NPCEscapeEvent>(
                new NPCEscapeEvent {
                    npc = npc,
                    target = target
                }
            );
            npcGenearted.NPC_EventBus.Raise<NPCEmotionEvent>(
                new NPCEmotionEvent {
                    npc = npc,
                    emotionName = "AngryGrumpy"
                }
            );
        } 
        else if (socialRelation >= 50)
        {
            // 友好关系
            npcGenearted.NPC_EventBus.Raise<NPCFollowEvent>(
                new NPCFollowEvent {
                    npc = npc,
                    target = target
                }
            );
            npcGenearted.NPC_EventBus.Raise<NPCEmotionEvent>(
                new NPCEmotionEvent {
                    npc = npc,
                    emotionName = "NaughtyTongue"
                }
            );
        }
        else
        {
            // 普通关系
        }
    }

    // private void OnTriggerEnter(Collider collider)
    // {
    //     Debug.Log($"SocialAware: {collider.gameObject.name} 进入了NPC {npc.name}的范围");
    // }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log($"SocialAware: {collision.gameObject.name} 进入了NPC {npc.name}的范围");
    // }
    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     Debug.Log($"SocialAware: {collision.gameObject.name} 进入了NPC {npc.name}的范围");
    // }
}