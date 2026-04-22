using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
using ComfyUI;
using System.Threading.Tasks;


public class ComfyUIClient : Singleton<ComfyUIClient>
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
#endregion

#region 生命周期
    protected override void Awake()
    {
        base.Awake();

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
    }
#endregion

    // public static void SetupWorkflow(
    //     Dictionary<string, Node> workflow,
    //     string positivePrompt,   // 正向提示词
    //     string negativePrompt    // 反向提示词
    // )
    // {
    //     // ========== 1. 设置随机种子（通常在 KSampler 节点，ID=3） ==========
    //     long randomSeed = RandomUtil.GetRandomSeed();

    //     JsonReader.ModifyNodeInput(workflow, "3", "seed", randomSeed);

    //     // ========== 2. 设置正向提示词（通常 ID=6） ==========
    //     JsonReader.ModifyNodeInput(workflow, "6", "text", positivePrompt);

    //     // ========== 3. 设置反向提示词（通常 ID=7） ==========
    //     JsonReader.ModifyNodeInput(workflow, "7", "text", negativePrompt);
    // }

    #region 外部接口
    public async Task<string> ExecuteComfyUIWorkflowAsync(Dictionary<string, Node> workflow)
    {
        return await ExecuteWorkflowAsync(workflow);
    }
    #endregion


    #region 全流程协程（提交→轮询→下载）

    private async Task<string> ExecuteWorkflowAsync(Dictionary<string, Node> workflow) 
    {
# region 提交工作流
        ComfyUIPromptResponse submitResponse = await SubmitWorkflowAsync(workflow);
        if (submitResponse == null || string.IsNullOrEmpty(submitResponse.PromptId))
        {
            Debug.LogError("提交工作流失败，未获取到PromptId");
            return null;
        }
        Debug.Log($"工作流提交成功，PromptId: {submitResponse.PromptId}");
# endregion


#region 轮询工作流状态
        ComfyUIWorkflowResult workflowResult = await PollWorkflowStatusAsync(submitResponse.PromptId);
        if (workflowResult == null || !workflowResult.Status.Completed || workflowResult.Status.StatusStr != "success")
        {
            Debug.LogError("工作流执行超时或执行失败");
            return null;
        }
        Debug.Log("工作流执行成功，开始下载图片");
# endregion


#region 下载图片
        string savedImagePath = await DownloadImageFromComfyAsync(workflowResult);
        if (string.IsNullOrEmpty(savedImagePath))
        {
            Debug.LogError("未下载到图片");
            return null;
        }
        Debug.Log($"成功保存 {savedImagePath}");
# endregion

        return savedImagePath;
    }

    #endregion

    #region 步骤1：提交工作流到ComfyUI /prompt接口
    private async Task<ComfyUIPromptResponse> SubmitWorkflowAsync(Dictionary<string, Node> workflow)
    {
        if (workflow == null) {
            Debug.LogError("工作流为空");
            return null;
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
            await webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success) 
            {
                Debug.LogError($"请求失败: {webRequest.error}");
                return null;
            }

            try 
            {
                string responseText = webRequest.downloadHandler.text;
                ComfyUIPromptResponse response = JsonConvert.DeserializeObject<ComfyUIPromptResponse>(responseText);
                return response;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"解析响应失败: {e.Message}");
                return null;
            }
        }
    }
    
    #endregion

    #region 步骤2：轮询 /history/{PromptId} 接口获取执行状态
    private async Task<ComfyUIWorkflowResult> PollWorkflowStatusAsync(string promptId)
    {
        int pollingCount = 0;
        string requestUrl = $"{ComfyUIBaseUrl.TrimEnd('/')}/history/{promptId}";

        while (pollingCount < MaxPollingTimes) {
            pollingCount ++;

            using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl))
            {
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                await webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"轮询状态失败: {webRequest.error}");
                    continue;
                }

                string responseText = webRequest.downloadHandler.text;
                if (string.IsNullOrWhiteSpace(responseText) || responseText == "{}")
                {
                    if (pollingCount % 5 == 0) {
                        Debug.Log($"轮询中...({pollingCount}/{MaxPollingTimes})");
                    }
                    
                    await Task.Delay(PollingIntervalMs);
                    continue;
                }

                Debug.Log($"轮询响应: {responseText}");
                // 反序列化并返回执行结果
                ComfyUIHistoryRoot historyRoot = JsonConvert.DeserializeObject<ComfyUIHistoryRoot>(responseText);
                if (historyRoot.WorkflowResults.ContainsKey(promptId))
                {
                    // 把 JToken 反序列化为我们的实体类
                    var result = historyRoot.WorkflowResults[promptId].ToObject<ComfyUIWorkflowResult>();
                    return result;
                }
            }
        }
        Debug.LogError($"轮询超时，已尝试{MaxPollingTimes}次");
        return null;
    }
    #endregion

    #region 步骤3：下载图片 /view 接口，并保存到本地
    private async Task<string> DownloadImageFromComfyAsync(ComfyUIWorkflowResult workflowResult) 
    {
        if (workflowResult == null || workflowResult.Outputs == null || workflowResult.Outputs.Count == 0)
        {
            Debug.LogError("工作流结果为空或没有输出节点");
            return null;
        }
        foreach (var node in workflowResult.Outputs)
        {
            ComfyUINodeOutput nodeOutput = node.Value;
            if (nodeOutput.Images == null || nodeOutput.Images.Count == 0)
            {
                continue;
            }

            foreach (var imageInfo in nodeOutput.Images)
            {
                if (imageInfo.Type != "output")
                {
                    continue;
                }

                return await DownloadSingleImageAsync(imageInfo);
            }
        }
        return null;
    }

    private async Task<string> DownloadSingleImageAsync(ComfyUIImageInfo imageInfo)
    {
        string queryParams = $"filename={UnityWebRequest.EscapeURL(imageInfo.FileName)}" +
                            $"&subfolder={UnityWebRequest.EscapeURL(imageInfo.Subfolder)}" +
                            $"&type={UnityWebRequest.EscapeURL(imageInfo.Type)}";
        string downloadUrl = $"{ComfyUIBaseUrl.TrimEnd('/')}/view?{queryParams}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(downloadUrl))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            await webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"下载失败 {imageInfo.FileName}: {webRequest.error}");
                return null;
            }

            try
            {
                byte[] imageBytes = webRequest.downloadHandler.data;
                string finalSaveDir = Path.Combine(ImageSaveRoot, imageInfo.Subfolder);
                Directory.CreateDirectory(finalSaveDir);
                string savePath = Path.Combine(finalSaveDir, imageInfo.FileName);
                File.WriteAllBytes(savePath, imageBytes);
                return savePath;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"保存图片异常: {e.Message}");
                return null;
            }
        }
    }
    #endregion
}