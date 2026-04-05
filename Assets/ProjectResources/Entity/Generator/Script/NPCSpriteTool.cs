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

    // private static Sprite[,] Split4Dir3FrameSprite(Sprite fullSprite)
    // {
    //     Texture2D tex = fullSprite.texture;
    //     int frameCountPerDir = 3;
    //     int directionCount = 4;

    //     int frameWidth = tex.width / frameCountPerDir;
    //     int frameHeight = tex.height / directionCount;

    //     Sprite[,] sprites = new Sprite[directionCount, frameCountPerDir];

    //     for (int dir = 0; dir < directionCount; dir++)
    //     {
    //         for (int frame = 0; frame < frameCountPerDir; frame++)
    //         {
    //             int x = frame * frameWidth;
    //             int y = tex.height - frameHeight * (dir + 1);

    //             sprites[dir, frame] = Sprite.Create(
    //                 tex,
    //                 new Rect(x, y, frameWidth, frameHeight),
    //                 new Vector2(0.5f, 0.5f),
    //                 tex.width
    //             );
    //         }
    //     }
    //     return sprites;
    // }
}



// using UnityEngine;
// using UnityEditor;
// using System.IO;


// public static class NPCSpriteTool
// {
//     /// <summary>
//     /// 创建可正常使用、无动画报错的 NPC（运行时安全）
//     /// </summary>
//     public static GameObject CreateNPCFromSprite(Sprite sprite)
//     {
//         // 1. 创建物体
//         GameObject npc = new GameObject("GameNPC");
//         npc.transform.position = Vector3.zero;

//         // 2. 显示图片
//         SpriteRenderer sr = npc.AddComponent<SpriteRenderer>();
//         sr.sprite = sprite;
//         sr.sortingOrder = 5;

//         // 3. 碰撞
//         BoxCollider2D col = npc.AddComponent<BoxCollider2D>();
//         col.size = new Vector2(1, 2);
//         col.offset = new Vector2(0, 1);

//         // 4. 添加移动脚本（自带4方向动画切换，不报错！）
//         npc.AddComponent<NPCMovement>();

//         Debug.Log("NPC 创建成功 ✅");
//         return npc;
//     }
// }


// public class NPCMovement : MonoBehaviour
// {
//     private SpriteRenderer sr;
//     private Vector2 moveDir;

//     void Awake()
//     {
//         sr = GetComponent<SpriteRenderer>();
//     }

//     void Update()
//     {
//         MoveTest();
//         UpdateAnimation();
//     }

//     void MoveTest()
//     {
//         // 简单自动行走测试
//         moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
//         if (moveDir.magnitude < 0.1f) return;

//         transform.Translate(moveDir * 2f * Time.deltaTime);
//     }

//     void UpdateAnimation()
//     {
//         if (moveDir.magnitude < 0.1f)
//         {
//             // 静止不播放动画 → 不报错！
//             return;
//         }

//         if (moveDir.y < 0) sr.flipX = false;
//         if (moveDir.y > 0) sr.flipX = false;
//         if (moveDir.x < 0) sr.flipX = true;
//         if (moveDir.x > 0) sr.flipX = false;
//     }
// }


// // public static class NPCSpriteTool
// // {
// //     #region 核心公开调用方法
// //     /// <summary>
// //     /// 外部调用的主方法：从精灵图生成完整NPC对象
// //     /// </summary>
// //     /// <param name="sourceSprite">4行3列的精灵图</param>
// //     /// <returns>生成的NPC GameObject</returns>
// //     public static GameObject CreateNPCFromSprite(Sprite sourceSprite)
// //     {
// //         // 1. 验证输入资源合法性
// //         if (!ValidateSpriteSource(sourceSprite))
// //         {
// //             Debug.LogError("NPC生成失败：精灵图资源无效！");
// //             return null;
// //         }

// //         // 2. 切割图集为独立Sprite单元格
// //         Sprite[,] cellSprites = SliceSpriteToCells(sourceSprite, 4, 3);

// //         // 3. 创建NPC根GameObject
// //         GameObject npc = CreateNPCGameObject();

// //         // 4. 添加基础渲染组件（SpriteRenderer）
// //         AddSpriteRenderer(npc, cellSprites[0, 0]);

// //         // 5. 添加动画组件（Animator + Animation）
// //         AddAnimationComponents(npc);

// //         // 6. 生成4方向行走动画剪辑
// //         CreateDirectionAnimations(npc, cellSprites);

// //         // 7. 保存生成的动画文件（可选）
// //         SaveGeneratedAssets(npc, sourceSprite);

// //         Debug.Log($"NPC {npc.name} 生成完成！", npc);
// //         return npc;
// //     }
// //     #endregion

// //     #region 步骤1：验证精灵图资源
// //     /// <summary>
// //     /// 验证输入的Sprite是否符合4x3规范
// //     /// </summary>
// //     private static bool ValidateSpriteSource(Sprite source)
// //     {
// //         if (source == null) return false;

// //         // 获取源Texture2D
// //         Texture2D texture = source.texture;
// //         if (texture == null) return false;

// //         // 必须是正方形单元格（4行3列，宽高必须满足 3W : 4W）
// //         float cellSize = texture.width / 3f;
// //         if (!Mathf.Approximately(cellSize * 4, texture.height))
// //         {
// //             Debug.LogError("精灵图尺寸不符合规范：必须是 3列×4行 的正方形单元格！");
// //             return false;
// //         }

// //         return true;
// //     }
// //     #endregion

// //     #region 步骤2：切割图集为4行3列Sprite数组
// //     /// <summary>
// //     /// 将大图切割为二维Sprite数组 [行,列]
// //     /// </summary>
// //     private static Sprite[,] SliceSpriteToCells(Sprite source, int rowCount, int colCount)
// //     {
// //         Texture2D texture = source.texture;
// //         Sprite[,] sprites = new Sprite[rowCount, colCount];

// //         float cellSize = texture.width / colCount;

// //         // 遍历4行3列，提取所有单元格Sprite
// //         for (int row = 0; row < rowCount; row++)
// //         {
// //             for (int col = 0; col < colCount; col++)
// //             {
// //                 // 计算单元格矩形（Unity纹理坐标：左下角为原点）
// //                 Rect rect = new Rect(
// //                     col * cellSize,
// //                     texture.height - (row + 1) * cellSize,
// //                     cellSize,
// //                     cellSize
// //                 );

// //                 // 创建单个单元格Sprite
// //                 sprites[row, col] = Sprite.Create(
// //                     texture,
// //                     rect,
// //                     new Vector2(0.5f, 0.5f),
// //                     cellSize,
// //                     0,
// //                     SpriteMeshType.FullRect
// //                 );
// //                 sprites[row, col].name = $"Dir{row}_Frame{col}";
// //             }
// //         }

// //         return sprites;
// //     }
// //     #endregion

// //     #region 步骤3：创建NPC根对象
// //     /// <summary>
// //     /// 创建空的NPC根GameObject
// //     /// </summary>
// //     private static GameObject CreateNPCGameObject()
// //     {
// //         GameObject npc = new GameObject("AutoGenerated_NPC");
// //         npc.transform.position = Vector3.zero;
// //         return npc;
// //     }
// //     #endregion

// //     #region 步骤4：添加Sprite渲染器
// //     /// <summary>
// //     /// 为NPC添加SpriteRenderer并设置默认精灵
// //     /// </summary>
// //     private static void AddSpriteRenderer(GameObject npc, Sprite defaultSprite)
// //     {
// //         SpriteRenderer renderer = npc.AddComponent<SpriteRenderer>();
// //         renderer.sprite = defaultSprite;
// //         renderer.sharedMaterial = new Material(Shader.Find("Sprites/Default"));
// //     }
// //     #endregion

// //     #region 步骤5：添加动画核心组件
// //     /// <summary>
// //     /// 添加Animator、Animation组件
// //     /// </summary>
// //     private static void AddAnimationComponents(GameObject npc)
// //     {
// //         npc.AddComponent<Animator>();
// //         npc.AddComponent<Animation>();
// //     }
// //     #endregion

// //     #region 步骤6：生成4方向行走动画
// //     /// <summary>
// //     /// 根据切割好的Sprite生成前、左、右、后4个动画
// //     /// </summary>
// //     private static void CreateDirectionAnimations(GameObject npc, Sprite[,] sprites)
// //     {
// //         Animation animation = npc.GetComponent<Animation>();
// //         string[] directionNames = { "Walk_Front", "Walk_Left", "Walk_Right", "Walk_Back" };

// //         // 遍历4个方向
// //         for (int dir = 0; dir < 4; dir++)
// //         {
// //             // 创建动画剪辑
// //             AnimationClip clip = new AnimationClip();
// //             clip.name = directionNames[dir];
// //             // 设置为循环动画
// //             clip.wrapMode = WrapMode.Loop;

// //             // 设置动画曲线（3帧，每帧0.15秒，可调整）
// //             EditorCurveBinding curveBinding = EditorCurveBinding.PPtrCurve(
// //                 "",
// //                 typeof(SpriteRenderer),
// //                 "m_Sprite"
// //             );

// //             ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[3];
// //             for (int frame = 0; frame < 3; frame++)
// //             {
// //                 keyFrames[frame] = new ObjectReferenceKeyframe
// //                 {
// //                     time = frame * 0.15f,
// //                     value = sprites[dir, frame]
// //                 };
// //             }

// //             // 将曲线绑定到动画
// //             AnimationUtility.SetObjectReferenceCurve(clip, curveBinding, keyFrames);

// //             // 添加到Animation组件
// //             animation.AddClip(clip, clip.name);
// //         }

// //         // 默认播放向前动画
// //         animation.Play("Walk_Front");
// //     }
// //     #endregion
// // #region 步骤7：保存生成的动画资源（可选）
// // /// <summary>
// // /// 将生成的动画剪辑保存为本地文件
// // /// </summary>
// // private static void SaveGeneratedAssets(GameObject npc, Sprite source)
// // {
    
// //     string sourcePath = AssetDatabase.GetAssetPath(source);
// //     string saveDir = Path.GetDirectoryName(sourcePath) + "/NPC_Generated";
// //     Debug.Log($"保存生成的动画资源到 {saveDir}");

// //     if (!Directory.Exists(saveDir))
// //         Directory.CreateDirectory(saveDir);

// //     Animation animation = npc.GetComponent<Animation>();
    
// //     // 修复：不用 GetClips()，改用手动获取所有动画剪辑
// //     string[] directionNames = { "Walk_Front", "Walk_Left", "Walk_Right", "Walk_Back" };
// //     foreach (string dirName in directionNames)
// //     {
// //         AnimationClip clip = animation.GetClip(dirName);
// //         if (clip != null)
// //         {
// //             string clipPath = $"{saveDir}/{npc.name}_{clip.name}.anim";
// //             AssetDatabase.CreateAsset(clip, clipPath);
// //         }
// //     }

// //     AssetDatabase.SaveAssets();
// //     AssetDatabase.Refresh();
// // }
// // #endregion
// // }



