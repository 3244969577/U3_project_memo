using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float rotateSpeed = 200f; // 拐弯快慢
    public float detectionRange = 15f; // 检测范围
    private Transform target;
    private Bullet bullet;


    private void Awake()
    {
        bullet = GetComponent<Bullet>();
    }

    // 外部绑定追踪目标
    public void SetTarget(Transform enemyTarget)
    {
        target = enemyTarget;
    }

    private void Update()
    {
        // 实时检测周围目标
        UpdateTarget();

        // 无目标时保持当前朝向
        if (target == null)
        {
            return;
        }

        // 1. 计算朝向目标方向
        Vector2 dir = (target.position - transform.position).normalized;
        
        // 2. 平滑旋转子弹朝向
        Quaternion targetRot = Quaternion.FromToRotation(Vector2.right, dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        rotateSpeed += 0.1f;
    }

    // 实时检测周围目标
    private void UpdateTarget()
    {
        // 如果有目标则不再检测
        if (target != null)
            return;
        // 检测周围的碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        
        // 寻找第一个目标
        foreach (Collider2D collider in colliders)
        {
            if (bullet.IsTarget(collider.gameObject) && collider.gameObject != bullet.attacker)
            {
                target = collider.transform;
                return;
            }
        }
        
        // 没有找到目标
        target = null;
    }


    // 绘制检测范围 gizmo
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}