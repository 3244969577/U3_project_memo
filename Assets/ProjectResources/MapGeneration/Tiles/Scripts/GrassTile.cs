using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class GrassTile : RuleTile<GrassTile.Neighbor> {
    
    [Header("继承设置")]
    public RuleTile baseGrassTile; // 现有的Grass规则瓦片
    
    [Header("兼容设置")]
    public TileBase mountainTile; // 山地瓦片

    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Null = 3;
        public const int NotNull = 4;
        public const int Mountain = 5; // 山地瓦片类型
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
            case Neighbor.Mountain: 
                // 匹配山地瓦片，视为非边界
                return tile == mountainTile;
        }
        return base.RuleMatch(neighbor, tile);
    }
    
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        // 如果有基础Grass瓦片，先使用其规则
        if (baseGrassTile != null) {
            baseGrassTile.GetTileData(position, tilemap, ref tileData);
        } else {
            base.GetTileData(position, tilemap, ref tileData);
        }
    }
}