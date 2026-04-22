// using UnityEngine;
// using Newtonsoft.Json;

// public class JSONTest : MonoBehaviour
// {
//     public string targetString;
//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
        
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.Y))
//         {
//             Debug.Log(targetString);
//             if (!string.IsNullOrEmpty(targetString))
//             {
//                 try
//                 {
//                     // 使用正确的反序列化方法
//                     NPCLLMRsp rsp = JsonConvert.DeserializeObject<NPCLLMRsp>(targetString);
//                     if (rsp != null)
//                     {
//                         if (rsp.social_alter != null)
//                         {
//                             Debug.Log($"name: {rsp.social_alter.name}");
//                             Debug.Log($"relation: {rsp.social_alter.relation}");
//                         }
//                         else
//                         {
//                             Debug.Log("social_alter is null");
//                         }
//                     }
//                     else
//                     {
//                         Debug.Log("Deserialized object is null");
//                     }
//                 }
//                 catch (System.Exception e)
//                 {
//                     Debug.LogError($"JSON deserialization error: {e.Message}");
//                 }
//             }
//             else
//             {
//                 Debug.Log("targetString is empty");
//             }
//         }
//     }
// }
