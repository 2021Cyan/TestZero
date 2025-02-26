using UnityEngine;
using System.Collections.Generic;

public class MapSegment : MonoBehaviour
{
    public string segmentName;
    public bool canBeStart;
    public Transform entryPoint;
    public List<Transform> exitPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Get name of prefab
        segmentName = gameObject.name.Replace("(Clone)", "").Trim();

        // Find sole entry
        entryPoint = transform.Find("Entry").Find("EntryPoint");

        // Find all exit points that exist
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Exit"))
            {
                exitPoints.Add(child.Find("ExitPoint"));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
