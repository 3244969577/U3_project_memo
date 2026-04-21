using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EmotionManager : MonoBehaviour
{
    [Header("表情图片目标组件")]
    public GameObject emotionPanel;

    [SerializeField] private Image emotionImage;

    // 表情获取工具脚本引用（自行赋值，外部负责名称转Sprite逻辑）
    public EmotionSpriteTool spriteTool;

    private Coroutine currentHideCoroutine;

    public void ShowEmotion(string emotionName)
    {
        // 停止上一次倒计时，重置时间
        if (currentHideCoroutine != null)
        {
            StopCoroutine(currentHideCoroutine);
        }

        // 调用外部工具，通过表情名称获取对应Sprite
        Sprite targetSprite = spriteTool.GetEmotionSprite(emotionName);
        
        if (targetSprite != null)
        {
            // 替换图片资源
            emotionImage.sprite = targetSprite;
            // 显示表情面板
            emotionPanel.SetActive(true);
            
            // 开启5秒隐藏协程
            currentHideCoroutine = StartCoroutine(HidePanelAfterDelay(5f));
        }
        else
        {
            Debug.LogWarning($"未找到表情【{emotionName}】对应的图片资源");
        }
    }


    private IEnumerator HidePanelAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        // 关闭表情面板
        emotionPanel.SetActive(false);
        currentHideCoroutine = null;
    }


    public void HideEmotionImmediate()
    {
        if (currentHideCoroutine != null)
        {
            StopCoroutine(currentHideCoroutine);
            currentHideCoroutine = null;
        }
        emotionPanel.SetActive(false);
    }

    // 初始化：默认隐藏面板
    private void Awake()
    {
        emotionPanel.SetActive(false);
    }
}