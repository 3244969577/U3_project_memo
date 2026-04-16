// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Networking;
// using Newtonsoft.Json;
// using ProjectResources.Utils.Files;
// using System.Threading.Tasks;

// namespace RPG.AI.Utility
// {
//     public class AsyncRequester : MonoBehaviour
//     {
//         public static AsyncRequester Instance;
//         // 替换成你给的方舟接口地址
//         private const string ApiUrl = "https://ark.cn-beijing.volces.com/api/v3/chat/completions";
//         // 换成你自己的方舟API Key（还是推荐用云函数代理，不要硬编码在客户端）  
//         private const string ApiKey = "a6aaec1c-a583-4361-8352-f75bf5ce2527"; 

//         private void Awake()
//         {
//             if (Instance == null) Instance = this;
//             else Destroy(gameObject);
//         }

//         public async Task<ChatCompletionResponse> SendChatRequest(string setting)
//         {
//             ChatCompletionRequest request = new ChatCompletionRequest();
//             request.Model = "doubao-seed-2-0-pro-260215";
//             request.Stream = false;

//             request.Messages.Add(new ChatMessage()
//             {
//                 Role = "system",
//                 Content = FileUtil.ReadFileAsString("Texts/APISystemSetting.md")
//             });
            
//             request.Messages.Add(new ChatMessage()
//             {
//                 Role = "user",
//                 Content = setting
//             });

//             Debug.Log($"Request: {request}");
//             string jsonData = JsonConvert.SerializeObject(request);
//             Debug.Log($"Request JSON: {jsonData}");

//             using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(ApiUrl, jsonData))
//             {
//                 webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
//                 webRequest.downloadHandler = new DownloadHandlerBuffer();
//                 webRequest.SetRequestHeader("Content-Type", "application/json");
//                 webRequest.SetRequestHeader("Authorization", $"Bearer {ApiKey}");

//                 var operation = webRequest.SendWebRequest();
//                 while (!operation.isDone)
//                 {
//                     await Task.Yield();
//                 }

//                 if (request.result != UnityWebRequest.Result.Success)
//                 {
//                     throw new Exception($"API请求失败: {request.error}");
//                 }

//                 string responseJson = webRequest.downloadHandler.text;
//                 Debug.Log($"Response JSON: {responseJson}");
//                 // 解析响应（复用ApiCore.cs中的ChatCompletionResponse）
//                 ChatCompletionResponse response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
//                 Debug.Log($"Response: {response}");
//                 return response;

//                 // yield return webRequest.SendWebRequest();
//                 // Debug.Log($"Response: {webRequest.result}");
//                 // if (webRequest.result == UnityWebRequest.Result.Success)
//                 // {
//                 //     string responseJson = webRequest.downloadHandler.text;
                    
//                 //     // Debug.Log($"Response JSON: {responseJson}");
//                 //     // 解析响应（复用ApiCore.cs中的ChatCompletionResponse）
//                 //     ChatCompletionResponse response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
//                 //     Debug.Log($"Response: {response}");
//                 //     onSuccess?.Invoke(response);
//                 // }
//                 // else
//                 // {
//                 //     Debug.LogError($"API请求失败：{webRequest.error}，返回内容：{webRequest.downloadHandler.text}");
//                 //     onFail?.Invoke(webRequest.error ?? "请求失败");
//                 // }
//             }
//             // StartCoroutine(RequestCoroutine(setting, onSuccess, onFail));
//         }

//         // private IEnumerator RequestCoroutine(string setting, System.Action<ChatCompletionResponse> onSuccess, System.Action<string> onFail)
//         // {
//         //     // -------------------------- 仅修改这里的请求组装逻辑 --------------------------
//         //     // 构建新的请求结构（复用ApiCore.cs中的ChatCompletionRequest）
//         //     ChatCompletionRequest request = new ChatCompletionRequest();
//         //     request.Model = "doubao-seed-2-0-pro-260215";
//         //     request.Stream = false;
            
//         //     // 添加系统消息（复用ApiCore.cs中的ChatMessage）
//         //     request.Messages.Add(new ChatMessage()
//         //     {
//         //         Role = "system",
//         //         Content = FileUtil.ReadFileAsString("Texts/APISystemSetting.md")
//         //     });
            
//         //     // 添加用户消息
//         //     request.Messages.Add(new ChatMessage()
//         //     {
//         //         Role = "user",
//         //         Content = setting
//         //     });
            
//         //     Debug.Log($"Request: {request}");
            
//         //     string jsonData = JsonConvert.SerializeObject(request);
//         //     Debug.Log($"Request JSON: {jsonData}");
//         //     // -----------------------------------------------------------------------------

//         //     using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(ApiUrl, jsonData))
//         //     {
//         //         webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
//         //         webRequest.downloadHandler = new DownloadHandlerBuffer();
//         //         webRequest.SetRequestHeader("Content-Type", "application/json");
//         //         webRequest.SetRequestHeader("Authorization", $"Bearer {ApiKey}");

//         //         yield return webRequest.SendWebRequest();
//         //         Debug.Log($"Response: {webRequest.result}");
//         //         if (webRequest.result == UnityWebRequest.Result.Success)
//         //         {
//         //             string responseJson = webRequest.downloadHandler.text;
                    
//         //             // Debug.Log($"Response JSON: {responseJson}");
//         //             // 解析响应（复用ApiCore.cs中的ChatCompletionResponse）
//         //             ChatCompletionResponse response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
//         //             Debug.Log($"Response: {response}");
//         //             onSuccess?.Invoke(response);
//         //         }
//         //         else
//         //         {
//         //             Debug.LogError($"API请求失败：{webRequest.error}，返回内容：{webRequest.downloadHandler.text}");
//         //             onFail?.Invoke(webRequest.error ?? "请求失败");
//         //         }
//         //     }
//         // }
//     }
    
//     // [System.Serializable]
//     // public class StreamOptions
//     // {
//     //     [JsonProperty("include_usage")]
//     //     public bool IncludeUsage { get; set; }
//     // }
// }
