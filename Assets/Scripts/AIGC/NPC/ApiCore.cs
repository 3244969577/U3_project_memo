using System.Collections.Generic;
using Newtonsoft.Json;
// 豆包API请求体结构
public class ChatCompletionRequest
{
    [JsonProperty("model")] public string Model = "doubao-lite-4k"; // 选模型，lite性价比最高
    [JsonProperty("messages")] public List<ChatMessage> Messages = new();
    [JsonProperty("temperature")] public float Temperature = 0.7f; // 回复随机性：0=最稳定，1=最随机
    [JsonProperty("stream")] public bool Stream = false; // 流式输出，后面优化会用到
}
// 单条对话消息结构
public class ChatMessage
{
    [JsonProperty("role")] public string Role; // 取值：system(系统设定)/user(玩家)/assistant(NPC)
    [JsonProperty("content")] public string Content;
}
// 豆包API返回结构
public class ChatCompletionResponse
{
    [JsonProperty("choices")] public List<ChatChoice> Choices;
    [JsonProperty("usage")] public Usage Usage; // 消耗的token数，用于统计成本
}
public class ChatChoice     
{
    [JsonProperty("message")] public ChatMessage Message;
}
public class Usage
{
    [JsonProperty("total_tokens")] public int TotalTokens;
}

// 方舟接口外层请求体
public class ArkChatCompletionRequest
{
    [JsonProperty("model")] public string Model = "doubao-seed-2-0-pro-260215"; // 你给的默认模型
    [JsonProperty("messages")] public List<ArkInputItem> Messages = new();
    // 如果需要温度、流式输出等参数，直接加在这里即可，比如：
    // [JsonProperty("temperature")] public float Temperature = 0.7f;
    // [JsonProperty("stream")] public bool Stream = false;
}

// input数组的单个元素（对应原来的单条ChatMessage）
public class ArkInputItem
{
    [JsonProperty("role")] public string Role;
    [JsonProperty("content")] public List<ArkContentItem> Content = new();
}

// content数组的单个元素（支持文本/图片）
public class ArkContentItem
{
    [JsonProperty("type")] public string Type;
    [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)] public string Text; // 文本类型用
    [JsonProperty("image_url", NullValueHandling = NullValueHandling.Ignore)] public string ImageUrl; // 图片类型用
}