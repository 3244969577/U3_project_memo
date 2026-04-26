using UnityEngine;
using Newtonsoft.Json;

namespace NPCDialogAPI 
{
    public class NPCLLMRsp
    {
        [JsonProperty("action")] public string action;   // 按照LLMRspRule.md中的action
        [JsonProperty("dialogue")] public string dialogue; // 原始的对话内容
        [JsonProperty("emotion")] public string emotion;  // 按照LLMRspRule.md中的emotion
        [JsonProperty("social_alter")] public SocialAlter social_alter;
    }
    public class SocialAlter
    {
        [JsonProperty("name")] public string name;
        [JsonProperty("relation")] public int relation;
    }

}