using System;
using System.IO;
using UnityEngine;

namespace ProjectResources.Utils.Files
{
    /// <summary>
    /// 文件操作工具类
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// 读取指定文件（相对路径，assets为根节点）
        /// </summary>
        /// <param name="relativePath">相对于Assets文件夹的路径</param>
        /// <returns>文件内容字符串，如果文件不存在或读取失败则返回空字符串</returns>
        public static string ReadFileAsString(string relativePath)
        {
            try
            {
                // 构建完整的文件路径
                string fullPath = Path.Combine(Application.dataPath, relativePath);
                
                // 检查文件是否存在
                if (!File.Exists(fullPath))
                {
                    Debug.LogError($"文件不存在: {fullPath}");
                    return string.Empty;
                }
                
                // 读取文件内容
                string content = File.ReadAllText(fullPath);
                Debug.Log($"成功读取文件: {relativePath}");
                return content;
            }
            catch (Exception e)
            {
                Debug.LogError($"读取文件失败: {e.Message}");
                return string.Empty;
            }
        }
        
        /// <summary>
        /// 写入文件到指定路径（相对路径，assets为根节点）
        /// </summary>
        /// <param name="relativePath">相对于Assets文件夹的路径</param>
        /// <param name="content">要写入的内容</param>
        /// <returns>是否写入成功</returns>
        public static bool WriteFileToAssets(string relativePath, string content)
        {
            try
            {
                // 构建完整的文件路径
                string fullPath = Path.Combine(Application.dataPath, relativePath);
                
                // 确保目录存在
                string directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // 写入文件内容
                File.WriteAllText(fullPath, content);
                Debug.Log($"成功写入文件: {relativePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"写入文件失败: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// 检查文件是否存在（相对路径，assets为根节点）
        /// </summary>
        /// <param name="relativePath">相对于Assets文件夹的路径</param>
        /// <returns>文件是否存在</returns>
        public static bool FileExists(string relativePath)
        {
            string fullPath = Path.Combine(Application.dataPath, relativePath);
            return File.Exists(fullPath);
        }
    }
}
