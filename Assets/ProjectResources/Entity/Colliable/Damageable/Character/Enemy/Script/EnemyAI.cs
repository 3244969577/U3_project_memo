using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人AI系统 - 控制敌人的行为状态
/// - 定现敌人的巡逻、追逐和攻击逻辑
/// - 使用状态机管理敌人的不同行为状态
/// </summary>
public class EnemyAI : MonoBehaviour
{
    /// <summary>
    /// 敌人的行为状态枚举
    /// - RoamingState: 巡逻状态，在指定范围内随机移动
    /// - ChaseState: 追逐状态，追击玩家
    /// - AttackState: 攻击状态，对玩家进行攻击
    /// </summary>
    private enum State
    {
        RoamingState,
        ChaseState,
        AttackState
    }

    /// <summary>
    /// 敌人的巡逻范围
    /// - 敌人在起始位置周围的巡逻半径
    /// </summary>
    public float RoamingRange = 4f;

    /// <summary>
    /// 敌人的追逐范围
    /// - 当玩家进入此范围时，敌人开始追逐
    /// </summary>
    public float ChaseRange = 8f;

    /// <summary>
    /// 敌人停止追逐的范围
    /// - 当玩家离开此范围时，敌人停止追逐
    /// </summary>
    public float StopChaseRange = 15f;

    /// <summary>
    /// 敌人的攻击范围
    /// - 当玩家进入此范围时，敌人开始攻击
    /// </summary>
    public float AttackRange = 4f;

    /// <summary>
    /// 敌人实例引用
    /// - 用于调用敌人的移动和攻击方法
    /// </summary>
    private Enemy instance;

    /// <summary>
    /// 敌人的起始位置
    /// - 巡逻状态下的中心点
    /// </summary>
    private Vector2 startPosition;

    /// <summary>
    /// 敌人当前的巡逻目标位置
    /// - 巡逻状态下随机生成的目标点
    /// </summary>
    private Vector2 roamPosition;

    /// <summary>
    /// 计时器
    /// - 用于控制巡逻位置的更新
    /// </summary>
    private float timer;

    /// <summary>
    /// 当前敌人的状态
    /// - 巡逻、追逐或攻击
    /// </summary>
    private State currentState;


    private void Awake()
    {
        this.instance = GetComponent<Enemy>();
        this.currentState = State.RoamingState;
    }

    private void Start()
    {
        startPosition = transform.position;
        roamPosition = GetRoamingPosition();
    }

    protected virtual void Update()
    {
        Player target = GetPlayerTarget();
        if (target == null) return;
        
        this.currentState = GetCurrentState(target);
        switch (this.currentState)
        {
            case State.RoamingState:
                RoamingAround();
                break;

            case State.ChaseState:
                ChaseTarget(target);
                break;
            
            case State.AttackState:
                AttackTarget(target);
                break;

        }
        
    }
    
    private Player GetPlayerTarget()
    {
        if (GameManager.instance == null || GameManager.instance.player == null)
            return null;
        
        return GameManager.instance.player.GetComponent<Player>();
    }

    private State GetCurrentState(Player player)
    {
        float distanceFromPlayer = GetDistanceFromPlayer(player);
        if (distanceFromPlayer <= this.AttackRange)
            return State.AttackState;
        else if (distanceFromPlayer <= this.ChaseRange)
            return State.ChaseState;
        else if (this.currentState == State.ChaseState && distanceFromPlayer <= this.StopChaseRange)
            return State.ChaseState;
        return State.RoamingState;
    }

    private Vector2 GetRoamingPosition()
    {
        timer = Time.time;
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        return startPosition + randomDirection * Random.Range(RoamingRange, RoamingRange);
    }

    private void RoamingAround()
    {
        
        this.instance.MoveTo(roamPosition);
        float reachedPositionDistance = 1f;

        if ((Vector3.Distance(transform.position, roamPosition) < reachedPositionDistance) || (Time.time - timer > 5f))
        {
            roamPosition = GetRoamingPosition();
        }
    }

    protected float GetDistanceFromPlayer(Player player)
    {
        return Vector2.Distance((Vector2)instance.transform.position, (Vector2)player.GetPosition());
    }

    protected void ChaseTarget(Player player)
    {
        this.instance.MoveTo(player.GetPosition());
    }

    protected void AttackTarget(Player player)
    {
        this.instance.Attack(player);
    }

}
