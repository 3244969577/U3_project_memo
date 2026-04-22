using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



namespace ComfyUI 
{
    
#region ComfyUI API 数据模型
    /// <summary>
    /// 提交工作流的请求体
    /// </summary>
    [Serializable]
    public class ComfyUIPromptRequest
    {
        [JsonProperty("prompt")]
        public Dictionary<string, Node> Prompt; // 工作流节点字典

        [JsonProperty("client_id")]
        public string ClientId = Guid.NewGuid().ToString(); // 唯一标识，自动生成

        [JsonProperty("extra_data")]
        public Dictionary<string, object> ExtraData = new Dictionary<string, object>();

        // [JsonProperty("number")]
        // public int Number = 0; // 队列优先级，0最高
    }

    /// <summary>
    /// 提交工作流的响应结果
    /// </summary>
    [Serializable]
    public class ComfyUIPromptResponse
    {
        [JsonProperty("prompt_id")]
        public string PromptId;

        [JsonProperty("number")]
        public int Number;

        [JsonProperty("node_errors")]
        public Dictionary<string, object> NodeErrors;
    }

    /// <summary>
    /// 轮询历史的根对象
    /// </summary>
    [Serializable]
    public class ComfyUIHistoryRoot
    {
        [JsonExtensionData]
        // public Dictionary<string, ComfyUIWorkflowResult> WorkflowResults = new Dictionary<string, ComfyUIWorkflowResult>();
        public Dictionary<string, JToken> WorkflowResults { get; set; } = new Dictionary<string, JToken>();
    }

    /// <summary>
    /// 工作流执行结果
    /// </summary>
    [Serializable]
    public class ComfyUIWorkflowResult
    {
        [JsonProperty("outputs")]
        public Dictionary<string, ComfyUINodeOutput> Outputs;

        [JsonProperty("status")]
        public ComfyUIWorkflowStatus Status;
    }

    /// <summary>
    /// 工作流执行状态
    /// </summary>
    [Serializable]
    public class ComfyUIWorkflowStatus
    {
        [JsonProperty("status_str")]
        public string StatusStr;

        [JsonProperty("completed")]
        public bool Completed;
    }

    /// <summary>
    /// 节点输出结果
    /// </summary>
    [Serializable]
    public class ComfyUINodeOutput
    {
        [JsonProperty("images")]
        public List<ComfyUIImageInfo> Images;
    }

    /// <summary>
    /// 图片信息（下载所需核心参数）
    /// </summary>
    [Serializable]
    public class ComfyUIImageInfo
    {
        [JsonProperty("filename")]
        public string FileName;

        [JsonProperty("subfolder")]
        public string Subfolder;

        [JsonProperty("type")]
        public string Type; // output/temp/input
    }
#endregion

#region 工作流JSON模型（复用并兼容原Node类）
    [Serializable]
    public class Node
    {
        [JsonProperty("inputs")]
        public Dictionary<string, object> inputs;

        [JsonProperty("class_type")]
        public string class_type;

        [JsonProperty("_meta")]
        public Meta _meta;
    }

    [Serializable]
    public class Meta
    {
        [JsonProperty("title")]
        public string title;
    }
#endregion

}