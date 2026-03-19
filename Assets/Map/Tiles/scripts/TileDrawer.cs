// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class TileDrawer : MonoBehaviour
// {
//     [Header("Tile Settings")]
//     public Tilemap tilemap; // 指定要使用的Tilemap
//     public TileBase tile; // 指定要使用的Tile
    
//     [Header("Player Settings")]
//     public Transform player; // 玩家的Transform组件
//     private Vector3Int lastPlayerCell; // 上一帧玩家的网格位置

//     // 在指定位置绘制一个正方形
//     // center: 正方形的中心点（世界坐标）
//     // size: 正方形的边长（以Tile数量为单位）
//     public void DrawSquare(Vector3 center, int size)
//     {
//         if (tilemap == null) {
//             Debug.LogError("Tilemap is not assigned!");
//             return;
//         }

//         if (tile == null) {
//             Debug.LogError("Tile is not assigned!");
//             return;
//         }

//         if (size <= 0) {
//             Debug.LogError("Square size must be positive!");
//             return;
//         }

//         // 将世界坐标转换为网格坐标
//         Vector3Int centerCell = tilemap.WorldToCell(center);

//         // 计算正方形的起始位置（左下角）
//         int halfSize = (size - 1) / 2;
//         Vector3Int startCell = centerCell - new Vector3Int(halfSize, halfSize, 0);

//         // 绘制正方形的四个边
//         for (int x = 0; x < size; x++) {
//             for (int y = 0; y < size; y++) {
//                 // 填充
//                 Vector3Int cellPos = startCell + new Vector3Int(x, y, 0);
//                 tilemap.SetTile(cellPos, tile);
//             }
//         }
//     }

//     // 在玩家脚下绘制瓦片
//     private void DrawTileUnderPlayer()
//     {
//         if (player == null || tilemap == null || tile == null) {
//             return;
//         }

//         // 将玩家的世界坐标转换为网格坐标
//         Vector3Int playerCell = tilemap.WorldToCell(player.position);
//         if (tilemap.HasTile(playerCell)) {
//             return;
//         }
//         tilemap.SetTile(playerCell, tile);
//         // // 只在玩家位置变化时绘制瓦片
//         // if (playerCell != lastPlayerCell) {
//         //     // 在玩家当前位置绘制瓦片
//         //     tilemap.SetTile(playerCell, tile);
//         //     lastPlayerCell = playerCell;
//         // }
//     }

//     // 示例：在Start方法中绘制一个测试正方形
//     void Start()
//     {
//         // 示例：在原点绘制一个5x5的正方形
//         DrawSquare(Vector3.zero, 5);
        
//         // 初始化lastPlayerCell
//         // if (player != null) {
//         //     lastPlayerCell = tilemap.WorldToCell(player.position);
//         // }
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         // 每帧检查并在玩家脚下绘制瓦片
//         DrawTileUnderPlayer();
//     }
// }
