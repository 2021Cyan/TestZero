using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // Parameters
    public int NumberOfSegmentsPerLevel;
    public int ShopFrequency;
    public int NumberOfLevels;
    public float NumberOfBlocksPerRaftSection;
    public float EnemySpawnLevelScaling; // value by which min/max enemies spawns are modified

    // Game objects
    public GameObject Player;

    // Map segment prefabs
    public GameObject ShopRoomPrefab;
    public GameObject StartRoomPrefab;
    public GameObject EndRoomPrefab;
    
    public GameObject[] SegmentPrefabs; // List of all generic segments that can spawn (NO DUPLICATES)

    // Raft section components
    public GameObject RaftSectionStartPrefab;
    public GameObject RaftSectionBlockPrefab;
    public GameObject RaftSectionEndPrefab;
    public GameObject[] Events;
    public int EventFrequency;

    // Private Attributes
    private List<MapSegment> _startRooms;
    private List<List<MapSegment>> _endRooms;
    private List<MapSegment> _levelsegments;

    void Start()
    {
        Player = PlayerController.Instance.gameObject;

        // Initialize lists
        _startRooms = new List<MapSegment>();
        _endRooms = new List<List<MapSegment>>();
        for (int i = 0; i < NumberOfLevels; ++i)
        {
            _endRooms.Add(new List<MapSegment>());
        }
        _levelsegments = new List<MapSegment>();

        // Create designated number of levels
        Vector3 levelCreationStartPosition = Vector3.zero;
        MapSegment levelStartSegment = null;
        for (int levelNumber = 0; levelNumber < NumberOfLevels; ++levelNumber)
        {
            // If first level, spawn start room
            if (levelNumber == 0)
            {
                // Spawn start room
                levelStartSegment = Spawn(StartRoomPrefab, levelCreationStartPosition);
            }

            // // Otherwise, spawn raft end segment
            // else
            // {
            //     levelStartSegment = Spawn(RaftSectionStart, levelCreationStartPosition);
            // }

            // // If not first level, set teleporter destination of previous level's end rooms
            // if (levelNumber != 0)
            // {
            //     foreach (EndRoomSegment endRoom in _endRooms[levelNumber - 1])
            //     {
            //         endRoom.SetTeleporterDestination(startRoom.GetPlayerSpawnPosition());
            //         endRoom.SetTeleporterPlayer(Player);
            //     }
            // }

            // Spawn segments recursively
            _levelsegments.Clear();
            SpawnSegment(
                levelStartSegment.GetExitPoints()[0].position, 
                NumberOfSegmentsPerLevel, 
                levelNumber, 
                levelStartSegment
            );

            // Spawn enemies into current level
            for (int i = 0; i < _levelsegments.Count; ++i)
            {
                _levelsegments[i].SpawnEnemies(levelNumber, EnemySpawnLevelScaling);
            }

            // // Update start point for level creation for next level
            // float furthestEndX = -1;
            // foreach (EndRoomSegment endRoom in _endRooms[levelNumber]) 
            // {
            //     furthestEndX = Mathf.Max(furthestEndX, endRoom.transform.position.x);
            // }
            // levelCreationStartPosition.x = furthestEndX + LevelGap;

            // Get last segment of level
            MapSegment lastSegment;
            if (_levelsegments.Count > 0)
            {
                lastSegment = _levelsegments[_levelsegments.Count -1];
            }
            else
            {
                lastSegment = levelStartSegment;
            }

            // If last level, spawn terminal room
            if (levelNumber == NumberOfLevels - 1)
            {
                MapSegment finalRoom = Spawn(EndRoomPrefab, lastSegment.GetExitPoints()[0].position);
                // finalRoom.SetTeleporterDestination(Vector3.zero);
            }

            // Otherwise, spawn raft section
            else
            {
                // Spawn raft start segment
                MapSegment raftStart = Spawn(RaftSectionStartPrefab, lastSegment.GetExitPoints()[0].position);
                
                // Get raft in start segment
                Raft raft = null;
                foreach (Interactable interactable in raftStart.GetInteractables())
                {
                    if (interactable is Raft)
                    {
                        raft = (Raft) interactable;
                    }
                }

                // Get RaftSectionBlockObject from segment
                RaftSectionBlock raftStartBlock = raftStart.GetParent().GetComponent<RaftSectionBlock>();

                // Spawn desired number of raft section blocks
                GameObject currentPrefab = null;
                RaftSectionBlock currentBlock = raftStartBlock;
                for (int i = 0; i < NumberOfBlocksPerRaftSection; ++i)
                {
                    currentPrefab = Instantiate(RaftSectionBlockPrefab, currentBlock.ExitPoint.position, Quaternion.identity);
                    currentBlock = currentPrefab.GetComponent<RaftSectionBlock>();
                    if (EventFrequency != 0 && i % EventFrequency == 0)
                    {
                        currentBlock.SetEvent(Events[Random.Range(0, Events.Length)]);
                    }
                }

                // Spawn raft end segment and set start room for next level
                currentPrefab = Instantiate(RaftSectionEndPrefab, currentBlock.ExitPoint.position, Quaternion.identity);
                levelStartSegment = currentPrefab.GetComponent<MapSegment>();

                // Set destination of raft
                raft.SetDestination(levelStartSegment.GetEntryPoint());
            }
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

        // Pass player object to interactables of segment
        segment.PassPlayerToInteractables(Player);

        // Return MapSegment object of spawned segment
        return segment;
    }

    void SpawnSegment(Vector3 nextSpawnPosition, int remainingSegments, int levelNumber, MapSegment prevSegment)
    {
        // Check if max segments have been reached
        if (remainingSegments <= 0)
        {
            // // If last level, spawn terminal segment
            // if (levelNumber == NumberOfLevels - 1)
            // {
            //     _endRooms[levelNumber].Add(Spawn(EndRoomPrefab, nextSpawnPosition));
            // }

            // // Otherwise, spawn raft start section
            // else
            // {
            //     _endRooms[levelNumber].Add(Spawn(RaftSectionStart, nextSpawnPosition));
            // }

            // Finish spawning segments for this branch
            return;
        }

        // Spawn shop segment if frequency has been reached
        if (ShopFrequency != 0 && remainingSegments % ShopFrequency == 0)
        {
            // Spawn shop
            MapSegment shopRoom = Spawn(ShopRoomPrefab, nextSpawnPosition);

            // Add segment to list of current level segments
            _levelsegments.Add(shopRoom);

            // Spawn next segment
            SpawnSegment(
                shopRoom.GetExitPoints()[0].position,
                remainingSegments - 1,
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
            if (SegmentPrefabs.Length > 1 && prevSegment != null && currentSegment.Type == prevSegment.Type) {continue;}

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
