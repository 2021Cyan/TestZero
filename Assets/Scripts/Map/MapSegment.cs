using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using UnityEditor;

public class MapSegment : ScriptableObject
{
    // Public attributes
    public int ID;
    public List<Vector3> Hull;

    // Private attributes
    private GameObject _prefab;
    private List<GameObject> _terrainComponents;
    private List<Vector3> _allPoints;

    // Methods
    public void Init(int id, GameObject prefab)
    {
        // Set attributes
        ID = id;
        _prefab = prefab;

        // Initialize lists
        _terrainComponents = new List<GameObject>();
        _allPoints = new List<Vector3>();
        Hull = new List<Vector3>();

        // Find all terrain pieces (walls, ceilings, floors, etc.)
        foreach (Transform child in prefab.transform)
        {
            if (child.CompareTag("Terrain"))
            {
                _terrainComponents.Add(child.gameObject);
            }
        }

        // Create hull of segment
        FindHull();
    }

    public MapSegmentPrefabAttributes Spawn(Vector3 spawnPosition)
    {
        // Instantiate new prefab
        GameObject segmentObj = Instantiate(_prefab, spawnPosition, Quaternion.identity);
        MapSegmentPrefabAttributes segmentObjAttributes = segmentObj.GetComponent<MapSegmentPrefabAttributes>();

        // Align current segment so EntryPoint matches previous ExitPoint
        Vector3 entryOffset = segmentObj.transform.position - segmentObjAttributes.EntryPoint.position;
        segmentObj.transform.position += entryOffset;

        // Set id and hull of created prefab
        segmentObjAttributes.SetID(ID);
        segmentObjAttributes.SetHull(CreateOffsetOfHull(entryOffset));

        // Return new prefab
        return segmentObjAttributes;
    }

    // Convex hull functions
    private void FindHull()
    {
        // Get points from all terrain objects
        foreach (GameObject terrainComp in _terrainComponents)
        {
            BoxCollider2D collider = terrainComp.GetComponent<BoxCollider2D>();
            Debug.Assert(collider != null);
            _allPoints.AddRange(GetBoxCorners(collider));
        }

        // Sort points by x-coordinate
        _allPoints.Sort((a, b) => a.x.CompareTo(b.x));

        // Calculate convex hull
        convex_hull();
    }

    // Convex hull helper functions
    private Vector3[] GetBoxCorners(BoxCollider2D collider)
    {
        // Create array and find cen
        Vector3[] corners = new Vector3[4];
        Vector3 center = collider.bounds.center;
        Vector3 extents = collider.bounds.extents;

        // Calculate 4 corners
        corners[0] = new Vector3(center.x - extents.x, center.y - extents.y); // Bottom Left
        corners[1] = new Vector3(center.x + extents.x, center.y - extents.y); // Bottom Right
        corners[2] = new Vector3(center.x - extents.x, center.y + extents.y); // Top Left
        corners[3] = new Vector3(center.x + extents.x, center.y + extents.y); // Top Right

        return corners;
    }

    // Adapted algorithm from PyRival (https://github.com/cheran-senthil/PyRival/blob/master/pyrival/geometry/convex_hull.py)
    private bool remove_middle(Vector3 a, Vector3 b, Vector3 c)
    {
        double cross = (a.x - b.x) * (c.y - b.y) - (a.y - b.y) * (c.x - b.x);
        double dot = (a.x - b.x) * (c.x - b.x) + (a.y - b.y) * (c.y - b.y);
        return cross < 0d || (cross == 0d && dot <= 0d);
    }

    private void convex_hull()
    {
        // Create new list for calculations
        List<Vector3> points = new List<Vector3>();

        // Add all sorted points
        for (int i = 0; i < _allPoints.Count; ++i) {points.Add(_allPoints[i]);}

        // Add all reverse points
        for (int i = _allPoints.Count - 1; i >= 0; --i) {points.Add(_allPoints[i]);}

        // Perform algorithm
        foreach (Vector3 point in points)
        {
            while (Hull.Count >= 2 && remove_middle(Hull[Hull.Count - 2], Hull[Hull.Count - 1], point))
            {
                Hull.RemoveAt(Hull.Count - 1);
            }
            Hull.Add(point);
        }
        Hull.RemoveAt(Hull.Count - 1);
    }

    // Determine if hulls overlap
    public bool Overlaps(List<Vector3> otherHull, Vector3 offset)
    {
        // Copy hull and apply offset
        List<Vector3> offsetHull = CreateOffsetOfHull(offset);

        // Check every combination of lines in each hull for overlaps
        for (int i = 0; i < offsetHull.Count; ++i)
        {
            for (int j = 0; j < otherHull.Count; ++j)
            {
                if (LinesIntersect(
                    offsetHull[i], 
                    offsetHull[(i + 1) % offsetHull.Count], 
                    otherHull[j], 
                    otherHull[(j + 1) % otherHull.Count]
                    ))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private List<Vector3> CreateOffsetOfHull(Vector3 offset)
    {
        // Copy hull and apply offset
        List<Vector3> offsetHull = new List<Vector3>(Hull);
        for (int i = 0; i < offsetHull.Count; ++i) {offsetHull[i] = offsetHull[i] + offset;}
        return offsetHull;
    }

    private int Orientation(Vector3 p, Vector3 q, Vector3 r)
    {
        float val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

        // Check for collinearity
        if (val == 0) {return 0;}

        // Otherwise, return orienatation betweek lines (clockwise or counterclockwise)
        return (val > 0) ? 1 : -1;
    }

    private bool LinesIntersect(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        // Check if points A and B are on opposite sides of line CD
        int o1 = Orientation(a, b, c);
        int o2 = Orientation(a, b, d);

        // Check if points C and D are on opposite sides of line AB
        int o3 = Orientation(c, d, a);
        int o4 = Orientation(c, d, b);

        // General case: straddling check
        if (o1 != o2 && o3 != o4) return true;

        // // Special cases: collinear overlap checks (optional if you want to count overlaps as intersections)
        // bool OnSegment(Vector3 p, Vector3 q, Vector3 r)
        // {
        //     return q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
        //         q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y);
        // }

        // if (o1 == 0 && OnSegment(a, c, b)) return true;
        // if (o2 == 0 && OnSegment(a, d, b)) return true;
        // if (o3 == 0 && OnSegment(c, a, d)) return true;
        // if (o4 == 0 && OnSegment(c, b, d)) return true;

        // No intersection
        return false;
    }
}