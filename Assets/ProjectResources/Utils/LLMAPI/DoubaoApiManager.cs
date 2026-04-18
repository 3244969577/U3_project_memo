using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using GameStatusSystem.PlayerStatus.Events;
using System.Threading.Tasks;

public class DoubaoApiManager : Singleton<DoubaoApiManager>
{
    // 替换成你给的方舟接口地址
    private const string ApiUrl = "https://ark.cn-beijing.volces.com/api/v3/chat/completions";
    // 换成你自己的方舟API Key（还是推荐用云函数代理，不要硬编码在客户端）
    private const string ApiKey = "a6aaec1c-a583-4361-8352-f75bf5ce2527";

    private void Awake()
    {
        base.Awake();
    }

    // 对外调用的方法签名完全没变！上层代码不用改任何东西
    public void SendChatRequest(List<ChatMessage> messages, System.Action<string> onSuccess, System.Action<string> onFail = null)
    {
        StartCoroutine(RequestCoroutine(messages, onSuccess, onFail));
    }

    public async Task<string> SendChatRequestAsync(List<ChatMessage> messages)
    {
        return await RequestChatAsync(messages);
    }

    private IEnumerator RequestCoroutine(List<ChatMessage> messages, System.Action<string> onSuccess, System.Action<string> onFail)
    {
        // -------------------------- 仅修改这里的请求组装逻辑 --------------------------
        // 把原有ChatMessage列表自动转换成方舟要求的input结构
        ArkChatCompletionRequest request = new ArkChatCompletionRequest();
        foreach (var msg in messages)
        {
            // 每条消息转成ArkInputItem，纯文本默认封装成input_text类型
            request.Messages.Add(new ArkInputItem()
            {
                Role = msg.Role,
                Content = new List<ArkContentItem>()
                {
                    new ArkContentItem()
                    {
                        Type = "text",
                        Text = msg.Content
                    }
                }
            });
        }
        string jsonData = JsonConvert.SerializeObject(request);
        Debug.Log($"Request JSON: {jsonData}");
        // -----------------------------------------------------------------------------

        using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(ApiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {ApiKey}");

            yield return webRequest.SendWebRequest();
            Debug.Log($"Response: {webRequest.result}");
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string responseJson = webRequest.downloadHandler.text;
                // 原有解析逻辑完全不用改（方舟返回格式默认兼容OpenAI）
                ChatCompletionResponse response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
                string reply = response.Choices[0].Message.Content;
                onSuccess?.Invoke(reply);
            }
            else
            {
                Debug.LogError($"API请求失败：{webRequest.error}，返回内容：{webRequest.downloadHandler.text}");
                onFail?.Invoke(webRequest.error ?? "请求失败");
            }
        }
    }

    public async Task<string> RequestChatAsync(List<ChatMessage> messages)
    {
        try
        {
            // -------------------------- 请求组装逻辑 --------------------------
            ArkChatCompletionRequest request = new ArkChatCompletionRequest();
            
            foreach (var msg in messages)
            {
                request.Messages.Add(new ArkInputItem()
                {
                    Role = msg.Role,
                    Content = new List<ArkContentItem>()
                    {
                        new ArkContentItem()
                        {
                            Type = "text",
                            Text = msg.Content
                        }
                    }
                });
            }

            string jsonData = JsonConvert.SerializeObject(request);
            Debug.Log($"Request JSON: {jsonData}");
            // -------------------------------------------------------------------

            // 创建 UnityWebRequest
            using UnityWebRequest webRequest = new UnityWebRequest(ApiUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {ApiKey}");

            // 发送请求并等待
            await webRequest.SendWebRequest();

            // 处理结果
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string responseJson = webRequest.downloadHandler.text;
                ChatCompletionResponse response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
                return response.Choices[0].Message.Content;
            }
            else
            {
                Debug.LogError($"请求失败: {webRequest.error}");
                return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"请求异常: {e}");
            return null;
        }
    }
}



