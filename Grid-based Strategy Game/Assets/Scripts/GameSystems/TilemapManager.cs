using GridCombat;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public Tilemap[] tilemaps;

    public List<TileBase> grassTiles;
    public List<TileBase> stoneTiles;
    public List<TileBase> wallTiles;
    public List<TileBase> movementTiles;

    public void SetTilemapSprites(int tilemapIndex, Vector3Int[] positions, TileBase[] tile)
    {
        tilemaps[tilemapIndex].SetTiles(positions, tile);
    }
}
