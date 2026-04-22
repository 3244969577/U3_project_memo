using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using GlobalEvents;
using System.Threading.Tasks;
using LLMAPI;

public class DoubaoApiManager : Singleton<DoubaoApiManager>
{
    [SerializeField] private LLMConfigData config;

    public async Task<string> SendChatRequestAsync(List<LLMMessage> messages)
    {
        return await RequestChatAsync(messages);
    }

    public async Task<string> RequestChatAsync(List<LLMMessage> messages)
    {
        LLMRequest request = new LLMRequest();
        request.Messages = messages;
        request.Model = config.Model;

        return await RequestChatAsync(request);
    }

    /// <summary> 返回LLM的输出文本 </summary>
    public async Task<string> RequestChatAsync(LLMRequest request) 
    {
        try 
        {
            string jsonData = JsonConvert.SerializeObject(request);
            
#region 创建请求
            // 创建 UnityWebRequest
            using (UnityWebRequest webRequest = new UnityWebRequest(config.APIUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", $"Bearer {config.APIKey}");

                // 发送请求并等待
                await webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success) 
                {
                    Debug.LogError($"请求失败: {webRequest.error}");
                    return null;
                }

                #region 处理响应
                string responseJson = webRequest.downloadHandler.text;

                LLMResponse response = JsonConvert.DeserializeObject<LLMResponse>(responseJson);
                if (response != null && response.Choices != null && response.Choices.Count > 0)
                {
                    return response.Choices[0].Message.Content;
                }
                else
                {
                    Debug.LogError("Response format error: invalid choices");
                    return null;
                }
                #endregion
            }
#endregion

        }
        catch (System.Exception e)
        {
            Debug.LogError($"请求异常: {e}");
            return null;
        }

    }
}



