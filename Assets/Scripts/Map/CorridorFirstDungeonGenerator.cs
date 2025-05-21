using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CorridorFirstDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    public int corridorLength = 14, corridorCount = 3;
    [SerializeField]
    [Range(0.1f, 1)]
    public float roomPercent = 0.8f;
    [SerializeField]
    public int iterations = 20;
    [SerializeField]
    public int walkLength = 5;
    [SerializeField]
    public bool startRandomlyEachIteration = true;
    [SerializeField]
    private TilemapVisualizer tilemapVisualizer;
    [SerializeField]
    private int scale = 30;

    public bool dense = false;

    [SerializeField]
    private MapManager _MapManager;

    public HashSet<Vector2Int> RunProceduralGeneration()
    {
        tilemapVisualizer.Clear();
        return CorridorFirstGeneration();
    }

    private HashSet<Vector2Int> CorridorFirstGeneration()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>();

        CreateCorridors(floorPositions, potentialRoomPositions);

        HashSet<Vector2Int> roomPositions = CreateRooms(potentialRoomPositions);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions);

        CreateRoomsAtDeadEnd(deadEnds, roomPositions);

        floorPositions.UnionWith(roomPositions);
        floorPositions.Add(new Vector2Int(0, 0));
        floorPositions.Add(new Vector2Int(0, 1));
        floorPositions.Add(new Vector2Int(1, 0));
        floorPositions.Add(new Vector2Int(1, 1));

        tilemapVisualizer.PaintFloorTiles(floorPositions, scale);
        //WallGenerator.CreateWalls(floorPositions, tilemapVisualizer, scale);


        return floorPositions;
    }

    private void CreateRoomsAtDeadEnd(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach (var position in deadEnds)
        {
            if (roomFloors.Contains(position) == false)
            {
                var room = RunRandomWalk(position);
                roomFloors.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var position in floorPositions)
        {
            int neighboursCount = 0;
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                if (floorPositions.Contains(position + direction))
                    neighboursCount++;

            }
            if (neighboursCount == 1)
                deadEnds.Add(position);
        }
        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = RunRandomWalk(roomPosition);
            roomPositions.UnionWith(roomFloor);
        }
        return roomPositions;
    }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> potentialRoomPositions)
    {
        var currentPosition = Vector2Int.zero;
        potentialRoomPositions.Add(currentPosition);
        for (int i = 0; i < corridorCount; i++)
        {
            List<Vector2Int> corridor = null;
            if (dense && (i+1) % 2 == 0)
            {
                corridor = ProceduralALgo.RandomWalkCorridor(currentPosition, corridorLength, true);
            }
            else
            {
                corridor = ProceduralALgo.RandomWalkCorridor(currentPosition, corridorLength, false);
            }
            currentPosition = corridor[corridor.Count - 1];
            potentialRoomPositions.Add(currentPosition);
            floorPositions.UnionWith(corridor);
        }
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
        return floorPositions;
    }
}
