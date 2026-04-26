using UnityEngine;

/// <summary>
/// 表情名称-Sprite映射工具
/// 负责根据表情字符串，返回对应的图片资源
/// 自行在字典/配置表中维护表情名称和贴图对应关系
/// </summary>
public class EmotionSpriteTool : MonoBehaviour
{
    // 在这里配置你的所有NPC表情名称与对应Sprite
    [SerializeField] private SerializableDictionary<string, Sprite> emotionSpriteDict;

    /// <summary>
    /// 根据表情名称获取Sprite
    /// </summary>
    public Sprite GetEmotionSprite(string emotionName)
    {
        if (emotionSpriteDict.TryGetValue(emotionName, out Sprite target))
        {
            return target;
        }
        return null;
    }
}