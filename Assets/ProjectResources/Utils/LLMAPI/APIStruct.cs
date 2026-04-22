using System.Collections.Generic;
using Newtonsoft.Json;

namespace LLMAPI
{
    // 豆包API请求体结构
    public class LLMRequest
    {
        [JsonProperty("model")] public string Model = "";
        [JsonProperty("messages")] public List<LLMMessage> Messages = new List<LLMMessage>();
        [JsonProperty("stream")] public bool Stream = false; // 流式输出，后面优化会用到
        [JsonProperty("reasoning")] public LLMReasonConfig Reasoning = new(); // 是否思考，disable=不思考，enable=思考
    }

    public class LLMResponse
    {
        [JsonProperty("choices")] public List<LLMChoice> Choices;   
    }

    public class LLMChoice {
        [JsonProperty("message")] public LLMMessage Message;
    }

    public class LLMMessage {
        [JsonProperty("role")] public string Role; // 取值：system(系统设定)/user(玩家)/assistant(NPC)
        [JsonProperty("content")] public string Content;
    }

    public class LLMReasonConfig {
        [JsonProperty("effort")] public string Effort = "low";
    }

    public class StreamOptions
    {
        [JsonProperty("include_usage")]
        public bool IncludeUsage { get; set; }
    }

}
