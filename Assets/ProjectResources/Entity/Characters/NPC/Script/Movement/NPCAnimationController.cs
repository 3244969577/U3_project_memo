using UnityEngine;
using NPCInfoEvents;
public class NPCAnimationController : MonoBehaviour
{
    [Header("动画设置")]
    public float frameRate = 10f;

    // 精灵数组：[方向, 帧] → 0=Down, 1=Left, 2=Right, 3=Up
    private Sprite[,] _dirFrameSprites;
    private SpriteRenderer _spriteRenderer;
    private NPC_Genearted _npcGenearted;

    // 动画状态
    private int _currentDir = 0; // 当前方向索引
    private int _currentFrame = 1; // 当前帧索引（停止时保持第二帧）
    private float _frameTimer = 0f;
    private bool _isMoving = false;

    private EventBinding<NPCMoveEvent> _moveEventBinding;
    private EventBinding<NPCStopMoveEvent> _stopMoveEventBinding;
    private EventBinding<NPCChangeDirEvent> _changeDirEventBinding;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _npcGenearted = GetComponent<NPC_Genearted>();
        // 注册事件
        _moveEventBinding = new EventBinding<NPCMoveEvent>(HandleNPCMove);
        _stopMoveEventBinding = new EventBinding<NPCStopMoveEvent>(HandleNPCStopMove);
        _changeDirEventBinding = new EventBinding<NPCChangeDirEvent>(HandleNPCChangeDir);
    }

    private void OnEnable()
    {
        _npcGenearted.NPC_EventBus.Register<NPCMoveEvent>(_moveEventBinding);
        _npcGenearted.NPC_EventBus.Register<NPCStopMoveEvent>(_stopMoveEventBinding);
        _npcGenearted.NPC_EventBus.Register<NPCChangeDirEvent>(_changeDirEventBinding);
    }

    private void OnDisable()
    {
        _npcGenearted.NPC_EventBus.Deregister<NPCMoveEvent>(_moveEventBinding);
        _npcGenearted.NPC_EventBus.Deregister<NPCStopMoveEvent>(_stopMoveEventBinding);
        _npcGenearted.NPC_EventBus.Deregister<NPCChangeDirEvent>(_changeDirEventBinding);
    }

    private void Update()
    {
        if (_dirFrameSprites == null || _spriteRenderer == null)
        {
            return;
        }
        // Debug.Log($"UpdateAnimationFrame: {_currentDir}, {_currentFrame}");
        UpdateAnimationFrame();
    }

    /// <summary>
    /// 设置精灵数组
    /// </summary>
    public void SetSprites(Sprite[,] sprites)
    {
        // Debug.Assert(sprites != null, "Sprites is null");
        // Debug.Log($"SetSprites: {sprites.GetLength(0)}x{sprites.GetLength(1)}");
        _dirFrameSprites = sprites;
        UpdateSprite();
    }

    /// <summary>
    /// 处理NPC移动事件
    /// </summary>
    private void HandleNPCMove(NPCMoveEvent e)
    {
        _isMoving = true;
    }

    /// <summary>
    /// 处理NPC停止移动事件
    /// </summary>
    private void HandleNPCStopMove(NPCStopMoveEvent e)
    {
        _isMoving = false;
        _currentFrame = 1; // 停止时保持第二帧
        UpdateSprite();
    }

    /// <summary>
    /// 处理NPC方向改变事件
    /// </summary>
    private void HandleNPCChangeDir(NPCChangeDirEvent e)
    {
        _currentDir = DirectionToIndex(e.newDirection);
        UpdateSprite();
    }

    /// <summary>
    /// 更新动画帧
    /// </summary>
    private void UpdateAnimationFrame()
    {
        if (!_isMoving)
        {
            return;
        }

        _frameTimer += Time.deltaTime;
        if (_frameTimer >= 1f / frameRate)
        {
            _currentFrame = (_currentFrame + 1) % 3;
            _frameTimer = 0f;
            UpdateSprite();
        }
    }

    /// <summary>
    /// 更新精灵显示
    /// </summary>
    private void UpdateSprite()
    {
        if (_dirFrameSprites == null || _spriteRenderer == null)
        {
            return;
        }

        _spriteRenderer.sprite = _dirFrameSprites[_currentDir, _currentFrame];
    }

    /// <summary>
    /// 将NPCDirection转换为索引
    /// </summary>
    private int DirectionToIndex(NPCDirection direction)
    {
        switch (direction)
        {
            case NPCDirection.Down:
                return 0;
            case NPCDirection.Left:
                return 1;
            case NPCDirection.Right:
                return 2;
            case NPCDirection.Up:
                return 3;
            default:
                return 0;
        }
    }
}
