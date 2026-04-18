using UnityEngine;
using UnityEngine.SceneManagement;
using GameStatusSystem.PlayerStatus.Events;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject gameWinUI;

    private static UIManager instance;
    public static UIManager Instance => instance;

    private EventBinding<GameOverEvent> gameOverEventBinding;
    private EventBinding<GameWinEvent> gameWinEventBinding;


    private void Awake()
    {
        if (UIManager.instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        // DontDestroyOnLoad(gameObject);

        // 初始化事件绑定
        gameOverEventBinding = new EventBinding<GameOverEvent>(HandleGameOver);
        gameWinEventBinding = new EventBinding<GameWinEvent>(HandleGameWin);
    }

    private void OnEnable()
    {
        // 注册事件绑定
        EventBus<GameOverEvent>.Register(gameOverEventBinding);
        EventBus<GameWinEvent>.Register(gameWinEventBinding);
    }

    private void OnDisable()
    {
        // 注销事件绑定
        EventBus<GameOverEvent>.Deregister(gameOverEventBinding);
        EventBus<GameWinEvent>.Deregister(gameWinEventBinding);
    }

    /// <summary>
    /// 显示游戏失败 UI
    /// </summary>
    public void ShowGameOverUI()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
    }

    /// <summary>
    /// 显示游戏胜利 UI
    /// </summary>
    public void ShowGameWinUI()
    {
        if (gameWinUI != null)
        {
            gameWinUI.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏游戏失败 UI
    /// </summary>
    public void HideGameOverUI()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    /// <summary>
    /// 隐藏游戏胜利 UI
    /// </summary>
    public void HideGameWinUI()
    {
        if (gameWinUI != null)
        {
            gameWinUI.SetActive(false);
        }
    }

    /// <summary>
    /// 处理游戏失败逻辑
    /// </summary>
    public void HandleGameOver(GameOverEvent e)
    {
        // 显示游戏失败 UI
        ShowGameOverUI();
    }

    /// <summary>
    /// 处理游戏胜利逻辑
    /// </summary>
    public void HandleGameWin(GameWinEvent e)
    {
        // 显示游戏胜利 UI
        ShowGameWinUI();
    }
}