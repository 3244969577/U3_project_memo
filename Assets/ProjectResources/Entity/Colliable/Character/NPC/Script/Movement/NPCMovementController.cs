using UnityEngine;
using NPCInfoEvents;
public enum NPCDirection
{
    Left,
    Right,
    Up,
    Down,
}
public class NPCMovementController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 2f;              // 移动速度
    public float stopDistance = 0.5f;         // 停止距离

    private Vector3 targetPosition;           // 目标位置
    private bool isMoving = false;            // 移动状态
    private Rigidbody2D rb;                   // 刚体组件
    private NPCDirection currentDirection = NPCDirection.Down; // 当前方向
    private NPC_Genearted npcGenearted;

    // 移动状态属性
    public bool IsMoving => isMoving;

    // 当前方向属性
    public NPCDirection CurrentDirection => currentDirection;

    // 目标位置属性
    public Vector3 TargetPosition => targetPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = transform.position; // 初始目标位置为当前位置
        npcGenearted = GetComponent<NPC_Genearted>();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        // Debug.Log(rb.linearVelocity);
        if (!isMoving)
        {
            
            return;
        }

        // 计算到目标点的方向和距离
        Vector3 direction = (targetPosition - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, targetPosition);

        // 检查是否到达目标点
        if (distance <= stopDistance)
        {
            StopMovement();
            return;
        }

        // 移动到目标点
        if (rb != null && isMoving)
        {
            Vector2 moveDirection = new Vector2(direction.x, direction.y).normalized;
            rb.linearVelocity = moveDirection * moveSpeed;
            UpdateDirection();
        }
    }

    private void UpdateDirection()
    {
        if (rb == null || rb.linearVelocity.magnitude < 0.1f)
        {
            return;
        }
        NPCDirection oldDirection = currentDirection;

        Vector2 velocity = rb.linearVelocity;
        float absX = Mathf.Abs(velocity.x);
        float absY = Mathf.Abs(velocity.y);

        if (absX > absY)
        {
            currentDirection = velocity.x > 0 ? NPCDirection.Right : NPCDirection.Left;
        }
        else
        {
            currentDirection = velocity.y > 0 ? NPCDirection.Up : NPCDirection.Down;
        }
        if (oldDirection != currentDirection)
        {
            npcGenearted.NPC_EventBus.Raise(new NPCChangeDirEvent {
                npcMovementController = this, 
                oldDirection = oldDirection, 
                newDirection = currentDirection
            });
        }
    }

   #region 移动控制外部接口
    public void SetTargetPosition(Vector3 position)
    {
        // Debug.Log($"SetTargetPosition: {position}");
        targetPosition = position;
        targetPosition.z = transform.position.z;
        if (Vector3.Distance(transform.position, targetPosition) <= stopDistance)
        {
            return;
        }

        if (!isMoving)
        {
            // 只有开始移动时才触发移动事件，避免重复触发移动事件
            isMoving = true;
            npcGenearted.NPC_EventBus.Raise(new NPCMoveEvent {
                npcMovementController = this
            });
        }
    }

    public void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        isMoving = false;
        npcGenearted.NPC_EventBus.Raise(new NPCStopMoveEvent {
            npcMovementController = this
        });
    }

    public void TeleportTo(Vector3 position)
    {
        transform.position = position;
        targetPosition = position;
        StopMovement();
    }

    public bool IsAtTarget()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        return distance <= stopDistance;
    }
#endregion

#region Debug

    private void OnDrawGizmos()
    {
        // 绘制从NPC到目标点的连线
        Gizmos.color = this.isMoving ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, targetPosition);
        
        // 绘制目标点标记
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 0.2f);
    }

#endregion
}
