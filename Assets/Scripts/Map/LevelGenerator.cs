using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Transform StartPoint;
    public int NumberOfSegments;
    public int ShopFrequency;

    // Map segment prefabs
    public GameObject StartRoomPrefab;
    public GameObject ShopRoomPrefab;
    public GameObject[] SegmentPrefabs; // List of all generic segments that can spawn (NO DUPLICATES)

    void Start()
    {
        // Spawn segments recursively
        SpawnSegment(StartPoint.position, NumberOfSegments); 
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

    // void ShuffleSegments()
    // {
    //     List<GameObject> shuffledList = new List<GameObject>(SegmentPrefabs);
    //     for (int i = SegmentPrefabs.Count; i <= 0; --i)
    //     {

    //     }
    // }

    void SpawnSegment(Vector3 nextSpawnPosition, int remainingSegments, MapSegment prevSegment=null)
    {
        // Check if max segments have been reached
        if (remainingSegments <= 0)
        {
            // <><><> Special Demo Addition <><><>
            // Spawn Test room last
            // GameObject testSegmentObj = Instantiate(testRoom, nextSpawnPosition, Quaternion.identity);
            // MapSegment testSegment = testSegmentObj.GetComponent<MapSegment>();
            // Vector3 entryOffset = testSegmentObj.transform.position - testSegment.entryPoint.position;
            // testSegmentObj.transform.position += entryOffset;
            // return;
            Debug.Log("Finished spawning segments");
            return;
        }

        // <><><> Special Demo Addition <><><>
        // if (ShopFrequency != 0 && remainingSegments % ShopFrequency == 0)
        // {
        //     // Spawn shop
        //     Spawn(ShopRoomPrefab, nextSpawnPosition);
        //     remainingSegments -= 1;
        // }

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
            if (prevSegment != null && currentSegment.Overlaps(prevSegment.GetHull())) 
            {
                // Destroy segment and select again
                Destroy(currentSegment.GetParent());
                remainingSegments += 1;
                continue;
            }
            
            // If all checks pass, break
            break;
        }

        // Recursively create new segments for each exit point
        foreach (Transform exitPoint in currentSegment.GetExitPoints())
        {
            SpawnSegment(
                exitPoint.position,
                remainingSegments / currentSegment.GetNumberOfExits(),
                currentSegment
            );
        }
    }
}
