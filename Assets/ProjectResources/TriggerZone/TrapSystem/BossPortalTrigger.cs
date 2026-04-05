using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Boss传送门触发器 - 生成Boss并在Boss死亡时生成传送门
/// - 继承自TriggerZone
/// - 存储Boss和传送门预制体
/// - 触发时生成Boss
/// - Boss死亡时生成传送门
/// </summary>
public class BossPortalTrigger : TriggerZone
{
    public GameObject portal;
    public GameObject boss;


    protected override void Start()
    {
        triggerPoint.OnPlayerEnterTrigger += SpawnBoss;
    }
    
    private void SpawnBoss(object sender, System.EventArgs e)   
    {
        GameObject spawn = Instantiate(boss, transform.position, transform.rotation);
        spawn.transform.SetParent(this.transform.parent);

        spawn.gameObject.GetComponent<Boss>().OnDeathTrigger += SpawnPortal;
    }

    private void SpawnPortal(object sender, System.EventArgs e)
    {
        Instantiate(portal, transform.position, transform.rotation);
    }
}

// /// <summary>
// /// 传送门触发器 - 为Boss分配死亡触发器以生成传送门
// /// - 继承自TriggerZone
// /// - 存储Portal和Boss引用
// /// - 为场景中的Boss分配死亡触发器
// /// - Boss死亡时生成传送门
// /// </summary>
// public class PortalTrigger : TriggerZone
// {
//     public Portal portal;
//     public Boss boss;

//     protected override void Start()
//     {
//         triggerPoint.OnPlayerEnterTrigger += AssignBossTrigger;
//     }
    
//     protected void AssignBossTrigger(object sender, System.EventArgs e)
//     {
//         string bossName = this.boss.name;
//         Transform bossTransform = gameObject.transform.Find(bossName);
//         Boss bossObject = bossTransform.gameObject.GetComponent<Boss>();
//         bossObject.OnDeathTrigger += SpawnPortal;
//     }

//     private void SpawnPortal(object sender, System.EventArgs e)
//     {
//         Instantiate(portal, transform.position, transform.rotation);
//     }
// }
