using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Parameters
    public int NumberOfSegmentsPerLevel;
    public int ShopFrequency;
    public int NumberOfLevels;
    public float LevelGap;
    public float EnemySpawnLevelScaling; // value by which min/max enemies spawns are modified

    // Game objects
    public GameObject Player;
    public GameObject Camera;

    // Map segment prefabs
    public GameObject ShopRoomPrefab;
    public GameObject[] StartRoomPrefabs;
    public GameObject[] EndRoomPrefabs;
    public GameObject[] SegmentPrefabs; // List of all generic segments that can spawn (NO DUPLICATES)

    // Private Attributes
    private List<StartRoomSegment> _startRooms;
    private List<List<EndRoomSegment>> _endRooms;
    private List<MapSegment> _levelsegments;

    void Start()
    {
        // Initialize lists
        _startRooms = new List<StartRoomSegment>();
        _endRooms = new List<List<EndRoomSegment>>();
        for (int i = 0; i < NumberOfLevels; ++i)
        {
            _endRooms.Add(new List<EndRoomSegment>());
        }
        _levelsegments = new List<MapSegment>();

        // Create designated number of levels
        Vector3 levelCreationStartPosition = Vector3.zero;
        for (int levelNumber = 0; levelNumber < NumberOfLevels; ++levelNumber)
        {
            // Spawn random start room
            StartRoomSegment startRoom = (StartRoomSegment) Spawn(StartRoomPrefabs[Random.Range(0, StartRoomPrefabs.Length)], levelCreationStartPosition);
            _startRooms.Add(startRoom);

            // If not first level, set teleporter destination of previous level's end rooms
            if (levelNumber != 0)
            {
                foreach (EndRoomSegment endRoom in _endRooms[levelNumber - 1])
                {
                    endRoom.SetTeleporterDestination(startRoom.GetPlayerSpawnPosition());
                    endRoom.SetTeleporterPlayer(Player);
                }
            }

            // Spawn segments recursively
            _levelsegments.Clear();
            SpawnSegment(
                startRoom.GetExitPoints()[0].position, 
                NumberOfSegmentsPerLevel, 
                levelNumber, 
                startRoom
            );

            // Update start point for level creation for next level
            float furthestEndX = -1;
            foreach (EndRoomSegment endRoom in _endRooms[levelNumber]) {furthestEndX = Mathf.Max(furthestEndX, endRoom.transform.position.x);}
            levelCreationStartPosition.x = furthestEndX + LevelGap;
        }
    }

    MapSegment GetMapSegmentFromPrefab(GameObject prefab)
    {
        return prefab.GetComponent<MapSegment>();
    }

    MapSegment Spawn(GameObject prefab, Vector3 spawnPosition)
    {
        // Instantiate new prefab
        GameObject segmentPrefab = Instantiate(prefab, spawnPosition, Quaternion.identity);
        MapSegment segment = GetMapSegmentFromPrefab(segmentPrefab);

        // Align current segment so EntryPoint matches previous ExitPoint
        Vector3 entryOffset = segmentPrefab.transform.position - segment.GetEntryPoint();
        segmentPrefab.transform.position += entryOffset;

        // Trigger convex hull calculation
        segment.CalculateHull(entryOffset);

        // Return MapSegment object of spawned segment
        return segment;
    }

    void SpawnSegment(Vector3 nextSpawnPosition, int remainingSegments, int levelNumber, MapSegment prevSegment=null)
    {
        // Check if max segments have been reached
        if (remainingSegments <= 0)
        {
            // Spawn end room
            EndRoomSegment endRoom = (EndRoomSegment) Spawn(EndRoomPrefabs[Random.Range(0, StartRoomPrefabs.Length)], nextSpawnPosition);
            _endRooms[levelNumber].Add(endRoom);

            // Finish spawning segments for this branch
            return;
        }

        // Spawn shop segment if frequency has been reached
        if (ShopFrequency != 0 && remainingSegments % ShopFrequency == 0)
        {
            // Spawn shop
            ShopRoomSegment shopRoom = (ShopRoomSegment) Spawn(ShopRoomPrefab, nextSpawnPosition);
            shopRoom.PassPlayerToDoors(Player);
            remainingSegments -= 1;

            // Spawn next segment
            SpawnSegment(
                shopRoom.GetExitPoints()[0].position,
                remainingSegments,
                levelNumber,
                shopRoom
            );
            return;
        }

        // Select next segment
        GameObject currentSegmentPrefab;
        MapSegment currentSegment;
        int attempts = 0;
        while (true)
        {
            attempts += 1;
            if (attempts > 1000) {
                Debug.Log("Map creation failed to find suitable segment");
                return;
            }

            currentSegmentPrefab = SegmentPrefabs[Random.Range(0, SegmentPrefabs.Length)];
            currentSegment = GetMapSegmentFromPrefab(currentSegmentPrefab);

            // Check for repeated segments
            if (SegmentPrefabs.Length > 1 && prevSegment != null && currentSegment.GetName() == prevSegment.GetName()) {continue;}

            // Spawn segment
            currentSegment = Spawn(currentSegmentPrefab, nextSpawnPosition);
            remainingSegments -= 1;

            // Check for overlap
            bool overlaps = false;
            foreach (MapSegment otherSegment in _levelsegments)
            {
                if (currentSegment.Overlaps(otherSegment.GetHull()))
                {
                    overlaps = true;
                    break;
                }
            }
            if (overlaps)
            {
                // Destroy segment and select again
                Destroy(currentSegment.GetParent());
                remainingSegments += 1;
                continue;
            }
            
            // If all checks pass, add accepted segment to list and break
            _levelsegments.Add(currentSegment);
            break;
        }

        // Recursively create new segments for each exit point
        foreach (Transform exitPoint in currentSegment.GetExitPoints())
        {
            SpawnSegment(
                exitPoint.position,
                remainingSegments / currentSegment.GetNumberOfExits(),
                levelNumber,
                currentSegment
            );
        }
    }
}
