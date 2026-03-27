using UnityEngine;
using UnityEngine.UI;
using Fungus;

/// <summary>
/// 可对话组件 - 处理NPC与玩家的交互逻辑
/// - 检测玩家靠近
/// - 显示交互提示
/// - 处理E键触发
/// </summary>
public class Dialogable : MonoBehaviour
{
    public Flowchart flowchart;
    private bool isInteractable = false;
    private NPCDialogue npcDialogue;
    private NPC npcAI;
    
    [Header("交互提示")]
    public float promptHeight = 1.5f; // 提示显示的高度
    public float promptDuration = 3f; // 提示显示的持续时间
    
    private GameObject promptObject;
    private Text promptText;
    private float promptTimer = 0f;
    private bool isPromptShowing = false;
    
    void Start()
    {
        // flowchart = GameManager.instance.flowchart;
        npcDialogue = GetComponent<NPCDialogue>();
        Debug.Assert(npcDialogue != null, "NPCDialogue component is null");
        npcAI = GetComponent<NPC>();
        Debug.Assert(npcAI != null, "NPC component is null");
        
        // 创建提示对象
        CreatePromptObject();
    }
    
    void Update()
    {
        if (isInteractable && Input.GetKeyDown(KeyCode.E))
        {
            flowchart.ExecuteBlock(npcAI.npcDialogueConfig.npcName);
            GameManager.instance.SetNPCInteracting(gameObject);
            // 冻结移动的逻辑放在fungus画布中

            HidePrompt();
        }
        
        // 更新提示位置和计时器
        if (isPromptShowing)
        {
            UpdatePromptPosition();
            promptTimer -= Time.deltaTime;
            if (promptTimer <= 0)
            {
                HidePrompt();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInteractable = true;
            ShowPrompt();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInteractable = false;
            HidePrompt();
        }
    }
    
    /// <summary>
    /// 创建提示对象
    /// </summary>
    private void CreatePromptObject()
    {
        // 创建提示容器
        promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(transform);
        promptObject.transform.localPosition = new Vector3(0, promptHeight, 0);
        
        // 添加Canvas组件
        Canvas canvas = promptObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 100;
        
        // 设置Canvas大小
        RectTransform canvasRect = promptObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(200, 50);
        canvasRect.localScale = new Vector3(0.03f, 0.03f, 1f);
        
        // 添加文本组件
        GameObject textObj = new GameObject("PromptText");
        textObj.transform.SetParent(promptObject.transform);
        textObj.transform.localPosition = Vector3.up * 10;
        textObj.transform.localScale = new Vector3(1f, 1f, 1f);
        
        promptText = textObj.AddComponent<Text>();
        promptText.text = "按下E键进行交互";
        promptText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        promptText.fontSize = 24;
        promptText.alignment = TextAnchor.MiddleCenter;
        promptText.color = Color.white;
        
        // 设置文本矩形
        RectTransform textRect = promptText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(200, 50);
        
        // // 添加背景
        // GameObject bgObj = new GameObject("Background");
        // bgObj.transform.SetParent(promptObject.transform);
        // bgObj.transform.localPosition = Vector3.zero;
        // bgObj.transform.SetSiblingIndex(0);
        
        // Image bgImage = bgObj.AddComponent<Image>();
        // bgImage.color = new Color(0, 0, 0, 0.7f);
        
        // RectTransform bgRect = bgImage.GetComponent<RectTransform>();
        // bgRect.sizeDelta = new Vector2(220, 60);
        
        // 初始隐藏
        promptObject.SetActive(false);
    }
    
    /// <summary>
    /// 显示提示
    /// </summary>
    private void ShowPrompt()
    {
        if (promptObject != null)
        {
            promptObject.SetActive(true);
            isPromptShowing = true;
            promptTimer = promptDuration;
            UpdatePromptPosition();
        }
    }
    
    /// <summary>
    /// 隐藏提示
    /// </summary>
    private void HidePrompt()
    {
        if (promptObject != null)
        {
            promptObject.SetActive(false);
            isPromptShowing = false;
            promptTimer = 0f;
        }
    }
    
    /// <summary>
    /// 更新提示位置
    /// </summary>
    private void UpdatePromptPosition()
    {
        if (promptObject != null)
        {
            promptObject.transform.position = transform.position + new Vector3(0, promptHeight, 0);
        }
    }
}
