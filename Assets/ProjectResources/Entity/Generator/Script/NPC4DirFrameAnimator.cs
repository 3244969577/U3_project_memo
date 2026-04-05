using UnityEngine;

public class NPC4DirFrameAnimator : MonoBehaviour
{
    [Header("动画设置")]
    public float frameRate = 10f;

    // 4×3精灵数组：方向(0前1左2右3后) × 帧(012)
    private Sprite[,] _dirFrameSprites;
    private SpriteRenderer _sr;

    // 动画状态（由移动脚本赋值）
    private int _currentDir;
    private int _currentFrame;
    private float _frameTimer;
    private bool _isMoving;

    public void SetSprites(Sprite[,] sprites)
    {
        _dirFrameSprites = sprites;
        _currentFrame = 0;
        UpdateSprite();
    }

    public void SetSpriteRenderer(SpriteRenderer sr)
    {
        _sr = sr;
        Debug.Assert(_sr != null, "sr 未赋值");
    }

    /// <summary>
    /// 外部调用：设置移动状态和方向
    /// </summary>
    public void SetMoveState(bool isMoving, int direction = 0)
    {
        _isMoving = isMoving;
        _currentDir = direction;
    }

    private void Update()
    {
        if (_dirFrameSprites == null || _sr == null) return;
        UpdateAnimationFrame();
    }

    /// <summary>
    /// 纯动画更新逻辑
    /// </summary>
    private void UpdateAnimationFrame()
    {
        if (!_isMoving)
        {
            _currentFrame = 1;
            _frameTimer = 0;
            UpdateSprite();
            return;
        }

        _frameTimer += Time.deltaTime;
        if (_frameTimer >= 1f / frameRate)
        {
            _currentFrame = (_currentFrame + 1) % 3;
            _frameTimer = 0;
            UpdateSprite();
        }
    }

    private void UpdateSprite()
    {
        Debug.Assert(_dirFrameSprites != null, "dirFrameSprites 未赋值");
        Debug.Assert(_sr != null, "sr 未赋值");
        _sr.sprite = _dirFrameSprites[_currentDir, _currentFrame];
    }
}


// using UnityEngine;

// /// <summary>
// /// 4方向3帧行走动画控制器（完全不用Animator，运行时100%安全）
// /// 支持WASD/方向键控制移动，自动切换方向和播放行走动画
// /// </summary>
// public class NPC4DirFrameAnimator : MonoBehaviour
// {
//     [Header("动画&移动设置")]
//     [Tooltip("动画帧率，每秒播放多少帧，默认10，数值越大越快")]
//     public float frameRate = 10f;
//     [Tooltip("NPC移动速度，默认3")]
//     public float moveSpeed = 3f;

//     // 4×3的Sprite数组：[方向, 帧] → 0=前,1=左,2=右,3=后
//     private Sprite[,] _dirFrameSprites;
//     private SpriteRenderer _sr;

//     // 动画状态
//     private int _currentDir = 0; // 当前方向
//     private int _currentFrame = 0; // 当前帧索引（0/1/2）
//     private float _frameTimer = 0f; // 帧计时器，控制帧率

//     // 移动状态
//     private Vector2 _moveDir;

//     // 方向枚举，方便管理
//     private enum Direction { Front, Left, Right, Back }

//     /// <summary>
//     /// 赋值切割好的4×3Sprite数组（由NPCSpriteTool自动调用）
//     /// </summary>
//     public void SetSprites(Sprite[,] sprites)
//     {
//         _dirFrameSprites = sprites;
//         // 初始化显示正面第0帧
//         if (_sr != null) _sr.sprite = _dirFrameSprites[0, 0];
//     }

//     /// <summary>
//     /// 绑定SpriteRenderer（由NPCSpriteTool自动调用）
//     /// </summary>
//     public void SetSpriteRenderer(SpriteRenderer sr)
//     {
//         _sr = sr;
//     }

//     private void Update()
//     {
//         if (_dirFrameSprites == null || _sr == null) return;

//         // 1. 获取移动输入
//         GetMoveInput();
//         // 2. 移动NPC
//         MoveNPC();
//         // 3. 更新动画方向
//         UpdateDirection();
//         // 4. 更新动画帧
//         UpdateAnimationFrame();
//     }

//     /// <summary>
//     /// 获取WASD/方向键移动输入
//     /// </summary>
//     private void GetMoveInput()
//     {
//         float h = Input.GetAxisRaw("Horizontal");
//         float v = Input.GetAxisRaw("Vertical");
//         _moveDir = new Vector2(h, v).normalized; // 归一化，避免斜向移动更快
//     }

//     /// <summary>
//     /// 移动NPC
//     /// </summary>
//     private void MoveNPC()
//     {
//         if (_moveDir.magnitude < 0.1f) return; // 无输入则不移动
//         transform.Translate(_moveDir * moveSpeed * Time.deltaTime, Space.World);
//     }

//     /// <summary>
//     /// 根据移动方向更新当前动画方向（优先级：上下>左右，避免斜向混乱）
//     /// </summary>
//     private void UpdateDirection()
//     {
//         if (_moveDir.magnitude < 0.1f) return;

//         if (Mathf.Abs(_moveDir.y) > Mathf.Abs(_moveDir.x))
//         {
//             // 上下方向
//             _currentDir = _moveDir.y < 0 ? (int)Direction.Front : (int)Direction.Back;
//         }
//         else
//         {
//             // 左右方向
//             _currentDir = _moveDir.x < 0 ? (int)Direction.Left : (int)Direction.Right;
//         }
//     }

//     /// <summary>
//     /// 更新动画帧，按帧率循环播放当前方向的3帧
//     /// 无移动时停在当前方向第0帧（待机）
//     /// </summary>
//     private void UpdateAnimationFrame()
//     {
//         // 无移动：待机，停在当前方向第0帧
//         if (_moveDir.magnitude < 0.1f)
//         {
//             _currentFrame = 0;
//             _frameTimer = 0f;
//             _sr.sprite = _dirFrameSprites[_currentDir, _currentFrame];
//             return;
//         }

//         // 有移动：按帧率循环播放3帧
//         _frameTimer += Time.deltaTime;
//         float frameInterval = 1f / frameRate; // 每帧的时间间隔

//         if (_frameTimer >= frameInterval)
//         {
//             _currentFrame = (_currentFrame + 1) % 3; // 0→1→2→0循环
//             _frameTimer = 0f;
//             _sr.sprite = _dirFrameSprites[_currentDir, _currentFrame];
//         }
//     }
// }