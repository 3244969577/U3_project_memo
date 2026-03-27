using UnityEngine;

public interface BulletEffect
{
    // 初始化效果
    void Initialize(Bullet bullet);
    
    // 射击时触发的效果（如分裂子弹）
    void OnShoot();
    
    // 碰撞时触发的效果（如子母弹、穿透）
    void OnCollisionEnter2D(Collision2D collision);
    
    // 更新效果（如需要持续计算的效果）
    void Update();
}
