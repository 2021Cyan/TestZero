using System;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] segmentPrefabs;
    public Transform startPoint;
    public int numberOfSegments = 5;

    void Start()
    {
        // Spawn segments recursively
        SpawnSegment(startPoint.position, numberOfSegments);        
    }

    void SpawnSegment(Vector3 nextSpawnPosition, int remainingSegments)
    {
        // Check if max segments have been reached
        if (remainingSegments == 0)
        {
            return;
        }

        // Create next segment
        GameObject segmentObj = Instantiate(segmentPrefabs[UnityEngine.Random.Range(0, segmentPrefabs.Length)], nextSpawnPosition, Quaternion.identity);
        MapSegment segment = segmentObj.GetComponent<MapSegment>();
        remainingSegments -= 1;

        if (segment != null && segment.entryPoint != null && segment.exitPoints.Count > 0)
        {
            // Align current segment so EntryPoint matches previous ExitPoint
            Vector3 entryOffset = segmentObj.transform.position - segment.entryPoint.position;
            segmentObj.transform.position += entryOffset;

            // Recursively create new segments for each exit point
            foreach (Transform exitPoint in segment.exitPoints)
            {
                SpawnSegment(exitPoint.position, remainingSegments / segment.exitPoints.Count);
            }
        }
        else
        {
            Debug.LogWarning("Spawned segment is missing MapSegment data or has no valid exit points.");
        }
    }


}
