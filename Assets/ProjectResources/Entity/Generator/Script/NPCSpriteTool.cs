using UnityEngine;
using Fungus;
using System.Collections.Generic;
using RPG.AI.Utility;
using ProjectResources.Utils.Files;
using Newtonsoft.Json;

public static class NPCSpriteTool
{
    [Header("调试设置")]
    public static bool useLLMForDialogue = true; // 控制是否使用LLM生成对话，调试时可以关闭以节省token
    
    public static GameObject CreateNPCFromSprite(Sprite fourDirWalkSprite)
    {
        return CreateNPCFrom4DirSprite(fourDirWalkSprite, Vector3.zero, "Generated_4DirNPC", "");
    }

    public static GameObject CreateNPCFromSprite(Sprite fourDirWalkSprite, Vector3 position, string name, string personalitySetting)
    {
        return CreateNPCFrom4DirSprite(fourDirWalkSprite, position, name, personalitySetting);
    }

    public static GameObject CreateNPCFrom4DirSprite(Sprite fourDirWalkSprite, Vector3 position, string name, string personalitySetting)
    {
        Vector3 spawnPos = position;
        GameObject npcObj = new GameObject(name);
        npcObj.transform.position = spawnPos;
        npcObj.transform.localScale = new Vector3(6, 6, 1); // 放大到原本的6倍

        // 基础组件
        SpriteRenderer sr = npcObj.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 10;

        // 添加NPC_Genearted组件
        npcObj.AddComponent<NPC_Genearted>();

        BoxCollider2D col = npcObj.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.3f, 0.4f);
        col.offset = new Vector2(0, 0f);
        col.isTrigger = true;

        // 切割合图
        Sprite[,] dirFrameSprites = Split4Dir3FrameSprite(fourDirWalkSprite);

        // 绑定双脚本（自动关联）
        NPC4DirFrameAnimator anim = npcObj.AddComponent<NPC4DirFrameAnimator>();
        anim.SetSpriteRenderer(sr);
        anim.SetSprites(dirFrameSprites);

        // 添加移动脚本
        NPCMovement npcMovement = npcObj.AddComponent<NPCMovement>();
        npcMovement.moveRange = 5f; // 移动范围

        CreateDialogConfiguration(npcObj, name, personalitySetting);

        Debug.Log($"✅ NPC生成完成（动画+移动解耦）| 位置：{spawnPos}");
        return npcObj;
    }
    private static void CreateDialogConfiguration(GameObject npcObj, string name, string personalitySetting)
    {
        string characterSetting = personalitySetting;
        string systemSetting = "只输出对话，每一句占一行，不能输出其他内容。不能输出任何解释或说明。";
        // 创建一个子物体，挂载组件DialogConfig
        GameObject dialogConfig = new GameObject("DialogConfig");
        dialogConfig.transform.SetParent(npcObj.transform);

        NPCDialogueConfig config = dialogConfig.AddComponent<NPCDialogueConfig>();
        config.systemPrompt = characterSetting + "\n" + systemSetting;
        config.npcName = name;
        config.npcId = name;
        
        // 可选参数
        config.temperature = 0.7f;
        config.maxHistoryRound = 10;
        config.defaultReply = "我现在有点忙，等会儿再说吧。";
        Debug.Log($"✅ NPC对话配置完成，名称：{name}");

        // 添加组件NPCDialogue
        NPCDialogue npcDialogue = npcObj.AddComponent<NPCDialogue>();
        npcDialogue.config = config;

        // 添加组件 Dialogable
        Dialogable dialogable = npcObj.AddComponent<Dialogable>();
        dialogable.config = config;

        // 创建一个子物体，名称为Character_{name}
        GameObject characterConfig = new GameObject($"Character_{name}");
        characterConfig.transform.SetParent(npcObj.transform);

        Fungus.Character character = characterConfig.AddComponent<Fungus.Character>();
        character.SetStandardText(name);
        Debug.Log($"✅ NPC角色配置完成，名称：{name}");

        // 创建一个子物体flowchart
        GameObject flowchartConfig = new GameObject($"Flowchart_{name}");
        flowchartConfig.transform.SetParent(npcObj.transform);

        Flowchart flowchart = flowchartConfig.AddComponent<Flowchart>();
        flowchart.name = name;

        // 使用LLM生成对话流程
        GenerateDialogueFlowchart(npcObj, name, personalitySetting, flowchart);
    }

    private static void GenerateDialogueFlowchart(GameObject npcObj, string name, string personalitySetting, Flowchart flowchart)
    {
        if (useLLMForDialogue)
        {
            string prompt = FileUtil.ReadFileAsString("Texts/APISystemSetting.md");
            prompt += $@"\nNPC name: {name}
\nPersonality Setting: {personalitySetting}";

            Debug.Log($"使用LLM生成设定");
            // 调用LLM API
            DialogJSONAPI.Instance.SendChatRequest(prompt, 
                (response) => {
                    try {
                        // 解析LLM返回的JSON
                        string llmResponse = response.Choices[0].Message.Content;
                        // Debug.Log($"LLM Response: {llmResponse}");

                        // 清理JSON字符串（移除可能的标记）
                        llmResponse = llmResponse.Trim();
                        Debug.Log($"Resp JSON: {llmResponse}");
                        // if (llmResponse.StartsWith("```json")) {
                        //     llmResponse = llmResponse.Substring(7);
                        // }
                        // if (llmResponse.EndsWith("```")) {
                        //     llmResponse = llmResponse.Substring(0, llmResponse.Length - 3);
                        // }

                        // 解析为 AIDialogueConfig
                        AIDialogueConfig aiConfig = new AIDialogueConfig();
                        aiConfig.character = name;
                        
                        // 使用 Newtonsoft.Json 解析 LLM 返回的 JSON
                        List<DialogueNode> dialogueNodes = ParseDialogueFromResponse(llmResponse);
                        aiConfig.dialogue = dialogueNodes.ToArray();

                        // 如果解析失败或节点为空，使用默认对话
                        if (aiConfig.dialogue == null || aiConfig.dialogue.Length == 0)
                        {
                            Debug.LogWarning("⚠️ LLM 返回数据解析失败，使用默认对话流程");
                            GenerateDefaultDialogueFlowchart(flowchart, name);
                            return;
                        }

                        // 构建 Fungus flowchart
                        FungusNodeBuilder.BuildNodeDialogue(flowchart, aiConfig);
                        Debug.Log($"✅ 对话流程生成完成，节点数：{aiConfig.dialogue.Length}");
                    } catch (System.Exception e) {
                        Debug.LogError($"生成对话流程失败：{e.Message}");
                        // 生成默认对话流程
                        GenerateDefaultDialogueFlowchart(flowchart, name);
                    }
                }, 
                (error) => {
                    Debug.LogError($"LLM API调用失败：{error}");
                    // 生成默认对话流程
                    GenerateDefaultDialogueFlowchart(flowchart, name);
                });
        }
        else
        {
            // 直接使用默认对话流程，避免LLM调用
            Debug.Log("⚠️ 跳过LLM调用，使用默认对话流程");
            GenerateDefaultDialogueFlowchart(flowchart, name);
        }
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

    private static void GenerateDefaultDialogueFlowchart(Flowchart flowchart, string name)
    {
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

    

    private static Sprite[,] Split4Dir3FrameSprite(Sprite fullSprite)
    {
        Texture2D tex = fullSprite.texture;
        int frameW = tex.width / 3;
        int frameH = tex.height / 4;
        Sprite[,] sprites = new Sprite[4, 3];

        for (int dir = 0; dir < 4; dir++)
        {
            for (int frame = 0; frame < 3; frame++)
            {
                int x = frame * frameW;
                int y = tex.height - frameH * (dir + 1);
                sprites[dir, frame] = Sprite.Create(tex, new Rect(x, y, frameW, frameH), new Vector2(0.5f, 0.5f), tex.width);
            }
        }
        Debug.Log($"✅ 合图切割完成，方向：{sprites.GetLength(0)}，帧：{sprites.GetLength(1)}");
        return sprites;
    }

}


