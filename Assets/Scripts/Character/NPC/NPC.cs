using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC状态枚举
/// </summary>
public enum NPCState
{
    Standing, // 站立
    Moving    // 移动
}

/// <summary>
/// NPC基类 - 游戏中的非玩家角色
/// - 继承自Collidable，包含碰撞检测功能
/// - 实现与玩家的交互逻辑
/// - 实现简单的移动AI
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class NPC : Collidable
{
    /// <summary>
    /// NPC的对话数据
    /// - 存储NPC的对话内容
    /// </summary>
    public Dialog dialog;

    [Header("移动AI配置")]
    public float moveSpeed = 1f; // 移动速度
    public float maxMoveDistance = 5f; // 最大移动距离
    public float moveWaitTime = 2f; // 移动后等待时间
    public float moveProbability = 0.3f; // 移动概率

    /// <summary>
    /// NPC是否可交互
    /// - true: 玩家可以与NPC交互
    /// - false: 玩家无法与NPC交互
    /// </summary>
    protected bool interacble = false;

    // 移动AI相关字段
    private NPCState currentState = NPCState.Standing;
    private Vector3 targetPosition;
    private float waitTimer = 0f;
    private SpriteRenderer spriteRenderer;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.interacble = true;
            // 与玩家互动时进入站立状态
            currentState = NPCState.Standing;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.interacble = false;
        }
    }

    protected virtual void Start()
    {
        // 初始化 spriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // protected override void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         this.interacble = true;
    //         // 与玩家互动时进入站立状态
    //         currentState = NPCState.Standing;
    //     }
    // }

    // protected override void OnCollisionExit2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         this.interacble = false;
    //     }
    // }

    protected virtual void Update()
    {
        // 处理交互
        if (Input.GetKeyDown("space"))
        {
            if (this.interacble)
            {
                DialogManager.instance.ShowDialog(dialog);
                // 交互时进入站立状态
                currentState = NPCState.Standing;
                // 朝向玩家
                FacePlayer();
            }
        }

        // 只有在可交互时不处理移动AI
        if (!interacble)
        {
            // 处理移动AI
            HandleAI();
        }
        else
        {
            // 朝向玩家
            FacePlayer();
        }
    }

    /// <summary>
    /// 朝向玩家
    /// </summary>
    private void FacePlayer()
    {
        if (Player.instance != null && spriteRenderer != null)
        {
            Vector3 playerPosition = Player.instance.transform.position;
            Vector3 direction = playerPosition - transform.position;
            
            // 只考虑x轴方向
            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    /// <summary>
    /// 处理AI逻辑
    /// </summary>
    private void HandleAI()
    {
        switch (currentState)
        {
            case NPCState.Standing:
                HandleStandingState();
                break;
            case NPCState.Moving:
                HandleMovingState();
                break;
        }
    }

    /// <summary>
    /// 处理站立状态
    /// </summary>
    private void HandleStandingState()
    {
        waitTimer += Time.deltaTime;
        
        // 随机决定是否开始移动
        if (waitTimer >= moveWaitTime && Random.value < moveProbability)
        {
            // 选择随机目标点
            targetPosition = GetRandomTargetPosition();
            currentState = NPCState.Moving;
            waitTimer = 0f;
        }
    }

    /// <summary>
    /// 处理移动状态
    /// </summary>
    private void HandleMovingState()
    {
        // 计算移动方向
        Vector3 direction = targetPosition - transform.position;
        float distance = direction.magnitude;
        
        // 检查是否到达目标点
        if (distance < 0.1f)
        {
            // 到达目标点，进入站立状态
            currentState = NPCState.Standing;
            waitTimer = 0f;
            return;
        }
        
        // 确保只沿着x或y轴移动
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // 沿x轴移动
            direction = new Vector3(Mathf.Sign(direction.x), 0, 0);
            
            // 反转sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = direction.x < 0;
            }
        }
        else
        {
            // 沿y轴移动
            direction = new Vector3(0, Mathf.Sign(direction.y), 0);
        }
        
        // 移动
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 获取随机目标位置
    /// </summary>
    /// <returns>随机目标位置</returns>
    private Vector3 GetRandomTargetPosition()
    {
        // 在当前位置附近随机选择一个目标点
        float randomX = Random.Range(-maxMoveDistance, maxMoveDistance);
        float randomY = Random.Range(-maxMoveDistance, maxMoveDistance);
        
        // 确保只沿着x或y轴移动
        if (Random.value > 0.5f)
        {
            // 沿x轴移动
            return new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z);
        }
        else
        {
            // 沿y轴移动
            return new Vector3(transform.position.x, transform.position.y + randomY, transform.position.z);
        }
    }
}
