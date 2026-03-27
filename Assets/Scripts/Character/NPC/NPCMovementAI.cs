using UnityEngine;

/// <summary>
/// NPC移动AI组件
/// - 处理NPC的随机移动逻辑
/// - 支持站立和移动状态
/// - 可配置移动参数
/// </summary>
public class NPCMovementAI : MonoBehaviour
{
    [Header("移动AI配置")]
    public float moveSpeed = 1f; // 移动速度
    public float maxMoveDistance = 5f; // 最大移动距离
    public float moveWaitTime = 2f; // 移动后等待时间
    public float moveProbability = 0.3f; // 移动概率
    
    /// <summary>
    /// NPC状态枚举
    /// </summary>
    public enum NPCState
    {
        Standing, // 站立
        Moving    // 移动
    }
    
    // 移动AI相关字段
    private NPCState currentState = NPCState.Standing;
    private Vector3 targetPosition;
    private float waitTimer = 0f;
    private SpriteRenderer spriteRenderer;
    
    /// <summary>
    /// 当前状态
    /// </summary>
    public NPCState CurrentState => currentState;
    
    /// <summary>
    /// 是否正在移动
    /// </summary>
    public bool IsMoving => currentState == NPCState.Moving;
    
    private void Start()
    {
        // 初始化 spriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    /// <summary>
    /// 处理AI逻辑
    /// </summary>
    public void HandleAI()
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
    
    /// <summary>
    /// 强制进入站立状态
    /// </summary>
    public void ForceStandingState()
    {
        currentState = NPCState.Standing;
        waitTimer = 0f;
    }
    
    /// <summary>
    /// 朝向玩家
    /// </summary>
    public void FacePlayer()
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
}
