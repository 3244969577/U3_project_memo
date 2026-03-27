using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class DoubaoApiManager : MonoBehaviour
{
    public static DoubaoApiManager Instance;
    // 替换成你给的方舟接口地址
    private const string ApiUrl = "https://ark.cn-beijing.volces.com/api/v3/chat/completions";
    // 换成你自己的方舟API Key（还是推荐用云函数代理，不要硬编码在客户端）
    private const string ApiKey = "a6aaec1c-a583-4361-8352-f75bf5ce2527"; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 对外调用的方法签名完全没变！上层代码不用改任何东西
    public void SendChatRequest(List<ChatMessage> messages, System.Action<string> onSuccess, System.Action<string> onFail = null)
    {
        StartCoroutine(RequestCoroutine(messages, onSuccess, onFail));
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
}




// using System.Collections;
// using UnityEngine;
// using UnityEngine.Networking;
// using Newtonsoft.Json;
// using System.Collections.Generic;

// public class DoubaoApiManager : MonoBehaviour
// {
//     public static DoubaoApiManager Instance; // 单例，全局调用
//     // private const string ApiUrl = "https://api.doubao.com/v3/chat/completions";
//     private const string ApiUrl = "https://ark.cn-beijing.volces.com/api/v3/chat/completions";
//     // 注意：如果要发布游戏，不要把API Key硬编码在这里！后面安全部分会讲替代方案
//     private const string ApiKey = "a6aaec1c-a583-4361-8352-f75bf5ce2527"; 
//     private const string Model = "doubao-seed-2-0-pro-260215";

//     /*
//     JSON示例：
// {
//     "model": "doubao-seed-2-0-pro-260215",
//     "input": [
//         {
//             "role": "user",
//             "content": [
//                 {
//                     "type": "input_image",
//                     "image_url": "https://ark-project.tos-cn-beijing.volces.com/doc_image/ark_demo_img_1.png"
//                 },
//                 {
//                     "type": "input_text",
//                     "text": "你看见了什么？"
//                 }
//             ]
//         }
//     ]
// }
//     */
//     private void Awake()
//     {
//         if (Instance == null) Instance = this;
//         else Destroy(gameObject);
//     }
//     // 对外调用的方法：传入对话列表，回调返回NPC回复
//     public void SendChatRequest(List<ChatMessage> messages, System.Action<string> onSuccess, System.Action<string> onFail = null)
//     {
//         StartCoroutine(RequestCoroutine(messages, onSuccess, onFail));
//     }
//     private IEnumerator RequestCoroutine(List<ChatMessage> messages, System.Action<string> onSuccess, System.Action<string> onFail)
//     {
//         // 组装请求体
//         ChatCompletionRequest request = new ChatCompletionRequest();
//         request.Messages = messages;
//         string jsonData = JsonConvert.SerializeObject(request);
//         // 发送POST请求
//         using (UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(ApiUrl, "POST"))
//         {
//             byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
//             webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
//             webRequest.downloadHandler = new DownloadHandlerBuffer();
//             // 设置请求头
//             webRequest.SetRequestHeader("Content-Type", "application/json");
//             webRequest.SetRequestHeader("Authorization", $"Bearer {ApiKey}");
//             // 等待请求返回
//             yield return webRequest.SendWebRequest();
//             // 处理返回结果
//             if (webRequest.result == UnityWebRequest.Result.Success)
//             {
//                 string responseJson = webRequest.downloadHandler.text;
//                 ChatCompletionResponse response = JsonConvert.DeserializeObject<ChatCompletionResponse>(responseJson);
//                 string reply = response.Choices[0].Message.Content;
//                 onSuccess?.Invoke(reply);
//             }
//             else
//             {
//                 Debug.LogError($"API请求失败：{webRequest.error}，返回内容：{webRequest.downloadHandler.text}");
//                 onFail?.Invoke(webRequest.error);
//             }
//         }
//     }
// }