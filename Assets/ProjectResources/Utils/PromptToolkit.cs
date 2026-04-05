/// <summary>
/// 游戏内提示词工具类
/// 玩家只需输入外观描述，自动拼接专业正向/反向提示词
/// </summary>
public static class PromptToolkit
{
    // ====================== 固定正向前缀（质量 + 风格）======================
    private const string PositivePrefix = 
        "(rpgchara), masterpiece, best quality, ultra-detailed, 8K, sharp focus, ";

    // ====================== 固定正向后缀（画面优化）======================
    private const string PositiveSuffix = 
        ", (white background:1.2), simple background, multiple views, chibi, multiple views, reference sheet, small sprite, rpgmaker sprite, sharp pixels, sharp edges, masterpiece, best quality, row_1: front, (row_2: left:1.1), row_3: right, row_4: back";

    // ====================== 固定反向提示词（完全屏蔽坏效果）======================
    public const string NegativePrompt = "background, cells, grid, straigt lines, lines, frame, boder, tools, item, weapons, staff, cliping, superposition, cramped, tight, bad, bad feet, text, error, fewer, extra, missing, worst quality, jpeg artifacts, low quality, watermark, unfinished, displeasing, oldest, early, chromatic aberration, signature, artistic error, username, scan, abstract, english text, shiny skin, fumes, fog, clouds, magic, floating light, (blurry),";

    /// <summary>
    /// 构建最终正向提示词
    /// </summary>
    /// <param name="playerAppearancePrompt">玩家输入的外观描述</param>
    public static string BuildPositivePrompt(string playerAppearancePrompt)
    {
        // 自动拼接：前缀 + 玩家输入 + 后缀
        return $"{PositivePrefix}{playerAppearancePrompt.Trim()}{PositiveSuffix}";
    }

    /// <summary>
    /// 获取固定反向提示词（全程不变）
    /// </summary>
    public static string GetFixedNegativePrompt()
    {
        return NegativePrompt;
    }
}