using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using CoordMap = System.Collections.Generic.Dictionary<(int x, int y), Coord>;
using static Events;


public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    public Tilemap floorTilemap, wallTilemap, eventTilemap, reachableTilemap;
    [SerializeField]
    public TileBase floorTile, floorTileVisited, wallTop, wallSideRight, wallSiderLeft, wallBottom, wallFull,
        wallInnerCornerDownLeft, wallInnerCornerDownRight,
        wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft,
        hero1Tile, hero2Tile, hero3Tile, hero4Tile, reachableTile, enemyTile, LootTile, porteTile,targetTile, wallTileNotVisible, porte;
    public void PaintFloorTiles(CoordMap coords, int scale)
    {
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        HashSet<Vector2Int> notVisited = new HashSet<Vector2Int>();
        foreach (var coord in coords.Values)
        {
            if (coord._isVisible)
            {
                visited.Add(new Vector2Int(coord.x_vector, coord.y_vector));
            }
            else
            {
                notVisited.Add(new Vector2Int(coord.x_vector, coord.y_vector));
            }
        }
        PaintTiles(visited, floorTilemap, floorTileVisited, scale);
        PaintTiles(notVisited, floorTilemap, floorTile, scale);
    }
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions, int scale)
    {
        PaintTiles(floorPositions, floorTilemap, floorTileVisited, scale);
    }
    public void PaintReachableTiles(IEnumerable<Vector2Int> floorPositions, int scale)
    {
        PaintTiles(floorPositions, reachableTilemap, reachableTile, scale);
    }
    public void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile, int scale)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position * scale);
        }
    }
    public void PaintEventTile(Vector2Int position, EventCoord Event, Hero heros, int scale)
    {

        switch (Event)
        {
            case EventCoord.Nothing:
                break;
            case EventCoord.Wall:
                break;
            case EventCoord.Loot:
                PaintSingleTile(eventTilemap, LootTile, position * scale);
                break;
            case EventCoord.Enemy:
                PaintSingleTile(eventTilemap, enemyTile, position * scale);
                break;
            case EventCoord.Trap:
                break;
            case EventCoord.DoorLocked:
                //PaintSingleTile(eventTilemap, porteTile, position * scale);
                break;
            case EventCoord.Boss:
                break;
            default:
                break;
        }

        switch (heros)
        {
            case Hero.Nothing:
                break;
            case Hero.Hero1:
                PaintSingleTile(eventTilemap, hero1Tile, position * scale);
                break;
            case Hero.Hero2:
                PaintSingleTile(eventTilemap, hero2Tile, position * scale);
                break;
            case Hero.Hero3:
                PaintSingleTile(eventTilemap, hero3Tile, position * scale);
                break;
            case Hero.Hero4:
                PaintSingleTile(eventTilemap, hero4Tile, position * scale);
                break;
            default:
                break;
        }

    }
    internal void PaintSingleBasicWall(Vector2Int position, string binaryType, int scale, bool isVisible, bool isDoor = false)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (!isVisible)
        {
            tile = wallTileNotVisible;
        }
        else if (isDoor)
        {
            tile = porte;
        }
        else if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSiderLeft;
        }
        else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position * scale);

    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    public void ClearVision()
    {
        reachableTilemap.ClearAllTiles();
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType, int scale, bool isVisible, bool isDoor = false)
    {
        int typeASInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (!isVisible)
        {
            tile = wallTileNotVisible;
        }
        else if (isDoor)
        {
            tile = porte;
        }
        else if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeASInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeASInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
            PaintSingleTile(wallTilemap, tile, position * scale);
    }

    public void ChangeColorTile(Vector2Int position, Color color, int scale)
    {
        var tilePosition = eventTilemap.WorldToCell((Vector3Int)position * scale);
        eventTilemap.SetTileFlags(tilePosition, TileFlags.None);
        eventTilemap.SetColor(tilePosition, color);
    }
    public void ChangeColorTileWall(Vector2Int position, Color color, int scale)
    {
        var tilePosition = wallTilemap.WorldToCell((Vector3Int)position * scale);
        wallTilemap.SetColor(tilePosition, color);
    }

}
