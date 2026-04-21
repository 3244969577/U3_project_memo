// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using System.IO;


// /// <summary>
// /// 测试ComfyUI全流程：生成 → 显示Sprite
// /// </summary>
// public class ComfyUITestWorkflow : MonoBehaviour
// {
//     [Header("配置")]
//     public string positivePrompt = "(rpgchara), 1girl, white background, high quality";
//     public string negativePrompt = "low quality, blurry, ugly, watermark";

//     [Header("UI显示")]
//     public Image resultImage;      // 拖入你的Image
//     public Button startButton;     // 开始按钮
//     public Text statusText;        // 状态提示

//     private void Start()
//     {
//         startButton.onClick.AddListener(StartGenerate);
//         statusText.text = "准备生成";
//     }

//     /// <summary>
//     /// 开始生成流程
//     /// </summary>
//     public void StartGenerate()
//     {
//         startButton.interactable = false;
//         statusText.text = "正在生成...请等待";

//         // 1. 读取工作流
//         Dictionary<string, Node> workflow = JsonReader.ReadWorkflowJson();

//         // 2. 设置参数：随机种子 + 提示词
//         SetupWorkflow(workflow, 
//             PromptToolkit.BuildPositivePrompt(positivePrompt), 
//             PromptToolkit.GetFixedNegativePrompt()
//         );

//         // 3. 执行全流程
//         ComfyUIClient.Instance.ExecuteComfyUIWorkflow(
//             workflow,
//             OnGenerateSuccess,
//             OnGenerateFailed
//         );

        
//     }

//     /// <summary>
//     /// 设置工作流参数（随机种子 + 提示词）
//     /// </summary>
//     private void SetupWorkflow(Dictionary<string, Node> workflow, string positive, string negative)
//     {
//         // 随机种子（兼容所有Unity版本）
//         System.Random rand = new System.Random();
//         long seed = (long)(rand.NextDouble() * 900000000000000L) + 100000000000000L;

//         // 设置参数（请确认你的节点ID是否正确！常见：3=采样器 6=正向 7=反向）
//         JsonReader.ModifyNodeInput(workflow, "3", "seed", seed);
//         JsonReader.ModifyNodeInput(workflow, "6", "text", positive);
//         JsonReader.ModifyNodeInput(workflow, "7", "text", negative);
//     }

//     /// <summary>
//     /// 生成成功 → 自动显示Sprite
//     /// </summary>
//     private void OnGenerateSuccess(List<string> allPaths)
//     {
//         string firstImagePath = allPaths[0];
//         startButton.interactable = true;
//         statusText.text = "生成完成！";

//         Debug.Log("生成图片路径：" + firstImagePath);

//         // 自动转Sprite并显示
//         Sprite sprite = TextureConverter.CreateSpriteFromPath(firstImagePath);
//         resultImage.sprite = sprite;
//         resultImage.preserveAspect = true; // 保持比例

//         // 4. 生成NPC
//         Debug.Log("生成NPC");
//         NPCSpriteTool.CreateNPCFromSprite(sprite);
//     }

//     /// <summary>
//     /// 生成失败
//     /// </summary>
//     private void OnGenerateFailed(string error)
//     {
//         startButton.interactable = true;
//         statusText.text = "错误：" + error;
//         Debug.LogError(error);
//     }

    
// }