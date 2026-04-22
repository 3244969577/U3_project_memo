using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System.Threading.Tasks;
using RPG.AI.Utility;
using System;
using System.Runtime.CompilerServices;
using ProjectResources.Utils.Files;
using GlobalEvents;
using Newtonsoft.Json;
using System.Reflection;
using ComfyUI;

public class NPCSettingManagerAsync : Singleton<NPCSettingManagerAsync>
{
    [Header("配置")]
    public bool useLLMForDialogue = true; // 控制是否使用LLM生成对话，调试时可以关闭以节省token
    public bool useDefaultSprite = true; // 控制是否使用默认的Sprite图片

    [Header("UI显示")]
    public Image resultImage;      // 拖入你的Image
    public Button startButton;     // 开始按钮
    public Text statusText;        // 状态提示


    [Header("UI 输入框")]
    // 绑定TMP输入框
    public TMP_InputField promptInput;
    public TMP_InputField nameInput;
    public TMP_InputField personalityInput;

#region UI事件
    public void onStartClick()
    {
        StartGenerate();
    }
#endregion

    private async void StartGenerate() {
        string name = nameInput.text;
        string personality = personalityInput.text;
        string prompt = promptInput.text;

        //  检测是否已经生成该NPC
        if (NPCManager.Instance.IsNPCExist(name))
        {
            statusText.text = "NPC已存在";
            NPCManager.Instance.GetNPC(name).transform.position = transform.position;
            return;
        } else {
            statusText.text = "正在生成...请等待";
        }


#region 提交请求
        // 开始生成NPC事件
        EventBus<NPCStartGenerateEvent>.Raise(new NPCStartGenerateEvent { 
            name = name, 
            personality = personality, 
            prompt = prompt 
        });
        Dictionary<string, Node> workflow = JsonReader.ReadWorkflowJson();

        // 更新嵌入提示词
        initPrompt(
            workflow,
            PromptToolkit.BuildPositivePrompt(prompt),
            PromptToolkit.GetFixedNegativePrompt()
        );
        Debug.Log(workflow);

        // 获取Sprite图片
        Debug.Log("GetSprite");
        Task<Sprite> spriteTask =  GetSprite(workflow, prompt);
        
        // 获取LLM的JSON
        Task<string> responseTask = GetChatRequestAsync();
        Debug.Log("await");
        // 等待所有任务完成，确保图片和JSON都获取到
        await Task.WhenAll(spriteTask, responseTask);
#endregion

        
        Sprite sprite = spriteTask.Result;
        string response = responseTask.Result;

#region 创建NPC
        // 使用sprite创建一个NPC
        GameObject npc = CreateNPCAround(sprite);
        npc.name = name;

        InitNPCSetting(npc, name, personality);
        // 构建对话流程
        if (useLLMForDialogue && response != null)
        {
            string jsonRsp = response;
            BuildFlowchart(npc, name, jsonRsp);
        } else {
            // 使用默认配置
            BuildDefaultFlowchart(npc, name);
        }
#endregion

        // 生成NPC事件
        EventBus<NPCGeneratedEvent>.Raise(new NPCGeneratedEvent { npc = npc });
    }


#region NPC处理
    private void InitNPCSetting(GameObject npc, string name, string personality)
    {
        // 初始化DialogConfig
        NPCDialogueConfig config = npc.GetComponentInChildren<NPCDialogueConfig>();
        
        // string NPCSettingString = "只输出对话，每一句占一行，不能输出其他内容。不能输出任何解释或说明。";
        config.systemPrompt = personality + "\n" + FileUtil.ReadFileAsString("Texts/LLMRspRule.md");
        config.npcName = name;
        config.npcId = name;

        //// 设置可选参数
        config.temperature = 0.7f;
        config.maxHistoryRound = 10;
        config.defaultReply = "我现在有点忙，等会儿再说吧。";

        Debug.Log($"✅ NPC对话配置完成，名称：{name}");
        
        // 初始化Dialogable
        // Dialogable dialogable = npc.GetComponentInChildren<Dialogable>();
        // dialogable.config = config;

        // 初始化Character
        Fungus.Character character = npc.GetComponentInChildren<Fungus.Character>();
        character.SetStandardText(name);
        Debug.Log($"✅ NPC角色配置完成，名称：{name}");

        // 初始化Flowchart
        Fungus.Flowchart flowchart = npc.GetComponentInChildren<Fungus.Flowchart>();
        flowchart.name = name;
        Debug.Log($"✅ NPC流程配置完成，名称：{name}");
    }

    private GameObject CreateNPCAround(Sprite sprite)
    {
        GameObject npc = NPCSpriteToolAsync.Instance.CreateNPCFromSprite(sprite);
        npc.transform.position = GenerateRandomPosition(transform.parent, 1f, 3f);
        Debug.Log(npc);

        return npc;
    }

    private Vector3 GenerateRandomPosition(Transform position, float minRadius, float maxRadius)
    {
        // 生成最小半径和最大半径之间的随机位置
        // 随机半径
        float radius = UnityEngine.Random.Range(minRadius, maxRadius);
        // 随机角度
        float angle = UnityEngine.Random.Range(0, 360);
        
        // 随机位置
        Vector3 randomPos = new Vector3(
            radius * Mathf.Cos(angle * Mathf.Deg2Rad),
            radius * Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        );
        // 应用到NPC
        return position.position + randomPos;
    }
#endregion


#region 构建对话流程
    private void BuildFlowchart(GameObject npc, string name, string jsonRsp) 
    {
        if (string.IsNullOrEmpty(jsonRsp))
        {
            // 使用默认配置
            
            return;
        }

        jsonRsp = jsonRsp.Trim();
        List<DialogueNode> nodes = ParseDialogueFromResponse(jsonRsp);


        AIDialogueConfig config = new AIDialogueConfig();
        config.character = name;
        config.dialogue = nodes.ToArray();


        Fungus.Flowchart flowchart = npc.GetComponentInChildren<Fungus.Flowchart>();
        FungusNodeBuilder.BuildNodeDialogue(flowchart, config);
        Debug.Log($"✅ 对话流程生成完成，节点数：{nodes.Count}");
    }

    private void BuildDefaultFlowchart(GameObject npc, string name)
    {
        Fungus.Flowchart flowchart = npc.GetComponentInChildren<Fungus.Flowchart>();

        AIDialogueConfig aiConfig = new AIDialogueConfig();
        aiConfig.character = name;
        
        List<DialogueNode> dialogueNodes = new List<DialogueNode>();
        
        DialogueNode node1 = new DialogueNode();
        node1.id = "1";
        node1.text = "你好，旅行者！";
        node1.branches = new DialogueBranch[] {
            new DialogueBranch() { optionText = "你好", nextDialogue = "2" },
            new DialogueBranch() { optionText = "关闭对话", nextDialogue = "3" }
        };
        dialogueNodes.Add(node1);

        DialogueNode node2 = new DialogueNode();
        node2.id = "2";
        node2.text = "欢迎来到我们的村庄！";
        node2.branches = new DialogueBranch[] {
            new DialogueBranch() { optionText = "谢谢", nextDialogue = "1" },
            new DialogueBranch() { optionText = "关闭对话", nextDialogue = "3" }
        };
        dialogueNodes.Add(node2);

        DialogueNode node3 = new DialogueNode();
        node3.id = "3";
        node3.text = "祝你有美好的一天！";
        // 结束对话节点，没有分支选项
        node3.branches = new DialogueBranch[0];
        dialogueNodes.Add(node3);
        
        aiConfig.dialogue = dialogueNodes.ToArray();

        FungusNodeBuilder.BuildNodeDialogue(flowchart, aiConfig);
        Debug.Log($"✅ 默认对话流程生成完成");
    }

    private static List<DialogueNode> ParseDialogueFromResponse(string response)
    {
        List<DialogueNode> nodes = new List<DialogueNode>();
        
        try {
            // 使用 Newtonsoft.Json 解析 LLM 返回的 JSON
            var jsonObj = JsonConvert.DeserializeObject<DialogueResponse>(response);
            
            if (jsonObj?.dialogue != null)
            {
                foreach (var node in jsonObj.dialogue)
                {
                    DialogueNode dialogueNode = new DialogueNode();
                    dialogueNode.id = node.id;
                    dialogueNode.text = node.text;
                    
                    if (node.branches != null)
                    {
                        List<DialogueBranch> branches = new List<DialogueBranch>();
                        foreach (var branch in node.branches)
                        {
                            DialogueBranch dialogueBranch = new DialogueBranch();
                            dialogueBranch.optionText = branch.optionText;
                            dialogueBranch.nextDialogue = branch.nextDialogue;
                            branches.Add(dialogueBranch);
                        }
                        dialogueNode.branches = branches.ToArray();
                    }
                    nodes.Add(dialogueNode);
                }
                
                Debug.Log($"✅ 成功解析 {nodes.Count} 个对话节点");
                return nodes;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"JSON 解析失败：{e.Message}");
            Debug.LogError($"原始响应：{response}");
        }
        
        // 解析失败时返回空列表，让调用方使用默认对话
        return nodes;
    }


    #region 构建对话流程相关结构
    // LLM 返回的对话数据结构
    [System.Serializable]
    private class DialogueResponse
    {
        [JsonProperty("dialogue")]
        public List<DialogueResponseNode> dialogue;
    }

    [System.Serializable]
    private class DialogueResponseNode
    {
        [JsonProperty("id")]
        public string id;

        [JsonProperty("text")]
        public string text;

        [JsonProperty("branches")]
        public List<DialogueResponseBranch> branches;
    }

    [System.Serializable]
    private class DialogueResponseBranch
    {
        [JsonProperty("optionText")]
        public string optionText;

        [JsonProperty("nextDialogue")]
        public string nextDialogue;
    }
    #endregion


#endregion


#region API请求

    private void initPrompt(Dictionary<string, Node> workflow, string positive, string negative) 
    {
        // 随机种子（兼容所有Unity版本）
        System.Random rand = new System.Random();
        long seed = (long)(rand.NextDouble() * 900000000000000L) + 100000000000000L;

        // 设置参数（请确认你的节点ID是否正确！常见：3=采样器 6=正向 7=反向）
        JsonReader.ModifyNodeInput(workflow, "3", "seed", seed);
        JsonReader.ModifyNodeInput(workflow, "6", "text", positive);
        JsonReader.ModifyNodeInput(workflow, "7", "text", negative);
    }
    
    private async Task<Sprite> GetSprite(Dictionary<string, Node> workflow, string prompt)
    {
        string path = await GetSpritePaths(workflow);
        Sprite sprite = TextureConverter.CreateSpriteFromPath(path);

        return sprite;
    }

    private async Task<string> GetSpritePaths(Dictionary<string, Node> workflow)
    {
        // return ComfyUIClient.Instance.
        // return "Assets\\GeneratedImages\\Char_0087.png";
        if (useDefaultSprite)
        {
            return "Assets\\GeneratedImages\\Char_0087.png";
        }
        return await ComfyUIClient.Instance.ExecuteComfyUIWorkflowAsync(workflow);
    }

    private async Task<string> GetChatRequestAsync()
    {
        if (!useLLMForDialogue)
        {
            Debug.Log("useLLMForDialogue = false");
            return null;
        }

        string personality = personalityInput.text;
        
        string personalityPrompt = FileUtil.ReadFileAsString("Texts/APISystemSetting.md");
        personalityPrompt += $@"\n NPC name: {name} \n Personality Setting: {personality}";
        Debug.Log("GetChatRequestAsync");

        return await DialogJSONAPI.Instance.SendChatRequestAsync(personalityPrompt);
    }

#endregion


}
