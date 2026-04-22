using UnityEngine;
using DG.Tweening; // 若未使用DOTween，可替换为原生Lerp逻辑

public class SpriteHoverHighlight : MonoBehaviour
{
    [Header("高亮配置")]
    [SerializeField] private Color highlightColor = Color.yellow; // 高亮颜色
    [SerializeField] private float highlightDuration = 0.2f; // 颜色渐变时长

    [Header("放大配置")]
    [SerializeField] private float targetScale = 1.1f; // 悬停放大倍数
    [SerializeField] private float scaleDuration = 0.2f; // 缩放过渡时长

    private SpriteRenderer _spriteRenderer;
    private Color _originalColor; // 原始颜色
    private Vector3 _originalScale; // 原始缩放

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteHoverHighlight：未找到SpriteRenderer组件！");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        // 初始化原始状态
        _originalColor = _spriteRenderer.color;
        _originalScale = transform.localScale;
    }

    // 鼠标进入悬浮区域
    private void OnMouseEnter()
    {
        // 1. 颜色高亮渐变
        _spriteRenderer.DOColor(highlightColor, highlightDuration);
        // 2. 平滑放大
        transform.DOScale(targetScale, scaleDuration);
    }

    // 鼠标离开悬浮区域
    private void OnMouseExit()
    {
        // 1. 还原颜色
        _spriteRenderer.DOColor(_originalColor, highlightDuration);
        // 2. 还原缩放
        transform.DOScale(_originalScale, scaleDuration);
    }
}