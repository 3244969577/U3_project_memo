using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [Header("冲刺设置")]
    public float dashDistance = 5f;      // 冲刺距离
    public float dashSpeed = 30f;        // 冲刺速度
    public float dashCooldown = 1f;      // 冷却时间（秒）

    [Header("组件引用")]
    private Rigidbody2D rb;              // 刚体引用

    private float cooldownTimer = 0f;     // 冷却计时器
    private bool isDashing = false;      // 是否正在冲刺
    private Vector2 dashDirection;        // 冲刺方向
    private Vector2 dashStartPosition;    // 冲刺开始位置

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 更新冷却计时器
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // 检测冲刺输入
        if (Input.GetKeyDown(KeyCode.F) && CanDash())
        {
            StartDash();
        }
    }

    private void FixedUpdate()
    {
        // 执行冲刺移动
        if (isDashing)
        {
            PerformDash();
        }
    }

    private bool CanDash() => cooldownTimer <= 0f && !isDashing;

    private void StartDash()
    {
        // 获取鼠标位置
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // 计算冲刺方向
        dashDirection = (mousePosition - transform.position).normalized;

        // 记录冲刺开始位置
        dashStartPosition = transform.position;

        // 开始冲刺
        isDashing = true;

        // 设置冷却时间
        cooldownTimer = dashCooldown;
    }

    private void PerformDash()
    {
        // 计算已经移动的距离
        float distanceMoved = Vector2.Distance(transform.position, dashStartPosition);

        if (distanceMoved >= dashDistance)
        {
            // 达到冲刺距离，结束冲刺
            isDashing = false;
            return;
        }

        // 高速移动
        Vector2 newPosition = (Vector2)transform.position + dashDirection * dashSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    public void StartDash(Vector2 direction)
    {
        if (!CanDash()) return;

        direction.Normalize();
        dashDirection = direction;
        dashStartPosition = transform.position;

        isDashing = true;
        cooldownTimer = dashCooldown;
    }
}
