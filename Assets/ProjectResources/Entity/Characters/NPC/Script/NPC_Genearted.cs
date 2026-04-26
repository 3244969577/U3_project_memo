using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class NPC_Genearted : Collidable
{
    public LocalEventBus NPC_EventBus { get; private set; } = new LocalEventBus();

    [Header("对话组件")]
    public NPCDialogue npcDialogue; // 挂载在角色上
    public NPCDialogueConfig npcDialogueConfig; // 挂载在子物体上
    public Fungus.Character fungusCharacter; // 挂载在子物体上

    private NPCMovementAI npcMovement;

    private void Awake()
    {
        // 自动获取对话组件
        npcDialogue = GetComponent<NPCDialogue>();    
        // 从子物体获取配置和Fungus角色
        npcDialogueConfig = GetComponentInChildren<NPCDialogueConfig>();
        fungusCharacter = GetComponentInChildren<Fungus.Character>();
        npcMovement = GetComponent<NPCMovementAI>();
    }

    // 根据对应组件的定义，为外界提供两个接口，用于控制NPC移动开关
    public void EnableMovement()
    {
        if (npcMovement != null)
        {
            npcMovement.ResumeBehaviour();
        }
    }
    public void DisableMovement()
    {
        if (npcMovement != null)
        {
            npcMovement.StopAllBehaviour();
        }
    }

}
