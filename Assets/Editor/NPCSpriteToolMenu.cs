// using UnityEditor;
// using System.IO;

// /// <summary>
// /// 编辑器菜单：快速调用生成工具
// /// </summary>
// public static class NPCSpriteToolMenu
// {
//     [MenuItem("Tools/NPC工具/从选中Sprite生成4方向NPC")]
//     private static void MenuCreateNPC()
//     {
//         if (Selection.activeObject is not Sprite sprite)
//         {
//             Debug.LogError("请先选中Project窗口中的Sprite精灵图！");
//             return;
//         }

//         NPCSpriteTool.CreateNPCFromSprite(sprite);
//     }
// }