using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using ProjectResources.Utils.Files;
using System.Threading.Tasks;
using LLMAPI;

namespace RPG.AI.Utility
{
    public class DialogJSONAPI : Singleton<DialogJSONAPI>
    {
        [SerializeField] private LLMConfigData config;

        public async Task<string> SendChatRequestAsync(string setting)
        {
            return await RequestAsync(setting);
        }

        private async Task<string> RequestAsync(string setting)
        {
            // ==================== 请求组装逻辑（完全不变） ====================
            LLMRequest request = new LLMRequest();
            request.Model = config.Model;
            request.Stream = false;

            // 系统消息
            request.Messages.Add(new LLMMessage()
            {
                Role = "system",
                Content = FileUtil.ReadFileAsString("Texts/APISystemSetting.md")
            });

            // 用户消息
            request.Messages.Add(new LLMMessage()
            {
                Role = "user",
                Content = setting
            });

            return await DoubaoApiManager.Instance.RequestChatAsync(request);
        }

        // private IEnumerator RequestCoroutine(string setting, System.Action<string> onSuccess, System.Action<string> onFail)
        // {
        //     // -------------------------- 仅修改这里的请求组装逻辑 --------------------------
        //     // 构建新的请求结构（复用ApiCore.cs中的ChatCompletionRequest）
        //     ChatCompletionRequest request = new ChatCompletionRequest();
        //     request.Model = "doubao-seed-2-0-pro-260215";
        //     request.Stream = false;
            
        //     // 添加系统消息（复用ApiCore.cs中的ChatMessage）
        //     request.Messages.Add(new ChatMessage()
        //     {
        //         Role = "system",
        //         Content = FileUtil.ReadFileAsString("Texts/APISystemSetting.md")
        //     });
            
        //     // 添加用户消息
        //     request.Messages.Add(new ChatMessage()
        //     {
        //         Role = "user",
        //         Content = setting
        //     });
            
        //     Debug.Log($"Request: {request}");
            
        //     string jsonData = JsonConvert.SerializeObject(request);
        //     Debug.Log($"Request JSON: {jsonData}");
        //     // -----------------------------------------------------------------------------

        //     using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(ApiUrl, jsonData))
        //     {
        //         webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        //         webRequest.downloadHandler = new DownloadHandlerBuffer();
        //         webRequest.SetRequestHeader("Content-Type", "application/json");
        //         webRequest.SetRequestHeader("Authorization", $"Bearer {ApiKey}");

        //         yield return webRequest.SendWebRequest();
        //         Debug.Log($"Response: {webRequest.result}");
        //         if (webRequest.result == UnityWebRequest.Result.Success)
        //         {
        //             string responseJson = webRequest.downloadHandler.text;
                    
        //             // Debug.Log($"Response JSON: {responseJson}");
        //             // 解析响应（复用ApiCore.cs中的ChatCompletionResponse）
        //             ChatCompletionResponse response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
        //             Debug.Log($"Response: {response}");
        //             onSuccess?.Invoke(response);
        //         }
        //         else
        //         {
        //             Debug.LogError($"API请求失败：{webRequest.error}，返回内容：{webRequest.downloadHandler.text}");
        //             onFail?.Invoke(webRequest.error ?? "请求失败");
        //         }
        //     }
        // }

       
            // try
            // {
            //     string jsonData = JsonConvert.SerializeObject(request);
            //     Debug.Log($"Request JSON: {jsonData}");
            //     // =================================================================

            //     // 创建 WebRequest
            //     using UnityWebRequest webRequest = new UnityWebRequest(ApiUrl, "POST");
            //     byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

            //     webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            //     webRequest.downloadHandler = new DownloadHandlerBuffer();
            //     webRequest.SetRequestHeader("Content-Type", "application/json");
            //     webRequest.SetRequestHeader("Authorization", $"Bearer {ApiKey}");

            //     // 等待请求完成
            //     await webRequest.SendWebRequest();

            //     // 处理成功
            //     if (webRequest.result == UnityWebRequest.Result.Success)
            //     {
            //         string responseJson = webRequest.downloadHandler.text;
            //         ChatCompletionResponse response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
            //         return response;
            //     }
            //     else
            //     {
            //         // 失败返回 null，可自行判断
            //         Debug.LogError($"API请求失败：{webRequest.error}，返回内容：{webRequest.downloadHandler.text}");
            //         return null;
            //     }
            // }
            // catch (System.Exception e)
            // {
            //     Debug.LogError($"请求异常：{e.Message}");
            //     return null;
            // }
        // }
    }
}
