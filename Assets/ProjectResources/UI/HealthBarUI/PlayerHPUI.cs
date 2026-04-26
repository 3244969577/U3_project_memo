using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GlobalEvents;

public class PlayerHPUI : MonoBehaviour
{
    [Header("UI引用")]
    public Slider hpSlider;      // 血条
    public TextMeshProUGUI hpText;          // 血量文字

    [Header("平滑渐变设置")]
    public float smoothSpeed = 5f; // 平滑过渡速度

    private EventBinding<PlayerHPChangeEvent> _playerHPChangeEventBinding;
    private float targetFillAmount = 1f; // 目标填充量

    private void Awake()
    {
        _playerHPChangeEventBinding = new EventBinding<PlayerHPChangeEvent>(HandlePlayerHPChangeEvent);
    }

    private void OnEnable()
    {
        EventBus<PlayerHPChangeEvent>.Register(_playerHPChangeEventBinding);
    }

    private void OnDisable()
    {
        EventBus<PlayerHPChangeEvent>.Deregister(_playerHPChangeEventBinding);
    }

    private void Update()
    {
        // 平滑过渡血条填充量
        if (hpSlider.value != targetFillAmount)
        {
            hpSlider.value = Mathf.Lerp(hpSlider.value, targetFillAmount, smoothSpeed * Time.deltaTime);
        }
    }

    private void HandlePlayerHPChangeEvent(PlayerHPChangeEvent e)
    {
        // 更新文字
        hpText.text = $"{e.currentHealth}/{e.maxHealth}";
        
        // 更新目标填充量
        targetFillAmount = e.currentHealth / e.maxHealth;
    }
}
