using System.Collections.Generic;
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

    // Private prefab representations
    private MapSegment _shopRoom;
    private List<MapSegment> _mapSegments;
    

    void Start()
    {
        // Create MapSegment for shop room
        _shopRoom = ScriptableObject.CreateInstance<MapSegment>();
        _shopRoom.Init(-1, ShopRoomPrefab);

        // Create MapSegment list
        _mapSegments = new List<MapSegment>();
        for (int i = 0; i < SegmentPrefabs.Length; ++i) 
        {
            Debug.Log(SegmentPrefabs[i].name);
            Debug.Log(i);
            MapSegment tempSegment = ScriptableObject.CreateInstance<MapSegment>();
            tempSegment.Init(i, SegmentPrefabs[i]);
            _mapSegments.Add(tempSegment);
        }

        // Spawn segments recursively
        SpawnSegment(StartPoint.position, NumberOfSegments); 
    }

    void SpawnSegment(Vector3 nextSpawnPosition, int remainingSegments, MapSegmentPrefabAttributes prevSegment=null)
    {
        // Check if max segments have been reached
        if (remainingSegments == 0)
        {
            // <><><> Special Demo Addition <><><>
            // Spawn Test room last
            // GameObject testSegmentObj = Instantiate(testRoom, nextSpawnPosition, Quaternion.identity);
            // MapSegment testSegment = testSegmentObj.GetComponent<MapSegment>();
            // Vector3 entryOffset = testSegmentObj.transform.position - testSegment.entryPoint.position;
            // testSegmentObj.transform.position += entryOffset;
            // return;
            return;
        }

        // <><><> Special Demo Addition <><><>
        if (ShopFrequency != 0 && remainingSegments % ShopFrequency == 0)
        {
            // Spawn shop
            _shopRoom.Spawn(nextSpawnPosition);
            remainingSegments -= 1;
            return;
        }

        // Select next segment
        MapSegment currentSegment;
        while (true)
        {
            currentSegment = _mapSegments[Random.Range(0, _mapSegments.Count)];

            break;
            // if (prevSegment == null) {break;}

            // // Check for repeated segments
            // // if (currentSegment.ID == prevSegment.GetID()) {continue;}

            // // Check for overlap
            // Vector3 offset = nextSpawnPosition - currentSegment.EntryPoint.position;
            // if (currentSegment.Overlaps(prevSegment.GetHull(), offset)) {continue;}

            // // Break to create segment if all checks pass
            // break;
        }

        // Create segment
        MapSegmentPrefabAttributes currentPrefabAttributes = currentSegment.Spawn(nextSpawnPosition);
        remainingSegments -= 1;

        // Recursively create new segments for each exit point
        foreach (Transform exitPoint in currentPrefabAttributes.ExitPoints)
        {
            SpawnSegment(
                exitPoint.position,
                remainingSegments / currentPrefabAttributes.ExitPoints.Count,
                currentPrefabAttributes
            );
        }
    }
}
