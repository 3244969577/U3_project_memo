// using UnityEngine;
// using Fungus;
// using System.Collections.Generic;
// using RPG.AI.Utility;
// using ProjectResources.Utils.Files;
// using Newtonsoft.Json;

// public class NPCSpriteToolAsync
// {
//     public static NPCSpriteToolAsync Instance { get; private set; } = new NPCSpriteToolAsync();
//     public GameObject npc_Prefab;
//     [Header("调试设置")]

//     public GameObject CreateNPCFromSprite(Sprite fourDirWalkSprite)
//     {
//         GameObject npc = Instantiate(npc_Prefab);

        
//         NPC4DirFrameAnimator anim = npc.GetComponent<NPC4DirFrameAnimator>();
//         SpriteRenderer sr = npc.GetComponent<SpriteRenderer>();
//         sr.sortingOrder = 10;


//         Sprite[,] dirFrameSprites = Split4Dir3FrameSprite(fourDirWalkSprite);
//         anim.SetSpriteRenderer(sr);
//         anim.SetSprites(dirFrameSprites);


//         NPCMovement npcMovement = npc.GetComponent<NPCMovement>();
//         npcMovement.moveRange = 5f; // 移动范围

//         return npc;
//     }
//     private Sprite[,] Split4Dir3FrameSprite(Sprite fullSprite)
//     {
//         Texture2D tex = fullSprite.texture;
//         int frameW = tex.width / 3;
//         int frameH = tex.height / 4;
//         Sprite[,] sprites = new Sprite[4, 3];

//         for (int dir = 0; dir < 4; dir++)
//         {
//             for (int frame = 0; frame < 3; frame++)
//             {
//                 int x = frame * frameW;
//                 int y = tex.height - frameH * (dir + 1);
//                 sprites[dir, frame] = Sprite.Create(tex, new Rect(x, y, frameW, frameH), new Vector2(0.5f, 0.5f), tex.width);
//             }
//         }
//         Debug.Log($"✅ 合图切割完成，方向：{sprites.GetLength(0)}，帧：{sprites.GetLength(1)}");
//         return sprites;
//     }

// }


