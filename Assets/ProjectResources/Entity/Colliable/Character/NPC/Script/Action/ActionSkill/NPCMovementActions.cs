using UnityEngine;
using NPCActionEvents;

public class NPCMovementActions : MonoBehaviour
{
    private NPC_Genearted _npcGenearted;
    private NPCMovementController _movementController;

    [Header("移动设置")]
    public float followDistance = 2f;        // 跟随距离
    public float escapeDistance = 5f;        // 逃离距离
    public float randomMoveRadius = 8f;      // 随机移动半径

    private GameObject _player;              // 玩家对象
    private GameObject _currentTarget;       // 当前目标对象
    private bool _isFollowing = false;       // 是否在跟随
    private bool _isEscaping = false;        // 是否在逃离
    private bool _isStandingBy = false;      // 是否在待命
    private bool _isIdle = true;             // 是否在闲置状态

    private float _updateInterval = 0.1f;     // 更新间隔
    private float _lastUpdateTime = 0f;      // 上次更新时间
    private float _idleMoveInterval = 3f;    // 闲置状态下随机移动的间隔
    private float _lastIdleMoveTime = 0f;    // 上次闲置移动的时间

    private EventBinding<NPCFollowEvent> _followEventBinding;
    private EventBinding<NPCEscapeEvent> _escapeEventBinding;
    private EventBinding<NPCStandbyEvent> _standbyEventBinding;

    private void Awake()
    {
        _npcGenearted = GetComponent<NPC_Genearted>();
        _movementController = GetComponent<NPCMovementController>();
        
        // 注册事件
        _followEventBinding = new EventBinding<NPCFollowEvent>(HandleFollow);
        _escapeEventBinding = new EventBinding<NPCEscapeEvent>(HandleEscape);
        _standbyEventBinding = new EventBinding<NPCStandbyEvent>(HandleStandby);
    }

    private void OnEnable()
    {
        _npcGenearted.NPC_EventBus.Register<NPCFollowEvent>(_followEventBinding);
        _npcGenearted.NPC_EventBus.Register<NPCEscapeEvent>(_escapeEventBinding);
        _npcGenearted.NPC_EventBus.Register<NPCStandbyEvent>(_standbyEventBinding);
    }

    private void OnDisable()
    {
        _npcGenearted.NPC_EventBus.Deregister<NPCFollowEvent>(_followEventBinding);
        _npcGenearted.NPC_EventBus.Deregister<NPCEscapeEvent>(_escapeEventBinding);
        _npcGenearted.NPC_EventBus.Deregister<NPCStandbyEvent>(_standbyEventBinding);
    }

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (Time.time - _lastUpdateTime < _updateInterval)
        {
            return;
        }

        _lastUpdateTime = Time.time;

        if (_isFollowing)
        {
            UpdateFollow();
        }
        else if (_isEscaping)
        {
            UpdateEscape();
        }
        else if (_isStandingBy)
        {
            UpdateStandby();
        }
        else if (_isIdle)
        {
            UpdateIdle();
        }
    }

    /// <summary>
    /// 处理跟随事件
    /// </summary>
    private void HandleFollow(NPCFollowEvent e)
    {
        if (e.npc != gameObject)
        {
            return;
        }

        _currentTarget = e.target;
        _isFollowing = true;
        _isEscaping = false;
        _isStandingBy = false;
        _isIdle = false;
        Debug.Log($"{gameObject.name} 开始跟随 {_currentTarget.name}");
    }

    /// <summary>
    /// 处理逃离事件
    /// </summary>
    private void HandleEscape(NPCEscapeEvent e)
    {
        if (e.npc != gameObject)
        {
            return;
        }

        _currentTarget = e.target;
        _isFollowing = false;
        _isEscaping = true;
        _isStandingBy = false;
        _isIdle = false;
        Debug.Log($"{gameObject.name} 开始逃离 {_currentTarget.name}");
    }

    /// <summary>
    /// 处理待命事件
    /// </summary>
    private void HandleStandby(NPCStandbyEvent e)
    {
        if (e.npc != gameObject)
        {
            return;
        }

        _isFollowing = false;
        _isEscaping = false;
        _isStandingBy = true;
        _isIdle = false;
        _movementController.StopMovement();
        Debug.Log($"{gameObject.name} 进入待命状态");
    }

    /// <summary>
    /// 处理闲置状态
    /// </summary>
    private void UpdateIdle()
    {
        // 检查是否需要随机移动
        if (Time.time - _lastIdleMoveTime >= _idleMoveInterval)
        {
            // 生成随机目标点
            Vector3 randomPosition = GenerateRandomPosition();
            _movementController.SetTargetPosition(randomPosition);
            _lastIdleMoveTime = Time.time;
        }

        // 检查是否到达目标点
        if (_movementController.IsAtTarget())
        {
            _movementController.StopMovement();
        }
    }

    /// <summary>
    /// 生成附近的随机位置
    /// </summary>
    private Vector3 GenerateRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * randomMoveRadius;
        randomDirection.z = transform.position.z;
        return transform.position + randomDirection;
    }

    /// <summary>
    /// 更新跟随逻辑
    /// </summary>
    private void UpdateFollow()
    {
        if (_currentTarget == null)
        {
            _isFollowing = false;
            _isIdle = true;
            return;
        }

        Vector3 targetPos = _currentTarget.transform.position;
        Vector3 npcPos = transform.position;
        float distance = Vector3.Distance(npcPos, targetPos);

        // 计算目标位置（保持一定距离）
        if (distance > followDistance)
        {
            Vector3 direction = (targetPos - npcPos).normalized;
            Vector3 moveTargetPos = targetPos - direction * followDistance;
            _movementController.SetTargetPosition(moveTargetPos);
        }
        else
        {
            _movementController.StopMovement();
        }
    }

    /// <summary>
    /// 更新逃离逻辑
    /// </summary>
    private void UpdateEscape()
    {
        if (_currentTarget == null)
        {
            _isEscaping = false;
            _isIdle = true;
            return;
        }

        Vector3 targetPos = _currentTarget.transform.position;
        Vector3 npcPos = transform.position;
        float distance = Vector3.Distance(npcPos, targetPos);

        // 如果距离足够远，停止逃离
        if (distance >= escapeDistance)
        {
            _movementController.StopMovement();
            _isEscaping = false;
            _isIdle = true;
            Debug.Log($"{gameObject.name} 逃离完成，进入闲置状态");
            return;
        }

        // 计算逃离方向
        Vector3 direction = (npcPos - targetPos).normalized;
        Vector3 moveTargetPos = npcPos + direction * escapeDistance;
        _movementController.SetTargetPosition(moveTargetPos);
    }

    /// <summary>
    /// 更新待命逻辑
    /// </summary>
    private void UpdateStandby()
    {
        // 待命状态下，保持在原地
        _movementController.StopMovement();
    }

#region Debug
    /// <summary>
    /// 绘制范围曲线
    /// </summary>
    private void OnDrawGizmos()
    {
        // 绘制当前状态的范围
        if (_isFollowing || _isEscaping)
        {
            // 绘制跟随或逃离范围
            Gizmos.color = _isFollowing ? Color.blue : Color.red;
            float radius = _isFollowing ? followDistance : escapeDistance;
            Gizmos.DrawWireSphere(transform.position, radius);

            // 绘制到当前目标的连线
            if (_currentTarget != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, _currentTarget.transform.position);
            }
        }
        else if (_isStandingBy)
        {
            // 绘制待命范围
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        else if (_isIdle)
        {
            // 绘制闲置状态范围
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, randomMoveRadius);
        }
    }
#endregion
}
