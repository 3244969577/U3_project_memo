using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商人NPC类 - 游戏中的商人角色
/// - 继承自NPC，包含NPC的基本属性和行为
/// - 实现商店功能，允许玩家购买物品
/// </summary>
public class Vendor : NPC
{
    /// <summary>
    /// 升级菜单回调委托
    /// - 用于控制升级菜单的显示和隐藏
    /// </summary>
    private delegate void UpgradeMenuCallback(bool active);

    /// <summary>
    /// 升级菜单引用
    /// - 用于显示和管理商店界面
    /// </summary>
    public UpgradeMenu upgradeMenu;

    protected override void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (this.interacble)
            {
                Shopping();
            }
        }
    }

    protected void Shopping()
    {
        upgradeMenu.Toggle();
        
    }
}
