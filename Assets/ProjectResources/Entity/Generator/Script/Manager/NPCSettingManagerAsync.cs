// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using System.IO;
// using TMPro;
// using System.Threading.Tasks;
// using RPG.AI.Utility;
// using System.Diagnostics;

// public class NPCSettingManagerAsync : ManagerBase
// {
//     public static NPCSettingManagerAsync Instance => (NPCSettingManagerAsync)instance;

//     [Header("配置")]
//     public bool useLLMForDialogue = true; // 控制是否使用LLM生成对话，调试时可以关闭以节省token

//     [Header("UI显示")]
//     public Image resultImage;      // 拖入你的Image
//     public Button startButton;     // 开始按钮
//     public Text statusText;        // 状态提示
//     // 绑定TMP输入框
//     public TMP_InputField promptInput;
//     public TMP_InputField nameInput;
//     public TMP_InputField personalityInput;
//     // public Dictionary<string, GameObject> generatedNPCs = new Dictionary<string, GameObject>();

//     // 确定生成位置
//     public Transform generatePosition;

//     private void Start()
//     {
//         startButton.onClick.AddListener(StartGenerate);
//         statusText.text = "准备生成";
//     }

//     /// <summary>
//     /// 开始生成流程
//     /// </summary>
//     public async void StartGenerate()
//     {
//         if (NPCManager.Instance.IsNPCExist(nameInput.text))
//         {
//             statusText.text = "NPC已存在";
//             NPCManager.Instance.GetNPC(nameInput.text).transform.position = transform.position;
//             return;
//         }

//         startButton.interactable = false;
//         statusText.text = "正在生成...请等待";

//         // 1. 获取工作流
//         Dictionary<string, Node> workflow = GetWorkflow();

//         var fungusJson = GetFungusJson(nameInput.text, personalityInput.text);
//         var spritePaths = GetSpritePaths();
//         await Task.WhenAll(fungusJson, spritePaths);

//         string response = fungusJson.Result.Choices[0].Message.Content;
//         Debug.Log($"Resp JSON: {response}");

//         string filename = spritePaths.Result[0];
//         Debug.Log($"Sprite路径: {filename}");
//         statusText.text = "生成完成！";


//         Sprite sprite = TextureConverter.CreateSpriteFromPath(filename);
//         resultImage.sprite = sprite;
//         resultImage.preserveAspect = true; // 保持比例
        
//         // 4. 生成NPC
//         Debug.Log("生成NPC");
//         GameObject npc = NPCSpriteToolAsync.Instance.CreateNPCFromSprite(sprite);
//         npc.transform.SetParent(generatePosition);
//         npc.transform.localPosition = GenerateRandomPosition(generatePosition, 1f, 3f);
//         npc.name = nameInput.text;

//         ProcessDialogableNPC(npc);
//     }

//     private void ProcessDialogableNPC(GameObject npc)
//     {
//         string characterSetting = personalityInput.text;
//         string systemSetting = "只输出对话，每一句占一行，不能输出其他内容。不能输出任何解释或说明。";
//         // 创建一个子物体，挂载组件DialogConfig
//         GameObject dialogConfig = npc.Find("DialogConfig");
//         if (dialogConfig == null)
//         {
//             dialogConfig = new GameObject("DialogConfig");
//             dialogConfig.transform.SetParent(npc.transform);
//         }        
        
//         string name = npc.name;
//         NPCDialogueConfig config = dialogConfig.GetComponent<NPCDialogueConfig>();
//         config.systemPrompt = characterSetting + "\n" + systemSetting;
//         config.npcName = name;
//         config.npcId = name;
        
//         // 可选参数
//         config.temperature = 0.7f;
//         config.maxHistoryRound = 10;
//         config.defaultReply = "我现在有点忙，等会儿再说吧。";
//         Debug.Log($"✅ NPC对话配置完成，名称：{name}");

        
//         GameObject characterConfig = npc.Find("Character");
//         if (characterConfig == null)
//         {
//             characterConfig = new GameObject("Character");
//             characterConfig.transform.SetParent(npc.transform);
//         }
//         Fungus.Character character = characterConfig.GetComponent<Fungus.Character>();
//         character.SetStandardText(name);
//         Debug.Log($"✅ NPC角色配置完成，名称：{name}");

        
//         GameObject flowchartConfig = npc.Find("Flowchart");
//         if (flowchartConfig == null)
//         {
//             flowchartConfig = new GameObject("Flowchart");
//             flowchartConfig.transform.SetParent(npc.transform);
//         }
//         Flowchart flowchart = flowchartConfig.GetComponent<Flowchart>();
//         flowchart.name = name;
//     }

//     private void GenerateFlowchart(GameObject npc, string json)
//     {
//         Flowchart flowchart = npc.Find("Flowchart").GetComponent<Flowchart>();
//         List<DialogueNode> nodes = ParseDialogueFromResponse(json);

//         AIDialogueConfig config = new AIDialogueConfig();
//         config.dialogueNodes = nodes.ToArray();
//         config.character = npc.name;

//         FungusNodeBuilder.BuildNodeDialogue(flowchart, config);
//     }
    
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

//     private List<DialogueNode> ParseDialogueFromResponse(string response)
//     {
//         List<DialogueNode> nodes = new List<DialogueNode>();
        
//         try {
//             // 使用 Newtonsoft.Json 解析 LLM 返回的 JSON
//             var jsonObj = JsonConvert.DeserializeObject<DialogueResponse>(response);
            
//             if (jsonObj?.dialogue != null)
//             {
//                 foreach (var node in jsonObj.dialogue)
//                 {
//                     DialogueNode dialogueNode = new DialogueNode();
//                     dialogueNode.id = node.id;
//                     dialogueNode.text = node.text;
                    
//                     if (node.branches != null)
//                     {
//                         List<DialogueBranch> branches = new List<DialogueBranch>();
//                         foreach (var branch in node.branches)
//                         {
//                             DialogueBranch dialogueBranch = new DialogueBranch();
//                             dialogueBranch.optionText = branch.optionText;
//                             dialogueBranch.nextDialogue = branch.nextDialogue;
//                             branches.Add(dialogueBranch);
//                         }
//                         dialogueNode.branches = branches.ToArray();
//                     }
//                     nodes.Add(dialogueNode);
//                 }
                
//                 Debug.Log($"✅ 成功解析 {nodes.Count} 个对话节点");
//                 return nodes;
//             }
//         }
//         catch (System.Exception e)
//         {
//             Debug.LogError($"JSON 解析失败：{e.Message}");
//             Debug.LogError($"原始响应：{response}");
//         }
        
//         // 解析失败时返回空列表，让调用方使用默认对话
//         return nodes;
//     }

//     // 生成随机位置
//     private Vector3 GenerateRandomPosition(Transform position, float minRadius, float maxRadius)
//     {
//         // 生成最小半径和最大半径之间的随机位置
//         // 随机半径
//         float radius = Random.Range(minRadius, maxRadius);
//         // 随机角度
//         float angle = Random.Range(0, 360);
        
//         // 随机位置
//         Vector3 randomPos = new Vector3(
//             radius * Mathf.Cos(angle * Mathf.Deg2Rad),
//             radius * Mathf.Sin(angle * Mathf.Deg2Rad),
//             0
//         );
//         // 应用到NPC
//         return randomPos;
//     }

//     private Dictionary<string, Node> GetWorkflow()
//     {
//         // 1. 读取工作流
//         Dictionary<string, Node> workflow = JsonReader.ReadWorkflowJson();

//         // 2. 设置参数：随机种子 + 提示词
//         string prompt = promptInput.text;
//         Debug.Log("提示词：" + prompt);

//         SetupWorkflow(workflow, 
//             PromptToolkit.BuildPositivePrompt(prompt), 
//             PromptToolkit.GetFixedNegativePrompt()
//         );
//         return workflow;
//     }

//     private async Task<List<string>> GetSpritePaths() {
//         // 默认实现：返回空数组
//         var spritePaths = new List<string>();
//         spritePaths.Add("D:\\课程\\毕业设计\\refs\\rpgshooter2d-main\\RPGShooter\\Assets\\GeneratedImages\\Char_0062.png");
//         return Task.FromResult(spritePaths);
//     }
//     private async Task<ChatCompletionResponse> GetFungusJson(string name, string personality) {
//         string prompt = FileUtil.ReadFileAsString("Texts/APISystemSetting.md");
//         prompt += $@"NPC name: {name}
// Personality Setting: {personality}";

//         return AsyncRequester.Instance.SendChatRequest(prompt);
//     }
// }
