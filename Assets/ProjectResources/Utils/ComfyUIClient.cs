using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;


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


public class ComfyUIClient : MonoBehaviour
{
    #region 配置参数
    [Header("ComfyUI服务配置")]
    [Tooltip("ComfyUI服务地址，默认本地")]
    public string ComfyUIBaseUrl = "http://127.0.0.1:6006";
    [Tooltip("轮询间隔（毫秒）")]
    public int PollingIntervalMs = 1000;
    [Tooltip("最大轮询次数（防止无限等待）")]
    public int MaxPollingTimes = 120;
    [Tooltip("图片保存根目录（默认PersistentDataPath）")]
    public string ImageSaveRoot = "";

    [Header("图片保存到项目文件夹")]
    public string ImageSaveFolder = "GeneratedImages";
    // public string ImageSaveFolder = "ProjectResources/Entity/Generator/Generated";

    // 单例（方便全局调用，可选）
    public static ComfyUIClient Instance;
    #endregion

    #region 生命周期
    private void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        #if UNITY_EDITOR
        ImageSaveRoot = Path.Combine(Application.dataPath, ImageSaveFolder);
        #else
        // 打包后不能写进Assets，自动切换到持久化路径
        ImageSaveRoot = Path.Combine(Application.persistentDataPath, ImageSaveFolder);
        #endif

        if (!Directory.Exists(ImageSaveRoot))
        {
            Directory.CreateDirectory(ImageSaveRoot);
            Debug.Log($"图片将保存到项目文件夹: " + ImageSaveRoot);
        }

        // // 初始化图片保存路径
        // if (string.IsNullOrEmpty(ImageSaveRoot))
        // {
        //     ImageSaveRoot = Path.Combine(Application.persistentDataPath, "ComfyUI_Images");
        // }
        // if (!Directory.Exists(ImageSaveRoot))
        // {
        //     Directory.CreateDirectory(ImageSaveRoot);
        //     Debug.Log($"初始化图片保存目录: {ImageSaveRoot}");
        // }
    }
    #endregion


    public static void SetupWorkflow(
        Dictionary<string, Node> workflow,
        string positivePrompt,   // 正向提示词
        string negativePrompt    // 反向提示词
    )
    {
        // ========== 1. 设置随机种子（通常在 KSampler 节点，ID=3） ==========
        long randomSeed = RandomUtil.GetRandomSeed();

        JsonReader.ModifyNodeInput(workflow, "3", "seed", randomSeed);

        // ========== 2. 设置正向提示词（通常 ID=6） ==========
        JsonReader.ModifyNodeInput(workflow, "6", "text", positivePrompt);

        // ========== 3. 设置反向提示词（通常 ID=7） ==========
        JsonReader.ModifyNodeInput(workflow, "7", "text", negativePrompt);

        // ========== 你还可以加更多参数 ==========
        // JsonReader.ModifyNodeInput(workflow, "3", "steps", 25);
        // JsonReader.ModifyNodeInput(workflow, "3", "cfg", 8.0f);  
        // JsonReader.ModifyNodeInput(workflow, "5", "width", 512);
        // JsonReader.ModifyNodeInput(workflow, "5", "height", 768);
    }


    public void TestComfyUI()
    {
        Dictionary<string, Node> workflow = JsonReader.ReadWorkflowJson();
        SetupWorkflow(workflow, 
            @"(rpgchara), 1girl, white and pink hair, green eyes, black office clothes, cleavage, (white background:1.2), simple background, multiple views, chibi, multiple views, reference sheet, small sprite, rpgmaker sprite,sharp pixels, sharp edges, masterpiece, best quality,row_1: front, (row_2: left:1.1), row_3: right, row_4: back ",
            "blurry, low quality, ugly, deformed, text, watermark, signature, extra limbs"
        );

        ExecuteComfyUIWorkflow(
            workflow, 
            (images) => {
                // Debug.Log($"成功保存 {images.Count} 张图片");
                foreach (string img in images)
                {
                    Debug.Log(img);
                }
            }, 
            (err) => Debug.LogError($"失败: {err}")
        );
    }

    #region 核心入口：执行ComfyUI全流程（提交+轮询+下载）
    /// <summary>
    /// 执行ComfyUI全流程
    /// </summary>
    /// <param name="workflow">工作流节点字典（从JsonReader读取）</param>
    /// <param name="onSuccess">成功回调（返回保存的图片路径列表）</param>
    /// <param name="onFail">失败回调（返回错误信息）</param>
    public void ExecuteComfyUIWorkflow(Dictionary<string, Node> workflow, Action<List<string>> onSuccess, Action<string> onFail)
    {
        if (workflow == null || workflow.Count == 0)
        {
            onFail?.Invoke("工作流为空，请先读取有效的JSON文件");
            return;
        }
        // 启动协程执行全流程
        StartCoroutine(ExecuteWorkflowCoroutine(workflow, onSuccess, onFail));
        Debug.Log("开始执行ComfyUI全流程");
    }

    /// <summary>
    /// 全流程协程（提交→轮询→下载）
    /// </summary>
    private IEnumerator ExecuteWorkflowCoroutine(Dictionary<string, Node> workflow, Action<List<string>> onSuccess, Action<string> onFail)
    {
        // 步骤1：提交工作流
        ComfyUIPromptResponse submitResponse = null;
        
        yield return StartCoroutine(SubmitWorkflowCoroutine(workflow, (res) => submitResponse = res, onFail));
        if (submitResponse == null || string.IsNullOrEmpty(submitResponse.PromptId))
        {
            onFail?.Invoke("提交工作流失败，未获取到PromptId");
            yield break;
        }
        Debug.Log($"工作流提交成功，PromptId: {submitResponse.PromptId}");

        // 步骤2：轮询工作流状态
        ComfyUIWorkflowResult workflowResult = null;
        yield return StartCoroutine(PollWorkflowStatusCoroutine(submitResponse.PromptId, (res) => workflowResult = res, onFail));
        if (workflowResult == null || !workflowResult.Status.Completed || workflowResult.Status.StatusStr != "success")
        {
            onFail?.Invoke("工作流执行超时或执行失败");
            yield break;
        }
        Debug.Log("工作流执行成功，开始下载图片");

        // 步骤3：下载所有图片
        List<string> savedImagePaths = new List<string>();
        yield return StartCoroutine(DownloadAllImagesCoroutine(workflowResult, savedImagePaths));
        if (savedImagePaths.Count == 0)
        {
            onFail?.Invoke("未下载到任何图片");
            yield break;
        }

        // 全流程成功
        onSuccess?.Invoke(savedImagePaths);
    }
    #endregion

    #region 步骤1：提交工作流到ComfyUI /prompt接口
    private IEnumerator SubmitWorkflowCoroutine(Dictionary<string, Node> workflow, Action<ComfyUIPromptResponse> onSuccess, Action<string> onFail)
    {
        if (workflow == null)
        {
            onFail?.Invoke("工作流为空");
            yield break;
        }

        ComfyUIPromptRequest requestData = new ComfyUIPromptRequest { Prompt = workflow };
        string jsonBody = JsonConvert.SerializeObject(requestData);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        string requestUrl = $"{ComfyUIBaseUrl.TrimEnd('/')}/prompt";

        using (UnityWebRequest webRequest = new UnityWebRequest(requestUrl, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(jsonBytes);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                onFail?.Invoke($"提交失败: {webRequest.error}");
                yield break;
            }

            try
            {
                string responseText = webRequest.downloadHandler.text;
                Debug.Log($"提交响应: {responseText}");
                ComfyUIPromptResponse response = JsonConvert.DeserializeObject<ComfyUIPromptResponse>(responseText);
                onSuccess?.Invoke(response);
            }
            catch (System.Exception e)
            {
                onFail?.Invoke($"解析响应失败: {e.Message}");
            }
        }
    }
    #endregion

    #region 步骤2：轮询 /history/{PromptId} 接口获取执行状态
    private IEnumerator PollWorkflowStatusCoroutine(string promptId, Action<ComfyUIWorkflowResult> onSuccess, Action<string> onFail)
    {
        int pollingCount = 0;
        string requestUrl = $"{ComfyUIBaseUrl.TrimEnd('/')}/history/{promptId}";

        while (pollingCount < MaxPollingTimes)
        {
            pollingCount++;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl))
            {
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    onFail?.Invoke($"轮询状态失败: {webRequest.error}");
                    yield break;
                }

                string responseText = webRequest.downloadHandler.text;
                // 未完成则返回空JSON，继续轮询
                if (string.IsNullOrWhiteSpace(responseText) || responseText == "{}")
                {
                    Debug.Log($"轮询中...({pollingCount}/{MaxPollingTimes})");
                    yield return new WaitForSeconds(PollingIntervalMs / 1000f);
                    continue;
                }

                Debug.Log($"轮询响应: {responseText}");
                // 反序列化并返回执行结果
                ComfyUIHistoryRoot historyRoot = JsonConvert.DeserializeObject<ComfyUIHistoryRoot>(responseText);
                if (historyRoot.WorkflowResults.ContainsKey(promptId))
                {
                    // 把 JToken 反序列化为我们的实体类
                    var result = historyRoot.WorkflowResults[promptId].ToObject<ComfyUIWorkflowResult>();
                    onSuccess?.Invoke(result);
                    yield break;
                }
                
            }

            yield return new WaitForSeconds(PollingIntervalMs / 1000f);
        }

        // 轮询超时
        onFail?.Invoke($"轮询超时，已尝试{MaxPollingTimes}次");
    }
    #endregion

    #region 步骤3：下载图片 /view 接口，并保存到本地
    private IEnumerator DownloadAllImagesCoroutine(ComfyUIWorkflowResult workflowResult, List<string> savedImagePaths)
    {
        if (workflowResult.Outputs == null || workflowResult.Outputs.Count == 0)
        {
            yield break;
        }

        // 遍历所有输出节点的图片
        foreach (var nodeOutputKvp in workflowResult.Outputs)
        {
            ComfyUINodeOutput nodeOutput = nodeOutputKvp.Value;
            if (nodeOutput.Images == null || nodeOutput.Images.Count == 0)
            {
                continue;
            }

            

            // 下载当前节点的所有图片
            foreach (var imageInfo in nodeOutput.Images)
            {
                if (imageInfo.Type != "output")
                {
                    Debug.Log($"跳过非output图片：{imageInfo.FileName} 类型：{imageInfo.Type}");
                    continue;
                }
                yield return StartCoroutine(DownloadSingleImageCoroutine(imageInfo, savedImagePaths));
            }
        }
    }

    /// <summary>
    /// 下载单张图片并保存到本地
    /// </summary>
    private IEnumerator DownloadSingleImageCoroutine(ComfyUIImageInfo imageInfo, List<string> savedImagePaths)
    {
        string queryParams = $"filename={UnityWebRequest.EscapeURL(imageInfo.FileName)}" +
                            $"&subfolder={UnityWebRequest.EscapeURL(imageInfo.Subfolder)}" +
                            $"&type={UnityWebRequest.EscapeURL(imageInfo.Type)}";
        string downloadUrl = $"{ComfyUIBaseUrl.TrimEnd('/')}/view?{queryParams}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(downloadUrl))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"下载失败 {imageInfo.FileName}: {webRequest.error}");
                yield break;
            }

            try
            {
                byte[] imageBytes = webRequest.downloadHandler.data;
                string finalSaveDir = Path.Combine(ImageSaveRoot, imageInfo.Subfolder);
                Directory.CreateDirectory(finalSaveDir);
                string savePath = Path.Combine(finalSaveDir, imageInfo.FileName);
                File.WriteAllBytes(savePath, imageBytes);
                savedImagePaths.Add(savePath);
                Debug.Log($"保存成功: {savePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"保存图片异常: {e.Message}");
            }
        }
    }
    #endregion

    #region 便捷方法：读取工作流并直接执行
    /// <summary>
    /// 【便捷方法】从默认路径读取工作流并执行全流程
    /// </summary>
    public void ReadAndExecuteWorkflow(Action<List<string>> onSuccess = null, Action<string> onFail = null)
    {
        Dictionary<string, Node> workflow = JsonReader.ReadWorkflowJson();
        ExecuteComfyUIWorkflow(workflow, onSuccess, onFail);
    }

    /// <summary>
    /// 【便捷方法】从指定相对路径读取工作流并执行全流程
    /// </summary>
    public void ReadAndExecuteWorkflow(string relativePath, Action<List<string>> onSuccess = null, Action<string> onFail = null)
    {
        Dictionary<string, Node> workflow = JsonReader.ReadJsonFromRelativePath<Dictionary<string, Node>>(relativePath);
        ExecuteComfyUIWorkflow(workflow, onSuccess, onFail);
    }
    #endregion
}















// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Text;
// using Newtonsoft.Json;
// using System;

// /// <summary>
// /// ComfyUI API请求工具类
// /// 每个实例代表一次完整的请求流程
// /// </summary>
// public class ComfyUIRequester : MonoBehaviour
// {
//     // 状态变量
//     public string baseUrl = "http://127.0.0.1:6006";
//     public string clientId { get; private set; }
//     public string promptId { get; private set; }
//     public bool IsCompleted { get; private set; }
//     public bool IsSuccess { get; private set; }
//     public Dictionary<string, object> Outputs { get; private set; }

//     /// <summary>
//     /// 构造函数
//     /// </summary>
//     public ComfyUIRequester()
//     {
//         clientId = Guid.NewGuid().ToString();
//         IsCompleted = false;
//         IsSuccess = false;
//         Outputs = new Dictionary<string, object>();
//     }

//     /// <summary>
//     /// 设置基础URL
//     /// </summary>
//     /// <param name="url">ComfyUI服务器URL</param>
//     public void SetBaseUrl(string url)
//     {
//         baseUrl = url;
//     }

//     /// <summary>
//     /// 发送prompt请求
//     /// </summary>
//     /// <param name="prompt">工作流定义</param>
//     /// <param name="callback">回调函数</param>
//     public IEnumerator PostPrompt(Dictionary<string, Node> prompt, System.Action<PromptResponse> callback)
//     {
//         string url = $"{baseUrl}/prompt";
        
//         // 构建请求数据
//         var requestData = new Dictionary<string, object>
//         {
//             { "prompt", prompt },
//             { "client_id", clientId }
//         };
        
//         string jsonData = JsonConvert.SerializeObject(requestData);
//         byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

//         using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Post(url, UnityEngine.Networking.UnityWebRequest.kHttpVerbPOST))
//         {
//             request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
//             request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
//             request.SetRequestHeader("Content-Type", "application/json");

//             yield return request.SendWebRequest();

//             if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
//             {
//                 try
//                 {
//                     var response = JsonConvert.DeserializeObject<PromptResponse>(request.downloadHandler.text);
//                     promptId = response.prompt_id;
//                     callback?.Invoke(response);
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError($"解析prompt响应失败: {e.Message}");
//                     callback?.Invoke(null);
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"POST prompt failed: {request.error}");
//                 callback?.Invoke(null);
//             }
//         }
//     }

//     /// <summary>
//     /// 发送prompt请求（通用版本）
//     /// </summary>
//     /// <param name="prompt">工作流定义</param>
//     /// <param name="callback">回调函数</param>
//     public IEnumerator PostPrompt(Dictionary<string, object> prompt, System.Action<PromptResponse> callback)
//     {
//         string url = $"{baseUrl}/prompt";
        
//         // 构建请求数据
//         var requestData = new Dictionary<string, object>
//         {
//             { "prompt", prompt },
//             { "client_id", clientId }
//         };
        
//         string jsonData = JsonConvert.SerializeObject(requestData);
//         byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

//         using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Post(url, UnityEngine.Networking.UnityWebRequest.kHttpVerbPOST))
//         {
//             request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(bodyRaw);
//             request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
//             request.SetRequestHeader("Content-Type", "application/json");

//             yield return request.SendWebRequest();

//             if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
//             {
//                 try
//                 {
//                     var response = JsonConvert.DeserializeObject<PromptResponse>(request.downloadHandler.text);
//                     promptId = response.prompt_id;
//                     callback?.Invoke(response);
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError($"解析prompt响应失败: {e.Message}");
//                     callback?.Invoke(null);
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"POST prompt failed: {request.error}");
//                 callback?.Invoke(null);
//             }
//         }
//     }

//     /// <summary>
//     /// 获取历史记录
//     /// </summary>
//     /// <param name="callback">回调函数</param>
//     public IEnumerator GetHistory(System.Action<HistoryResponse> callback)
//     {
//         if (string.IsNullOrEmpty(promptId))
//         {
//             Debug.LogError("promptId is not set. Call PostPrompt first.");
//             callback?.Invoke(null);
//             yield break;
//         }

//         string url = $"{baseUrl}/history/{promptId}";

//         using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url))
//         {
//             yield return request.SendWebRequest();

//             if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
//             {
//                 try
//                 {
//                     var response = JsonConvert.DeserializeObject<HistoryResponse>(request.downloadHandler.text);
                    
//                     // 检查是否有数据
//                     if (response != null && response.ContainsKey(promptId))
//                     {
//                         var historyItem = response[promptId];
//                         IsCompleted = true;
//                         IsSuccess = historyItem.status?.status_str == "success";
//                         Outputs = historyItem.outputs;
//                     }
                    
//                     callback?.Invoke(response);
//                 }
//                 catch (Exception e)
//                 {
//                     Debug.LogError($"解析history响应失败: {e.Message}");
//                     callback?.Invoke(null);
//                 }
//             }
//             else
//             {
//                 Debug.LogError($"GET history failed: {request.error}");
//                 callback?.Invoke(null);
//             }
//         }
//     }

//     /// <summary>
//     /// 查看文件
//     /// </summary>
//     /// <param name="filename">文件名</param>
//     /// <param name="subfolder">子文件夹</param>
//     /// <param name="type">文件类型</param>
//     /// <param name="callback">回调函数</param>
//     public IEnumerator GetView(string filename, string subfolder, string type, System.Action<Texture2D> callback)
//     {
//         string url = $"{baseUrl}/view?filename={UnityEngine.Networking.UnityWebRequest.EscapeURL(filename)}&subfolder={UnityEngine.Networking.UnityWebRequest.EscapeURL(subfolder)}&type={UnityEngine.Networking.UnityWebRequest.EscapeURL(type)}";

//         using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
//         {
//             yield return request.SendWebRequest();

//             if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
//             {
//                 Texture2D texture = ((UnityEngine.Networking.DownloadHandlerTexture)request.downloadHandler).texture;
//                 callback?.Invoke(texture);
//             }
//             else
//             {
//                 Debug.LogError($"GET view failed: {request.error}");
//                 callback?.Invoke(null);
//             }
//         }
//     }

//     /// <summary>
//     /// 轮询任务状态
//     /// </summary>
//     /// <param name="interval">轮询间隔（秒）</param>
//     /// <param name="timeout">超时时间（秒）</param>
//     /// <param name="callback">回调函数</param>
//     public IEnumerator PollStatus(float interval = 1f, float timeout = 60f, System.Action<HistoryResponse> callback = null)
//     {
//         float startTime = Time.time;
        
//         while (Time.time - startTime < timeout)
//         {
//             yield return new WaitForSeconds(interval);
            
//             yield return GetHistory((response) => {
//                 if (response != null && response.ContainsKey(promptId))
//                 {
//                     IsCompleted = true;
//                     var historyItem = response[promptId];
//                     IsSuccess = historyItem.status?.status_str == "success";
//                     Outputs = historyItem.outputs;
//                     callback?.Invoke(response);
//                 }
//             });
            
//             if (IsCompleted)
//             {
//                 break;
//             }
//         }
        
//         if (!IsCompleted)
//         {
//             Debug.LogError("Polling timeout");
//             callback?.Invoke(null);
//         }
//     }
// }

// /// <summary>
// /// Prompt响应数据类
// /// </summary>
// [System.Serializable]
// public class PromptResponse
// {
//     public string prompt_id;
//     public int number;
//     public Dictionary<string, object> node_errors;
// }

// /// <summary>
// /// 历史记录响应数据类
// /// </summary>
// [System.Serializable]
// public class HistoryResponse : Dictionary<string, HistoryItem>
// {}

// /// <summary>
// /// 历史记录项数据类
// /// </summary>
// [System.Serializable]
// public class HistoryItem
// {
//     public object prompt;
//     public Dictionary<string, object> outputs;
//     public Status status;
//     public Dictionary<string, object> meta;
// }

// /// <summary>
// /// 状态数据类
// /// </summary>
// [System.Serializable]
// public class Status
// {
//     public string status_str;
//     public bool completed;
//     public List<object> messages;
// }

// /// <summary>
// /// 节点数据类
// /// </summary>
// [System.Serializable]
// public class Node
// {
//     public Dictionary<string, object> inputs;
//     public string class_type;
//     public Meta _meta;
// }

// /// <summary>
// /// 元数据类
// /// </summary>
// [System.Serializable]
// public class Meta
// {
//     public string title;
// }

