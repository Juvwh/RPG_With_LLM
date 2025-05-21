using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer, int scale, Dictionary<(int x, int y), Coord> m_WallCoordSet)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionsList);


        CreateBasicWall(tilemapVisualizer, basicWallPositions, floorPositions, scale, m_WallCoordSet);
        CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions, scale, m_WallCoordSet);




    }

    private static void CreateCornerWalls(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions, int scale, Dictionary<(int x, int y), Coord> m_WallCoordSet)
    {
        foreach (Vector2Int position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (Vector2Int direction in Direction2D.eightDirectionsList)
            {
                Vector2Int neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }

            if (m_WallCoordSet.ContainsKey((position.x, position.y)))
            {

                //Debug.Log("Dictionnaire contient clé");
                Coord coord = m_WallCoordSet[(position.x, position.y)];
                if (coord == null)
                {
                    //Debug.Log("Ce mur n'existe pas");
                }
                else if (coord._isVisible)
                {
                    tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType, scale, true);
                    //Si c'est une porte, on peint un mur de porte
                    if (coord._event == Events.EventCoord.DoorLocked)
                    {
                        tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType, scale, true, true);
                    }
                }
                else
                {
                    tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType, scale, false);
                }

            }
            else
            {
                tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType, scale, true);
            }

            //tilemapVisualizer.PaintSingleCornerWall(position, neighboursBinaryType, scale, true);
        }
    }

    private static void CreateBasicWall(TilemapVisualizer tilemapVisualizer, HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions, int scale, Dictionary<(int x, int y), Coord> m_WallCoordSet)
    {


        foreach (Vector2Int position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (Vector2Int direction in Direction2D.cardinalDirectionsList)
            {
                Vector2Int neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }



            if (m_WallCoordSet.ContainsKey((position.x, position.y)))
            {

                Coord coord = m_WallCoordSet[(position.x, position.y)];
                if (coord == null)
                {
                    //
                }
                else if (coord._isVisible)
                {
                    tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType, scale, true);
                    //Si c'est une porte, on peint un mur de porte
                    if (coord._event == Events.EventCoord.DoorLocked)
                    {
                        tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType, scale, true, true);
                    }
                }
                else
                {
                    tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType, scale, false);
                }

            }
            else
            {
                tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType, scale, true);
            }
            //tilemapVisualizer.PaintSingleBasicWall(position, neighboursBinaryType, scale, true);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (Vector2Int position in floorPositions)
        {
            foreach (Vector2Int direction in directionList)
            {
                Vector2Int neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }
}
