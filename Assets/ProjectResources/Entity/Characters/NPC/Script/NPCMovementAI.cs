using UnityEngine;
using NPCActionEvents;

/// <summary>
/// NPC移动AI组件
/// - 处理NPC的移动逻辑
/// - 支持多种移动模式
/// - 集成动画控制器
/// </summary>
public class NPCMovementAI : MonoBehaviour
{
    [Header("基础移动设置")]
    public float moveSpeed = 2f;       // 移动速度
    public float moveRange = 1.5f;     // 每次移动的范围半径
    public float stopDistance = 0.1f;   // 停止距离

    [Header("行为间隔设置")]
    public float minIdleTime = 2f;    // 最小待机时间
    public float maxIdleTime = 5f;    // 最大待机时间

    [Header("移动模式设置")]
    public float followDistance = 2f; // 跟随距离（大于此距离时开始跟随）
    public float followStopDistance = 1f; // 跟随停止距离（小于此距离时停止跟随）
    public float avoidDistance = 10f; // 远离距离（大于此距离时停止远离）
    public float avoidStopDistance = 5f; // 远离安全距离（小于此距离时继续远离）

    /// <summary>
    /// 移动模式枚举
    /// </summary>
    public enum MovementMode
    {
        Standby,      // 站立等待
        RandomMove,   // 随机移动
        FollowTarget, // 跟随目标
        AvoidTarget   // 远离目标
    }

    // 动画控制器（解耦引用）
    private NPC4DirFrameAnimator _animator;

    // 移动AI相关字段
    private MovementMode currentMode = MovementMode.Standby;
    private Vector3 targetPosition;   // 目标位置
    private float stateTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private GameObject currentTarget;
    private LocalEventBus npcEventBus;

    /// <summary>
    /// 当前移动模式
    /// </summary>
    public MovementMode CurrentMode => currentMode;

    /// <summary>
    /// 是否正在移动
    /// </summary>
    public bool IsMoving => currentMode != MovementMode.Standby;

    private void Awake()
    {
        // 初始化动画控制器
        _animator = GetComponent<NPC4DirFrameAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 为每个 NPC 初始化不同的随机种子，避免所有 NPC 朝同一方向移动
        // 使用物体的实例ID和当前时间作为种子
        int seed = (int)(gameObject.GetInstanceID() + Time.time * 1000f);
        Random.InitState(seed);

        // 初始状态：站立等待
        SetMode(MovementMode.Standby);
    }

    private void Start()
    {
        // 获取 NPC_Genearted 组件的局部事件总线
        NPC_Genearted npcGenerated = GetComponent<NPC_Genearted>();
        if (npcGenerated != null)
        {
            npcEventBus = npcGenerated.NPC_EventBus;
            RegisterEventHandlers();
        }
    }

    private void Update()
    {
        stateTimer += Time.deltaTime;

        // 根据当前模式更新目标位置
        UpdateTargetPosition();

        // 计算移动方向和距离
        Vector3 moveDir = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // 检查是否到达目标点或指定距离
        if (distanceToTarget <= stopDistance)
        {
            // 到达目标点，处理不同模式的逻辑
            HandleReachTarget();
            // 不执行移动，保持移动状态
            return;
        }

        // 执行移动
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        // 更新动画和朝向
        UpdateAnimationAndFacing(moveDir);
    }

    /// <summary>
    /// 注册事件处理器
    /// </summary>
    private void RegisterEventHandlers()
    {
        if (npcEventBus != null)
        {
            // 注册逃离事件处理
            npcEventBus.Register<NPCEscapeEvent>(new EventBinding<NPCEscapeEvent>(HandleNPCEscapeEvent));

            // 注册待机事件处理
            npcEventBus.Register<NPCStandbyEvent>(new EventBinding<NPCStandbyEvent>(HandleNPCStandbyEvent));

            // 注册跟随事件处理
            npcEventBus.Register<NPCFollowEvent>(new EventBinding<NPCFollowEvent>(HandleNPCFollowEvent));
        }
    }

    /// <summary>
    /// 处理逃离事件
    /// </summary>
    private void HandleNPCEscapeEvent(NPCEscapeEvent e)
    {
        if (e.npc == gameObject)
        {
            SetMode(MovementMode.AvoidTarget, e.target);
            Debug.Log($"NPC {gameObject.name} 切换到逃离模式，目标：{e.target.name}");
        }
    }

    /// <summary>
    /// 处理待机事件
    /// </summary>
    private void HandleNPCStandbyEvent(NPCStandbyEvent e)
    {
        if (e.npc == gameObject)
        {
            SetMode(MovementMode.Standby);
            Debug.Log($"NPC {gameObject.name} 切换到待机模式");
        }
    }

    /// <summary>
    /// 处理跟随事件
    /// </summary>
    private void HandleNPCFollowEvent(NPCFollowEvent e)
    {
        if (e.npc == gameObject)
        {
            SetMode(MovementMode.FollowTarget, e.target);
            Debug.Log($"NPC {gameObject.name} 切换到跟随模式，目标：{e.target.name}");
        }
    }

    /// <summary>
    /// 设置移动模式
    /// </summary>
    /// <param name="mode">移动模式</param>
    /// <param name="target">目标对象（可选）</param>
    public void SetMode(MovementMode mode, GameObject target = null)
    {
        currentMode = mode;
        currentTarget = target;
        stateTimer = 0f;

        // 立即更新目标位置
        UpdateTargetPosition();

        // 更新动画状态
        if (_animator != null)
        {
            _animator.SetMoveState(mode != MovementMode.Standby);
        }
    }

    /// <summary>
    /// 更新目标位置
    /// </summary>
    private void UpdateTargetPosition()
    {
        switch (currentMode)
        {
            case MovementMode.Standby:
                // 站立等待：将自己的位置作为目标点
                targetPosition = transform.position;
                break;

            case MovementMode.RandomMove:
                // 随机移动：在周围随机找一点作为目标点
                // 只有当距离当前目标点较近时才重新选择目标点
                if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
                {
                    Vector3 moveDir = GetRandomDirection() * moveRange;
                    targetPosition = transform.position + moveDir;
                    Debug.Log($"NPC {gameObject.name} 随机移动到：{targetPosition}");
                }
                break;

            case MovementMode.FollowTarget:
                // 跟随：将目标的位置作为目标点
                if (currentTarget != null)
                {
                    targetPosition = currentTarget.transform.position;
                }
                else
                {
                    // 目标不存在，切换回随机移动
                    SetMode(MovementMode.RandomMove);
                }
                break;

            case MovementMode.AvoidTarget:
                // 远离：以目标点相反方向选一点作为目标点
                if (currentTarget != null)
                {
                    Vector3 awayDir = (transform.position - currentTarget.transform.position).normalized;
                    targetPosition = transform.position + awayDir * moveRange;
                }
                else
                {
                    // 目标不存在，切换回随机移动
                    SetMode(MovementMode.RandomMove);
                }
                break;
        }
    }

    /// <summary>
    /// 处理到达目标点的逻辑
    /// </summary>
    private void HandleReachTarget()
    {
        switch (currentMode)
        {
            case MovementMode.Standby:
                // 站立等待模式：保持当前位置
                break;

            case MovementMode.RandomMove:
                // 随机移动模式：等待一段时间后重新选择目标点
                if (stateTimer >= GetRandomIdleTime())
                {
                    // 重新选择随机目标点
                    Vector3 moveDir = GetRandomDirection() * moveRange;
                    targetPosition = transform.position + moveDir;
                    Debug.Log($"NPC {gameObject.name} 随机移动到：{targetPosition}");
                    stateTimer = 0f;
                }
                break;

            case MovementMode.FollowTarget:
                // 跟随模式：检查距离，如果太近则停止跟随
                if (currentTarget != null)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
                    if (distanceToTarget <= followStopDistance)
                    {
                        // 距离太近，停止跟随
                        // SetMode(MovementMode.Standby);
                        Debug.Log($"NPC {gameObject.name} 已接近目标，停止跟随");
                    }
                }
                break;

            case MovementMode.AvoidTarget:
                // 远离模式：检查距离，如果足够远则停止远离
                if (currentTarget != null)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
                    if (distanceToTarget >= avoidDistance)
                    {
                        // 已经远离到安全距离，切换回随机移动
                        SetMode(MovementMode.RandomMove);
                        Debug.Log($"NPC {gameObject.name} 已远离目标，切换回随机移动");
                    }
                    else if (distanceToTarget <= avoidStopDistance)
                    {
                        // 距离太近，继续远离
                        Vector3 awayDir = (transform.position - currentTarget.transform.position).normalized;
                        targetPosition = transform.position + awayDir * moveRange;
                        Debug.Log($"NPC {gameObject.name} 继续远离目标");
                    }
                }
                break;
        }

        // 更新动画状态
        if (_animator != null)
        {
            _animator.SetMoveState(currentMode != MovementMode.Standby);
        }
    }

    /// <summary>
    /// 更新动画和朝向
    /// </summary>
    private void UpdateAnimationAndFacing(Vector3 moveDir)
    {
        // 跟随和远离模式：始终朝向目标
        if (currentMode == MovementMode.FollowTarget || currentMode == MovementMode.AvoidTarget)
        {
            FaceTarget();
        }
        else if (currentMode == MovementMode.RandomMove)
        {
            // 随机移动模式：根据移动方向设置朝向
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = moveDir.x < 0;
            }
        }

        // 播放移动动画
        if (_animator != null)
        {
            int animDir = GetAnimationDirection(moveDir);
            _animator.SetMoveState(true, animDir);
        }
    }

    /// <summary>
    /// 朝向目标
    /// </summary>
    private void FaceTarget()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.transform.position - transform.position;

        // 只考虑x轴方向
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    /// <summary>
    /// 获取随机待机时间
    /// </summary>
    private float GetRandomIdleTime() => Random.Range(minIdleTime, maxIdleTime);

    /// <summary>
    /// 获取随机移动方向
    /// </summary>
    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
    }

    /// <summary>
    /// 转换移动方向为动画方向
    /// 0=前(y<0) / 1=左(x<0) / 2=右(x>0) / 3=后(y>0)
    /// </summary>
    private int GetAnimationDirection(Vector3 moveDir)
    {
        if (Mathf.Abs(moveDir.y) > Mathf.Abs(moveDir.x))
        {
            // 优先上下方向
            return moveDir.y < 0 ? 0 : 3;
        }
        else
        {
            // 左右方向
            return moveDir.x < 0 ? 1 : 2;
        }
    }

    #region 外部调用接口
    /// <summary>
    /// 强制停止NPC所有行为
    /// </summary>
    public void StopAllBehaviour()
    {
        SetMode(MovementMode.Standby);
        enabled = false;
    }

    /// <summary>
    /// 恢复NPC行为
    /// </summary>
    public void ResumeBehaviour()
    {
        enabled = true;
        SetMode(MovementMode.Standby);
    }

    /// <summary>
    /// 强制进入站立状态
    /// </summary>
    public void ForceStandingState()
    {
        SetMode(MovementMode.Standby);
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
    #endregion
}
