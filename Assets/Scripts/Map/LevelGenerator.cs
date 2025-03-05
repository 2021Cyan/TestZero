using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] segmentPrefabs; // List of all segments that can spawn (NO DUPLICATES)
    public Transform startPoint;
    public int numberOfSegments = 5;
    private List<int> segmentIndexes = new List<int>();

    // <><><> Special Demo Addition <><><>   
    public GameObject testRoom;
    public GameObject shopRoom;
    public int shopFrequency;

    void Start()
    {
        // Add more segments to array to reflect likelihood
        GameObject segment;
        for (int i = 0; i < segmentPrefabs.Length; ++i) 
        {
            segment = segmentPrefabs[i];
            for (int j = 0; j < segment.GetComponent<MapSegment>().likelihood; ++j)
            {
                segmentIndexes.Add(i);
            }
        }

        // Spawn segments recursively
        SpawnSegment(startPoint.position, numberOfSegments); 
    }

    private bool SegmentsOverlap(List<Vector2> segment1Hull, List<Vector2> segment2Hull)
    {
        return false;
    }

    void SpawnSegment(Vector3 nextSpawnPosition, int remainingSegments, int prevSegment=-1)
    {
        // Check if max segments have been reached
        if (remainingSegments == 0)
        {
            // <><><> Special Demo Addition <><><>
            // Spawn Test room last
            GameObject testSegmentObj = Instantiate(testRoom, nextSpawnPosition, Quaternion.identity);
            MapSegment testSegment = testSegmentObj.GetComponent<MapSegment>();
            Vector3 entryOffset = testSegmentObj.transform.position - testSegment.entryPoint.position;
            testSegmentObj.transform.position += entryOffset;
            return;
        }

        // <><><> Special Demo Addition <><><>
        if (remainingSegments % shopFrequency == 0)
        {
            // Spawn shop
            GameObject shopSegmentObj = Instantiate(shopRoom, nextSpawnPosition, Quaternion.identity);
            MapSegment shopSegment = shopSegmentObj.GetComponent<MapSegment>();
            remainingSegments -= 1;
            Vector3 entryOffset = shopSegmentObj.transform.position - shopSegment.entryPoint.position;
            shopSegmentObj.transform.position += entryOffset;
            SpawnSegment(shopSegment.exitPoints[0].position, remainingSegments);
            return;
        }

        // Select next segment
        int segmentIndex;
        while (true)
        {
            segmentIndex = segmentIndexes[Random.Range(0, segmentIndexes.Count)];
            if (segmentIndex != prevSegment) {break;}
        }

        // Create next segment
        GameObject segmentObj = Instantiate(segmentPrefabs[segmentIndex], nextSpawnPosition, Quaternion.identity);
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
                Debug.Log(segment.name);
                SpawnSegment(exitPoint.position, remainingSegments / segment.exitPoints.Count, prevSegment=segmentIndex);
            }
        }
        else
        {
            Debug.LogWarning(segment.name + " is missing MapSegment data or has no valid exit points.");
        }
    }
}
