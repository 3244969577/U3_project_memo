using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC基类 - 游戏中的非玩家角色
/// - 继承自Collidable，包含碰撞检测功能
/// - 实现与玩家的交互逻辑
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class NPC : Collidable
{
    /// <summary>
    /// NPC的对话数据
    /// - 存储NPC的对话内容
    /// </summary>
    public Dialog dialog;

    /// <summary>
    /// NPC是否可交互
    /// - true: 玩家可以与NPC交互
    /// - false: 玩家无法与NPC交互
    /// </summary>
    protected bool interacble = false;

    /// <summary>
    /// 交互提示文本
    /// - 显示"Press Space to interact"等提示信息
    /// </summary>
    private FloatingText guidanceText;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            guidanceText = GameManager.instance.ShowText("Press Space to interact", 100, Color.white, transform.position + new Vector3(0f, 1.75f, 0), Vector3.zero, 0);
            this.interacble = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.interacble = false;
            guidanceText.Hide();
        }
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            guidanceText = GameManager.instance.ShowText("Press Space to interact", 100, Color.white, transform.position + new Vector3(0f, 1.75f, 0), Vector3.zero, 0);
            this.interacble = true;
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.interacble = false;
            guidanceText.Hide();
        }
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (this.interacble)
            {
                DialogManager.instance.ShowDialog(dialog);
            }
        }
    }
}
