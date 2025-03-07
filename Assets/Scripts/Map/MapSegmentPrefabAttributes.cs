using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using FMOD.Studio;

public class MapSegmentPrefabAttributes : MonoBehaviour
{
    // public string segmentName;
    public bool CanBeStart;
    public bool CanFlipX;
    public bool CanFlipY;
    public int Likelihood; // value representing how often segment should occur
    public Transform EntryPoint;
    public List<Transform> ExitPoints;
    private int _id;
    private List<Vector3> _hull;
    
    // Getters
    public int GetID()
    {
        return _id;
    }
    public List<Vector3> GetHull()
    {
        return _hull;
    }

    // Setters
    public void SetID(int id)
    {
        _id = id;
    }

    public void SetHull(List<Vector3> hull)
    {
        _hull = new List<Vector3>();

        foreach (Vector3 point in hull)
        {
            _hull.Add(transform.TransformPoint(point)); // Convert local to world space
        }
    }

    // Methods
    void Awake()
    {
        // Initialize lists
        ExitPoints = new List<Transform>();

        // Find sole entry
        EntryPoint = transform.Find("Entry").Find("EntryPoint");

        // Find all exit points that exist
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Exit"))
            {
                ExitPoints.Add(child.Find("ExitPoint"));
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_hull != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < _hull.Count; ++i)
            {
                // Gizmos.DrawLine(
                //         _hull[i], 
                //         _hull[(i + 1) % _hull.Count]
                // );
                Gizmos.DrawSphere(_hull[i], 0.5f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
