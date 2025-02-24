using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.VersionControl;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] segmentPrefabs;
    public Transform startPoint;
    public int numberOfSegments = 5;

    private Vector3 nextSpawnPosition;

    void Start()
    {
        nextSpawnPosition = startPoint.position;

        for (int i = 0; i < numberOfSegments; i++)
        {
            SpawnSegment();
        }
    }

    Vector3 GetSpawnOffset(GameObject segment)
    {
        Transform entryPoint = segment.transform.Find("EntryPoint");
        Debug.Assert(entryPoint != null);
        
        Vector3 entryOffset = segment.transform.position - entryPoint.position;
        segment.transform.position += entryOffset;

    }

    void SpawnSegment()
    {
        GameObject segment = Instantiate(segmentPrefabs[Random.Range(0, segmentPrefabs.Length)], nextSpawnPosition, Quaternion.identity);
        
        Transform exitPoint = segment.transform.Find("ExitPoint");
        

       
        Debug.Assert(exitPoint != null);

        if (entryPoint != null && exitPoint != null)
        {
            
            nextSpawnPosition = exitPoint.position;
        }
    }
}
