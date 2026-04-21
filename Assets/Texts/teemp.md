using UnityEngine;
using NPCActionEvents;

/// <summary>
/// NPC移动AI组件
/// - 处理NPC的随机移动逻辑
/// - 支持站立和移动状态
/// - 可配置移动参数
/// - 支持多种移动模式切换
/// - 集成动画控制器
/// </summary>
public class NPCMovementAI : MonoBehaviour
{
    [Header("基础移动设置")]
    public float moveSpeed = 2f;       // 移动速度
    public float moveRange = 1.5f;     // 每次移动的范围半径

    [Header("行为间隔设置")]
    public float minIdleTime = 2f;    // 最小待机时间
    public float maxIdleTime = 5f;    // 最大待机时间
    public float minMoveTime = 0.5f;  // 最小移动时间
    public float maxMoveTime = 3f;  // 最大移动时间

    [Header("移动模式设置")]
    public float followDistance = 2f; // 跟随距离（大于此距离时开始跟随）
    public float followStopDistance = 1f; // 跟随停止距离（小于此距离时停止跟随）
    public float avoidDistance = 10f; // 远离距离（大于此距离时停止远离）
    public float avoidStopDistance = 5f; // 远离安全距离（小于此距离时继续远离）

    /// <summary>
    /// NPC状态枚举
    /// </summary>
    public enum NPCState
    {
        Idle,    // 待机
        Moving   // 移动
    }

    /// <summary>
    /// 移动模式枚举
    /// </summary>
    public enum MovementMode
    {
        RandomMove,  // 随机移动
        FollowTarget,  // 跟随目标
        AvoidTarget  // 远离目标
    }

    // 动画控制器（解耦引用）
    private NPC4DirFrameAnimator _animator;

    // 移动AI相关字段
    private NPCState currentState = NPCState.Idle;
    private MovementMode currentMode = MovementMode.RandomMove;
    private Vector3 targetPosition;
    private float stateTimer = 0f;
    private Vector3 startPos;        // NPC初始原点
    private Vector3 moveTargetPos;   // 随机移动目标点
    private SpriteRenderer spriteRenderer;
    private GameObject currentTarget;
    private LocalEventBus npcEventBus;

    /// <summary>
    /// 当前状态
    /// </summary>
    public NPCState CurrentState => currentState;

    /// <summary>
    /// 当前移动模式
    /// </summary>
    public MovementMode CurrentMode => currentMode;

    /// <summary>
    /// 是否正在移动
    /// </summary>
    public bool IsMoving => currentState == NPCState.Moving;

    private void Awake()
    {
        // 初始化动画控制器
        _animator = GetComponent<NPC4DirFrameAnimator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;

        // 为每个 NPC 初始化不同的随机种子，避免所有 NPC 朝同一方向移动
        // 使用物体的实例ID和当前时间作为种子
        int seed = (int)(gameObject.GetInstanceID() + Time.time * 1000f);
        Random.InitState(seed);

        // 初始状态：待机
        SwitchToIdle();
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

        // 状态机逻辑
        switch (currentState)
        {
            case NPCState.Idle:
                if (stateTimer >= GetRandomIdleTime())
                {
                    // 待机结束 → 开始移动
                    SwitchToMoving();
                }
                break;

            case NPCState.Moving:
                // 检查是否应该停止移动（仅对随机移动模式生效）
                if (currentMode == MovementMode.RandomMove)
                {
                    if (stateTimer >= GetRandomMoveTime())
                    {
                        // 移动时间结束 → 回到待机
                        SwitchToIdle();
                    }
                    else
                    {
                        // 执行移动
                        DoMove();
                    }
                }
                else
                {
                    // 跟随和远离模式：持续移动，不受时间限制
                    DoMove();
                }
                break;
        }
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
            currentMode = MovementMode.AvoidTarget;
            currentTarget = e.target;
            SwitchToMoving();
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
            currentMode = MovementMode.RandomMove;
            currentTarget = null;
            SwitchToIdle();
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
            currentMode = MovementMode.FollowTarget;
            currentTarget = e.target;
            SwitchToMoving();
            Debug.Log($"NPC {gameObject.name} 切换到跟随模式，目标：{e.target.name}");
        }
    }

    #region 核心状态切换
    /// <summary>
    /// 切换到待机状态
    /// </summary>
    private void SwitchToIdle()
    {
        currentState = NPCState.Idle;
        stateTimer = 0f;
        // 通知动画：停止移动，待机
        if (_animator != null)
        {
            _animator.SetMoveState(false);
        }
    }

    /// <summary>
    /// 切换到移动状态
    /// </summary>
    private void SwitchToMoving()
    {
        currentState = NPCState.Moving;
        stateTimer = 0f;

        // 根据当前模式设置移动目标
        switch (currentMode)
        {
            case MovementMode.RandomMove:
                // 随机移动：基于当前位置，而不是固定的初始位置
                Vector3 moveDir = GetRandomDirection() * moveRange;
                Debug.Log($"NPC: {gameObject.name} 随机移动方向：{moveDir}");
                moveTargetPos = transform.position + moveDir;
                break;

            case MovementMode.FollowTarget:
                // 跟随模式：直接设置目标位置为玩家位置
                if (currentTarget != null)
                {
                    moveTargetPos = currentTarget.transform.position;
                }
                else
                {
                    currentMode = MovementMode.RandomMove;
                    moveDir = GetRandomDirection() * moveRange;
                    moveTargetPos = transform.position + moveDir;
                }
                break;

            case MovementMode.AvoidTarget:
                // 逃离模式：设置远离目标的位置
                if (currentTarget != null)
                {
                    Vector3 awayDir = (transform.position - currentTarget.transform.position).normalized;
                    moveTargetPos = transform.position + awayDir * moveRange;
                }
                else
                {
                    currentMode = MovementMode.RandomMove;
                    moveDir = GetRandomDirection() * moveRange;
                    moveTargetPos = transform.position + moveDir;
                }
                break;
        }
    }
    #endregion

    #region 移动执行
    /// <summary>
    /// 执行移动逻辑
    /// </summary>
    private void DoMove()
    {
        // 跟随和远离模式：实时更新目标位置
        if (currentMode == MovementMode.FollowTarget || currentMode == MovementMode.AvoidTarget)
        {
            UpdateTargetPosition();
        }

        // 计算移动方向
        Vector3 moveDir = (moveTargetPos - transform.position).normalized;

        // 随机移动模式：检查是否到达目标点
        if (currentMode == MovementMode.RandomMove)
        {
            if (Vector3.Distance(transform.position, moveTargetPos) < 0.1f)
            {
                SwitchToIdle();
                return;
            }
        }

        // 执行移动
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        // 更新动画和朝向
        UpdateAnimationAndFacing(moveDir);
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
            // 跟随和远离模式也需要显示移动动画
            if (_animator != null)
            {
                int animDir = GetAnimationDirection(moveDir);
                _animator.SetMoveState(true, animDir);
            }
        }
        else
        {
            // 随机移动模式：使用原有的动画方向
            if (_animator != null)
            {
                int animDir = GetAnimationDirection(moveDir);
                _animator.SetMoveState(true, animDir);
            }
        }
    }

    /// <summary>
    /// 更新跟随和远离模式的目标位置
    /// </summary>
    private void UpdateTargetPosition()
    {
        if (currentTarget == null)
        {
            // 目标不存在，切换回随机移动
            currentMode = MovementMode.RandomMove;
            Vector3 moveDir = GetRandomDirection() * moveRange;
            moveTargetPos = transform.position + moveDir;
            return;
        }

        Vector3 toTarget = currentTarget.transform.position - transform.position;
        float distanceToTarget = toTarget.magnitude;

        if (currentMode == MovementMode.FollowTarget)
        {
            // 跟随模式逻辑
            if (distanceToTarget <= followStopDistance)
            {
                // 距离太近，停止跟随
                SwitchToIdle();
                return;
            }
            else if (distanceToTarget > followDistance)
            {
                // 距离拉远，继续跟随
                moveTargetPos = currentTarget.transform.position;
            }
        }
        else if (currentMode == MovementMode.AvoidTarget)
        {
            // 逃离模式逻辑
            if (distanceToTarget >= avoidDistance)
            {
                // 已经远离到安全距离，切换回随机移动
                currentMode = MovementMode.RandomMove;
                currentTarget = null;
                Vector3 moveDir = GetRandomDirection() * moveRange;
                moveTargetPos = transform.position + moveDir;
                Debug.Log($"NPC {gameObject.name} 已远离目标，切换回随机移动");
                return;
            }
            else if (distanceToTarget <= avoidStopDistance)
            {
                // 距离太近，继续逃离
                Vector3 awayDir = (transform.position - currentTarget.transform.position).normalized;
                moveTargetPos = transform.position + awayDir * moveRange;
            }
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
    /// 获取随机移动时间
    /// </summary>
    private float GetRandomMoveTime() => Random.Range(minMoveTime, maxMoveTime);

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
            // 由于 flipX 会翻转 SpriteRenderer，左右方向的判断需要和 flipX 一致
            // flipX = direction.x < 0 表示向左看
            // 所以向左移动(x<0)时返回 1，向右移动(x>=0)时返回 2
            return moveDir.x < 0 ? 1 : 2;
        }
    }
    #endregion

    #region 外部调用接口
    /// <summary>
    /// 强制停止NPC所有行为
    /// </summary>
    public void StopAllBehaviour()
    {
        SwitchToIdle();
        enabled = false;
    }

    /// <summary>
    /// 恢复NPC行为
    /// </summary>
    public void ResumeBehaviour()
    {
        enabled = true;
        SwitchToIdle();
    }

    /// <summary>
    /// 强制进入站立状态
    /// </summary>
    public void ForceStandingState()
    {
        SwitchToIdle();
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
