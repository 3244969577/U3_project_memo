using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class NPC : Collidable
{

    [Header("对话组件")]
    public NPCDialogue npcDialogue; // 挂载在角色上
    public NPCDialogueConfig npcDialogueConfig; // 挂载在子物体上
    public Fungus.Character fungusCharacter; // 挂载在子物体上

    [Header("移动设置")]
    public bool enableMovement = true; // 是否启用移动AI

    // 移动AI组件
    private NPCMovementAI movementAI;

    protected virtual void Start()
    {
        // 自动获取对话组件
        npcDialogue = GetComponent<NPCDialogue>();
        
        // 从子物体获取配置和Fungus角色
        npcDialogueConfig = GetComponentInChildren<NPCDialogueConfig>();
        fungusCharacter = GetComponentInChildren<Fungus.Character>();
        
        // 初始化移动AI
        if (enableMovement)
        {
            movementAI = GetComponent<NPCMovementAI>();
            if (movementAI == null)
            {
                movementAI = gameObject.AddComponent<NPCMovementAI>();
            }
        }

    }

    protected virtual void Update()
    {
        // 只有在启用移动且没有交互时处理移动AI
        if (enableMovement)
        {
            // movementAI?.HandleAI();
        }
    }
    
    /// <summary>
    /// 启用或禁用移动
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public void SetMovementEnabled(bool enabled)
    {
        enableMovement = enabled;
        if (enabled && movementAI == null)
        {
            movementAI = GetComponent<NPCMovementAI>();
            if (movementAI == null)
            {
                movementAI = gameObject.AddComponent<NPCMovementAI>();
            }
        }
    }
    
    public void EnableMovement()
    {
        SetMovementEnabled(true);
    }
    
    public void DisableMovement()
    {
        SetMovementEnabled(false);
    }
    
    /// <summary>
    /// 检查是否启用移动
    /// </summary>
    /// <returns>是否启用移动</returns>
    public bool IsMovementEnabled()
    {
        return enableMovement;
    }
    
    /// <summary>
    /// 强制进入站立状态
    /// </summary>
    public void ForceStandingState()
    {
        if (enableMovement)
        {
            movementAI?.ForceStandingState();
        }
    }
    
    /// <summary>
    /// 朝向玩家
    /// </summary>
    public void FacePlayer()
    {
        if (enableMovement)
        {
            movementAI?.FacePlayer();
        }
    }
}
