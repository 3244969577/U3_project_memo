using UnityEngine;
using System.IO;

public static class TextureConverter
{
    /// <summary>
    /// 从图片路径直接创建 Sprite（自动设置正确格式）
    /// </summary>
    public static Sprite CreateSpriteFromPath(string imagePath)
    {
        // 读取图片字节
        byte[] bytes = File.ReadAllBytes(imagePath);

        // 创建纹理
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        tex.LoadImage(bytes);

        // 🔴 关键：设置成 Sprite 能用的纹理类型
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        // 生成 Sprite
        Rect rect = new Rect(0, 0, tex.width, tex.height);
        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0.5f, 0.5f));

        return sprite;
    }
}