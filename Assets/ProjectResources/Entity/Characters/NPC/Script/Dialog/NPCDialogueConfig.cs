using UnityEngine;

public class NPCDialogueConfig : MonoBehaviour
{
    [Header("基础信息")]
    public string npcName; // NPC名字
    public string npcId; // 唯一ID，用于存储记忆
    [Header("性格设定")]
    [TextArea(5, 10)] public string systemPrompt; // 系统提示词，核心设定
    [Header("对话参数")]
    public float temperature = 0.7f; // 不同性格可以配不同的随机性：活泼的NPC设高，严肃的设低
    public int maxHistoryRound = 10; // 最多保留多少轮对话记忆
    public string defaultReply = "我现在有点忙，等会儿再说吧。"; // 请求失败时的默认回复
}