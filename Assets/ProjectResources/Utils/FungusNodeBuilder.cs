using Fungus;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using RPG.AI.Utility;
// using ProjectResources.UI.Script;

public static class FungusNodeBuilder
{
    /// <summary>
    /// 【核心】根据AI对话配置，动态生成完整的节点式Fungus对话
    /// </summary>
    public static void BuildNodeDialogue(Flowchart flowchart, AIDialogueConfig config)
    {
        Debug.Log($"开始构建节点式对话流程，对话节点数：{config.dialogue.Length}");
        
        // 字典：对话ID → Fungus块（快速跳转）
        Dictionary<string, Block> nodeBlockDict = new Dictionary<string, Block>();

        // 1. 遍历所有对话节点，为每个ID创建Fungus块
        for (int i = 0; i < config.dialogue.Length; i++)
        {
            DialogueNode node = config.dialogue[i];
            // 计算块的位置，避免重叠
            Vector2 blockPosition = new Vector2(i * 200, 0);
            // 第一个节点使用角色名作为 BlockName，其他节点使用 Node_{id} 格式
            bool isFirstNode = (i == 0);
            Block nodeBlock = CreateNodeBlock(flowchart, node, config.character, blockPosition, isFirstNode);
            nodeBlockDict.Add(node.id, nodeBlock); 
        }

        // 2. 给每个块绑定分支选项（跳转到对应ID的块）
        foreach (DialogueNode node in config.dialogue)
        {
            BindBranchToBlock(nodeBlockDict[node.id], node.branches, nodeBlockDict);
            Debug.Log($"绑定分支");    
        }
    }

    // 创建单个对话节点块（包含 说话 + 等待）
    private static Block CreateNodeBlock(Flowchart flowchart, DialogueNode node, string npcName, Vector2 blockPosition, bool isFirstNode = false)
    {
        Block block = flowchart.CreateBlock(blockPosition);
        // 第一个节点使用 Node_{config.character} 格式，其他节点使用 Node_{id} 格式
        block.BlockName = isFirstNode ? $"Node_{npcName}" : $"Node_{node.id}";
        
        // 按换行符分割文本为多个句子
        string[] sentences = node.text.Split('\n');
        
        // 为每个句子创建一个 Say 命令
        foreach (string sentence in sentences)
        {
            // 跳过空句子
            if (!string.IsNullOrWhiteSpace(sentence))
            {
                Say say = AddCommandToBlock<Say>(block);
                say.SetStandardText(sentence.Trim());
            }
        }

        return block;
    }

    // 绑定选项分支：点击选项 → 跳转到目标对话ID
    private static void BindBranchToBlock(Block block, DialogueBranch[] branches, Dictionary<string, Block> dict)
    {
        if (branches == null || branches.Length == 0) {
            Debug.LogWarning($"⚠️ 节点 {block.BlockName} 没有分支选项");
            return;
        } else {
            Debug.Log($"绑定分支：{block.BlockName} -> {branches.Length} 个选项");
        }

        // 为每个分支创建一个Menu命令
        foreach (DialogueBranch branch in branches)
        {
            // 根据nextDialogue找到目标块
            if (dict.TryGetValue(branch.nextDialogue, out Block targetBlock))
            {
                Fungus.Menu menu = AddCommandToBlock<Fungus.Menu>(block);
                menu.SetStandardText(branch.optionText);
                // 设置目标块
                var targetBlockField = typeof(Fungus.Menu).GetField("targetBlock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (targetBlockField != null)
                {
                    targetBlockField.SetValue(menu, targetBlock);
                }
            }
        }

        // 【关键：添加自定义输入选项】玩家自由打字
        Fungus.Menu customMenu = AddCommandToBlock<Fungus.Menu>(block);
        customMenu.SetStandardText("more...");
        Block customInputBlock = CreateCustomInputBlock(block.GetFlowchart());
        var customTargetField = typeof(Fungus.Menu).GetField("targetBlock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (customTargetField != null)
        {
            customTargetField.SetValue(customMenu, customInputBlock);
        }
    }

    // 创建【自定义输入】专用块
    private static Block CreateCustomInputBlock(Flowchart flowchart)
    {
        Debug.Log($"创建自定义输入节点：CustomInputNode");
        Block block = flowchart.CreateBlock(Vector2.zero);
        block.BlockName = "CustomInputNode";
        
        // 触发玩家输入面板
        // 使用CallMethod命令调用PlayerInputManager
        CallMethod callMethod = AddCommandToBlock<CallMethod>(block);
        
        // 设置目标对象和方法
        var targetObjectField = typeof(CallMethod).GetField("targetObject", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var methodNameField = typeof(CallMethod).GetField("methodName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // 设置为调用 PlayerInputManager.instance.ShowInput()
        if (targetObjectField != null && methodNameField != null)
        {
            // 获取 PlayerInputManager 单例
            GameObject playerInputManagerObj = GameObject.FindObjectOfType<PlayerInputManager>()?.gameObject;
            if (playerInputManagerObj != null)
            {
                targetObjectField.SetValue(callMethod, playerInputManagerObj);
                methodNameField.SetValue(callMethod, "ShowInput");
                Debug.Log("✅ 配置 CustomInputNode 调用 PlayerInputManager.ShowInput()");
            }
            else
            {
                Debug.LogWarning("⚠️ 未找到 PlayerInputManager 实例");
            }
        } else {
            Debug.LogError("❌ 未找到 CallMethod 命令");
            return null;
        }
        
        return block;
    }

    // 辅助方法：添加命令到块
    private static T AddCommandToBlock<T>(Block block) where T : Command
    {
        var flowchart = block.GetFlowchart() as Flowchart;
        var newCommand = Undo.AddComponent(block.gameObject, typeof(T)) as T;
        newCommand.ParentBlock = block;
        newCommand.ItemId = flowchart.NextItemId();
        block.CommandList.Add(newCommand);
        return newCommand;
    }
}
