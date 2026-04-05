using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class JsonReader
{
    /// <summary>
    /// 读取指定路径的JSON文件并反序列化为指定类型
    /// </summary>
    public static T ReadJsonFile<T>(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"JSON文件不存在: {filePath}");
                return default;
            }

            string jsonContent = File.ReadAllText(filePath);
            T result = JsonConvert.DeserializeObject<T>(jsonContent);
            Debug.Log($"成功读取JSON: {filePath}");
            return result;
        }
        catch (Exception e)
        {
            Debug.LogError($"读取JSON文件失败: {e.Message}\n{e.StackTrace}");
            return default;
        }
    }

    /// <summary>
    /// 读取默认路径的工作流JSON（原方法，兼容）
    /// </summary>
    public static Dictionary<string, Node> ReadWorkflowJson()
    {
        string filePath = Path.Combine(Application.dataPath, "ProjectResources/Entity/Generator/workflow/w2.json");
        // string filePath = Path.Combine(Application.dataPath, "ProjectResources/Entity/Generator/workflow/w1.json");
        return ReadJsonFile<Dictionary<string, Node>>(filePath);
    }

    /// <summary>
    /// 从Unity工程相对路径读取JSON（原方法，兼容）
    /// </summary>
    public static T ReadJsonFromRelativePath<T>(string relativePath)
    {
        string filePath = Path.Combine(Application.dataPath, relativePath);
        return ReadJsonFile<T>(filePath);
    }

    /// <summary>
    /// 【新增】修改工作流中指定节点的输入参数（如动态修改prompt、seed）
    /// </summary>
    /// <param name="workflow">工作流节点字典</param>
    /// <param name="nodeId">节点ID（如6/7）</param>
    /// <param name="paramKey">参数名（如text/seed）</param>
    /// <param name="value">新值</param>
    public static void ModifyNodeInput(Dictionary<string, Node> workflow, string nodeId, string paramKey, object value)
    {
        if (workflow == null || !workflow.ContainsKey(nodeId))
        {
            Debug.LogWarning($"工作流中无此节点ID: {nodeId}");
            return;
        }
        if (workflow[nodeId].inputs == null)
        {
            workflow[nodeId].inputs = new Dictionary<string, object>();
        }
        if (workflow[nodeId].inputs.ContainsKey(paramKey))
        {
            workflow[nodeId].inputs[paramKey] = value;
        }
        else
        {
            workflow[nodeId].inputs.Add(paramKey, value);
        }
        Debug.Log($"修改节点{nodeId}的{paramKey}为: {value}");
    }

    /// <summary>
    /// 【新增】将对象序列化为JSON字符串（用于调试/请求体构建）
    /// </summary>
    public static string ToJsonString(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}









// using System;
// using System.IO;
// using UnityEngine;
// using Newtonsoft.Json;
// using System.Collections.Generic;

// public class JsonReader
// {
    
//     public static T ReadJsonFile<T>(string filePath)
//     {
//         try
//         {
//             if (!File.Exists(filePath))
//             {
//                 Debug.LogError($"JSON文件不存在: {filePath}");
//                 return default;
//             }

//             string jsonContent = File.ReadAllText(filePath);
//             T result = JsonConvert.DeserializeObject<T>(jsonContent);
//             return result;
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"读取JSON文件失败: {e.Message}");
//             return default;
//         }
//     }

   
//     public static Dictionary<string, Node> ReadWorkflowJson()
//     {
//         string filePath = Path.Combine(Application.dataPath, "ProjectResources/Entity/Generator/workflow/w1.json");
//         return ReadJsonFile<Dictionary<string, Node>>(filePath);
//     }

    
//     public static T ReadJsonFromRelativePath<T>(string relativePath)
//     {
//         string filePath = Path.Combine(Application.dataPath, relativePath);
//         return ReadJsonFile<T>(filePath);
//     }
// }

// [System.Serializable]
// public class Node
// {
//     public Dictionary<string, object> inputs;
//     public string class_type;
//     public Meta _meta;
// }

// [System.Serializable]
// public class Meta
// {
//     public string title;
// }
