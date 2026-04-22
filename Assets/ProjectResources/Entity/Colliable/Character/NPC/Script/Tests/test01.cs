// using UnityEngine;
// using System.Collections.Generic;



// public class TestChat : MonoBehaviour
// {
//     void Start()
//     {
//         List<ChatMessage> testMessages = new List<ChatMessage>();
//         testMessages.Add(new ChatMessage(){Role = "user", Content = "你好，介绍一下你自己"});
        
//         DoubaoApiManager.Instance.SendChatRequest(testMessages, 
//             reply => Debug.Log($"NPC回复：{reply}"),
//             error => Debug.LogError($"失败：{error}")
//         );
//     }
// }