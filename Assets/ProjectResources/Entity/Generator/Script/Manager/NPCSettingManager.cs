using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;


public class NPCSettingManager : MonoBehaviour
{
    [Header("配置")]
    // public string positivePrompt = "(rpgchara), 1girl, white background, high quality";
    // public string negativePrompt = "low quality, blurry, ugly, watermark";
    public bool useLLMForDialogue = true; // 控制是否使用LLM生成对话，调试时可以关闭以节省token

    [Header("UI显示")]
    public Image resultImage;      // 拖入你的Image
    public Button startButton;     // 开始按钮
    public Text statusText;        // 状态提示
    // 绑定TMP输入框
    public TMP_InputField promptInput;
    public TMP_InputField nameInput;
    public TMP_InputField personalityInput;
    public Dictionary<string, GameObject> generatedNPCs = new Dictionary<string, GameObject>();

    // 确定生成位置
    public Transform generatePosition;

    private void Start()
    {
        startButton.onClick.AddListener(StartGenerate);
        statusText.text = "准备生成";
    
    }

    /// <summary>
    /// 开始生成流程
    /// </summary>
    public void StartGenerate()
    {

        // 检查是否已经生成过
        if (generatedNPCs.ContainsKey(nameInput.text))
        {
            generatedNPCs[nameInput.text].transform.position = transform.position;
            return;
        }

        startButton.interactable = false;
        statusText.text = "正在生成...请等待";

        // 1. 读取工作流
        Dictionary<string, Node> workflow = JsonReader.ReadWorkflowJson();

        // 2. 设置参数：随机种子 + 提示词
        string prompt = promptInput.text;
        Debug.Log("提示词：" + prompt);

        SetupWorkflow(workflow, 
            PromptToolkit.BuildPositivePrompt(prompt), 
            PromptToolkit.GetFixedNegativePrompt()
        );

        // 3. 执行全流程
        ComfyUIClient.Instance.ExecuteComfyUIWorkflow(
            workflow,
            OnGenerateSuccess,
            OnGenerateFailed
        );

        
    }

    /// <summary>
    /// 设置工作流参数（随机种子 + 提示词）
    /// </summary>
    private void SetupWorkflow(Dictionary<string, Node> workflow, string positive, string negative)
    {
        // 随机种子（兼容所有Unity版本）
        System.Random rand = new System.Random();
        long seed = (long)(rand.NextDouble() * 900000000000000L) + 100000000000000L;

        // 设置参数（请确认你的节点ID是否正确！常见：3=采样器 6=正向 7=反向）
        JsonReader.ModifyNodeInput(workflow, "3", "seed", seed);
        JsonReader.ModifyNodeInput(workflow, "6", "text", positive);
        JsonReader.ModifyNodeInput(workflow, "7", "text", negative);
    }

    /// <summary>
    /// 生成成功 → 自动显示Sprite
    /// </summary>
    private void OnGenerateSuccess(List<string> allPaths)
    {
        string firstImagePath = allPaths[0];
        startButton.interactable = true;
        statusText.text = "生成完成！";

        Debug.Log("生成图片路径：" + firstImagePath);

        // 自动转Sprite并显示
        Sprite sprite = TextureConverter.CreateSpriteFromPath(firstImagePath);
        resultImage.sprite = sprite;
        resultImage.preserveAspect = true; // 保持比例

        // 4. 生成NPC
        Debug.Log("生成NPC");
        
        // 设置是否使用LLM生成对话
        NPCSpriteTool.useLLMForDialogue = useLLMForDialogue;
        Debug.Log($"设置 useLLMForDialogue: {useLLMForDialogue}");
        
        GameObject npc = NPCSpriteTool.CreateNPCFromSprite(sprite, transform.position, nameInput.text, personalityInput.text);
        generatedNPCs.Add(nameInput.text, npc);
        npc.transform.SetParent(generatePosition);
        npc.transform.localPosition = GenerateRandomPosition(generatePosition, 1f, 3f);
    }

    // 生成随机位置
    private Vector3 GenerateRandomPosition(Transform position, float minRadius, float maxRadius)
    {
        // 生成最小半径和最大半径之间的随机位置
        // 随机半径
        float radius = Random.Range(minRadius, maxRadius);
        // 随机角度
        float angle = Random.Range(0, 360);
        
        // 随机位置
        Vector3 randomPos = new Vector3(
            radius * Mathf.Cos(angle * Mathf.Deg2Rad),
            radius * Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );
        // 应用到NPC
        return randomPos;
    }

    /// <summary>
    /// 生成失败
    /// </summary>
    private void OnGenerateFailed(string error)
    {
        startButton.interactable = true;
        statusText.text = "错误：" + error;
        Debug.LogError(error);
    }

}
