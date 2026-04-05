using System;
using UnityEngine;

namespace RPG.AI.Utility
{
    // 根节点：AI返回的完整NPC对话配置
    [Serializable]
    public class AIDialogueConfig
    {
        public string character;          // NPC名称：武器店老板
        public DialogueNode[] dialogue;   // 所有对话节点数组

        public static AIDialogueConfig FromJSON(string json)
        {
            return JsonUtility.FromJson<AIDialogueConfig>(json);
        }
    }

    // 单个对话节点（对应JSON里的每一段对话）
    [Serializable]
    public class DialogueNode
    {
        public string id;                 // 对话ID：shop_greet / buy_sword 等
        public string text;               // 对话文本
        public string audio;              // 音频路径
        public DialogueBranch[] branches; // 选项分支
    }

    // 对话分支（玩家选项+跳转目标ID）
    [Serializable]
    public class DialogueBranch
    {
        public string optionText;    // 选项显示文字
        public string nextDialogue; // 跳转的对话ID
    }
}