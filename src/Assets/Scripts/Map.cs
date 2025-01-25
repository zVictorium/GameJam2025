using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class Map : MonoBehaviour
{
    [SerializeField] private bool isWall = false;
    private Tilemap tilemap;
    private Dictionary<Vector3Int, TileBase> initialTiles;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        SaveInitialTiles();
    }

    private void SaveInitialTiles()
    {
        initialTiles = new Dictionary<Vector3Int, TileBase>();
        BoundsInt bounds = tilemap.cellBounds;

        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                initialTiles[pos] = tile;
            }
        }
    }

    public bool IsWall()
    {
        return isWall;
    }

    public void DeleteTile(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        tilemap.SetTile(cellPosition, null);
    }

    public void ResetTilemap()
    {
        tilemap.ClearAllTiles();
        foreach (var kvp in initialTiles)
        {
            tilemap.SetTile(kvp.Key, kvp.Value);
        }
    }
}
