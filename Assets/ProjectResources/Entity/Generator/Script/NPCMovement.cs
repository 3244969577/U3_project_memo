using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("基础移动设置")]
    public float moveSpeed = 2f;       // 移动速度
    public float moveRange = 1.5f;     // 每次移动的范围半径

    [Header("行为间隔设置")]
    public float minIdleTime = 2f;    // 最小待机时间
    public float maxIdleTime = 5f;    // 最大待机时间
    public float minMoveTime = 0.5f;  // 最小移动时间
    public float maxMoveTime = 3f;  // 最大移动时间

    // 动画控制器（解耦引用）
    private NPC4DirFrameAnimator _animator;
    // NPC行为状态
    private enum NPCState { Idle, Moving }
    private NPCState _currentState;

    // 行为计时器与目标
    private float _stateTimer;
    private Vector3 _startPos;        // NPC初始原点
    private Vector3 _moveTargetPos;   // 随机移动目标点

    private void Awake()
    {
        _animator = GetComponent<NPC4DirFrameAnimator>();
        _startPos = transform.position;
        
        // 为每个 NPC 初始化不同的随机种子，避免所有 NPC 朝同一方向移动
        // 使用物体的实例ID和当前时间作为种子
        int seed = (int)(gameObject.GetInstanceID() + Time.time * 1000f);
        Random.InitState(seed);
        
        // 初始状态：待机
        SwitchToIdle();
    }

    private void Update()
    {
        _stateTimer += Time.deltaTime;

        // 状态机逻辑
        switch (_currentState)
        {
            case NPCState.Idle:
                if (_stateTimer >= GetRandomIdleTime())
                {
                    // 待机结束 → 开始移动
                    SwitchToMoving();
                }
                break;

            case NPCState.Moving:
                if (_stateTimer >= GetRandomMoveTime())
                {
                    // 移动结束 → 回到待机
                    SwitchToIdle();
                }
                else
                {
                    // 执行移动
                    DoRandomMove();
                }
                break;
        }
    }

    #region 核心状态切换
    /// <summary>
    /// 切换到待机状态
    /// </summary>
    private void SwitchToIdle()
    {
        _currentState = NPCState.Idle;
        _stateTimer = 0f;
        // 通知动画：停止移动，待机
        _animator.SetMoveState(false);
    }

    /// <summary>
    /// 切换到移动状态
    /// </summary>
    private void SwitchToMoving()
    {
        _currentState = NPCState.Moving;
        _stateTimer = 0f;
        // 设置随机移动目标点（基于当前位置，而不是固定的初始位置）
        Vector3 moveDir = GetRandomDirection() * moveRange;
        Debug.Log($"NPC: {gameObject.name} 随机移动方向：{moveDir}");
        _moveTargetPos = transform.position + moveDir;
    }
    #endregion

    #region 移动执行
    /// <summary>
    /// 执行随机移动逻辑
    /// </summary>
    private void DoRandomMove()
    {
        // 计算移动方向
        Vector3 moveDir = (_moveTargetPos - transform.position).normalized;
        
        // 接近目标点 → 提前结束移动
        if (Vector3.Distance(transform.position, _moveTargetPos) < 0.1f)
        {
            SwitchToIdle();
            return;
        }

        // 执行移动
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        // 通知动画：正在移动 + 更新方向
        int animDir = GetAnimationDirection(moveDir);
        _animator.SetMoveState(true, animDir);
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
    /// 转换移动方向为动画方向(0=前/1=左/2=右/3=后)
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
    #endregion
}

