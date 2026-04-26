using UnityEngine;

public class SpeedingBullet : MonoBehaviour
{
    public float speedIncreaseRate = 0.1f; // 速度增加率
    public float damageIncreaseRate = 0.1f; // 伤害增加率
    private Bullet bullet;

    private void Awake()
    {
        bullet = GetComponent<Bullet>();
    }

    private void Update()
    {
        if (bullet != null)
        {
            // 线性增加子弹速度
            bullet.moveSpeed += speedIncreaseRate * Time.deltaTime;
            
            // 线性增加子弹伤害
            bullet.damage += damageIncreaseRate * Time.deltaTime;
        }
    }
}
