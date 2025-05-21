using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class SimpleRandomWalkDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private Vector2Int startPosition = Vector2Int.zero;
    [SerializeField]
    public int iterations = 10;
    [SerializeField]
    public int walkLength = 10;
    [SerializeField]
    public int scale = 30;
    [SerializeField]
    private bool startRandomlyEachIteration = true;
    [SerializeField]
    private TilemapVisualizer tilemapVisualizer;

    [SerializeField]
    private MapManager _MapManager;

    public HashSet<Vector2Int> RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = RunRandomWalk(startPosition);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions, scale);
        //WallGenerator.CreateWalls(floorPositions, tilemapVisualizer, scale);

        return floorPositions;
    }

    protected HashSet<Vector2Int> RunRandomWalk(Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < iterations; i++)
        {
            var path = ProceduralALgo.SimpleRandomWalk(currentPosition, walkLength);
            floorPositions.UnionWith(path);
            if (startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        floorPositions.Add(new Vector2Int(1, 0));
        floorPositions.Add(new Vector2Int(0, 1));
        floorPositions.Add(new Vector2Int(1, 1));
        return floorPositions;
    }

}
