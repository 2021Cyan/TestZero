using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class MapSegment : MonoBehaviour
{
    // Public attributes
    public bool CanBeStart;
    public bool CanFlipX;
    public bool CanFlipY;
    public int Likelihood; // value representing how often segment should occur

    // Private attributes
    private List<GameObject> _terrainComponents;
    private List<Vector3> _allPoints;
    private List<Vector3> _hull;
    private Transform _entryPoint;
    private List<Transform> _exitPoints;

    // Getters
    public List<Vector3> GetHull()
    {
        return _hull;
    }

    public Vector3 GetEntryPoint()
    {
        if (_entryPoint != null) {return _entryPoint.position;};
        return Vector3.zero;
    }

    public string GetName()
    {
        return gameObject.name.Replace("(Clone)", "").Trim();
    }

    public List<Transform> GetExitPoints()
    {
        return _exitPoints;
    }

    public GameObject GetParent()
    {
        return gameObject;
    }

    public int GetNumberOfExits()
    {
        return _exitPoints.Count;
    }

    // Methods
    public void Awake()
    {
        Debug.Log(gameObject.name + "awakened!!");

        // Initialize lists
        _exitPoints = new List<Transform>();
        _terrainComponents = new List<GameObject>();
        _allPoints = new List<Vector3>();
        _hull = new List<Vector3>();

        // Find sole entry
        try 
        {
            _entryPoint = transform.Find("Entry").Find("EntryPoint");
        } catch {
            _entryPoint = transform.Find("PlayerSpawnPoint");
        }
        

        // Find all exit points that exist
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Exit"))
            {
                _exitPoints.Add(child.Find("ExitPoint"));
            }
        }

        Debug.Log(gameObject.name + "has " + _exitPoints.Count.ToString() + " exits");
    }

    public void CalculateHull(Vector3 offset)
    {
        // Find all terrain pieces (walls, ceilings, floors, etc.)
        foreach (Transform child in transform.GetComponentInChildren<Transform>())
        {
            if (child.CompareTag("Terrain"))
            {
                _terrainComponents.Add(child.gameObject);
            }
        }

        // Get points from all terrain objects
        foreach (GameObject terrainComp in _terrainComponents)
        {
            BoxCollider2D collider = terrainComp.GetComponent<BoxCollider2D>();
            Debug.Assert(collider != null);
            foreach (Vector3 point in GetBoxCorners(collider))
            {
                _allPoints.Add(point + offset);
            }
        }

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
        float cross = (a.x - b.x) * (c.y - b.y) - (a.y - b.y) * (c.x - b.x);
        float dot = (a.x - b.x) * (c.x - b.x) + (a.y - b.y) * (c.y - b.y);
        return cross < 0f || (cross == 0f && dot <= 0f);
    }

    private void convex_hull()
    {
        // Ensure hull is empty
        _hull.Clear();

        // Create new list for calculations
        List<Vector3> points = new List<Vector3>(_allPoints);

        // Sort points by x-coordinate
        points.Sort((a, b) => 
            a.x != b.x ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y)
        );

        // Add all reverse points
        points.AddRange(points.AsEnumerable().Reverse());

        // Adjust all points to being relative to world space
        // for (int i = 0; i < points.Count; ++i) {points[i] -= transform.position;}

        // Perform algorithm
        foreach (Vector3 point in points)
        {
            while (_hull.Count >= 2 && remove_middle(_hull[_hull.Count - 2], _hull[_hull.Count - 1], point))
            {
                _hull.RemoveAt(_hull.Count - 1);
            }
            _hull.Add(point);
        }
        _hull.RemoveAt(_hull.Count - 1);
    }

    private void OnDrawGizmos()
    {
        if (_hull != null)
        {
            List<Vector3> points = _hull;
            Gizmos.color = Color.green;
            for (int i = 0; i < points.Count; ++i)
            {
                Gizmos.DrawLine(
                        points[i], points[(i + 1) % points.Count]
                );
                Gizmos.DrawSphere(points[i], 0.5f);
            }
        }
    }

    // Determine if hulls overlap
    public bool Overlaps(List<Vector3> otherHull)
    {
        // Check hull's point for being in otherHull
        foreach (Vector3 point in _hull)
        {
            if (IsPointInsidePolygon(point, otherHull))
            {
                return true;
            }
        }

        // Check other hull's point for being in hull
        foreach (Vector3 point in otherHull)
        {
            if (IsPointInsidePolygon(point, _hull))
            {
                return true;
            }
        }

        // Return false if checks have passed
        return false;
    }

    private bool IsPointInsidePolygon(Vector3 point, List<Vector3> polygon)
    {
        Nullable<int> prev_orientation = null;

        // Check every line in other hull
        for (int j = 0; j < polygon.Count; ++j)
        {
            // Determine orientation of line to point
            int orientation = FindOrientation(
                polygon[j],
                polygon[(j + 1) % polygon.Count],
                point
            );
            
            // Check orientation of previous line
            if (prev_orientation == null) {prev_orientation = orientation;}
            else if (orientation == 0) {return false;}
            else if (orientation != 0 && orientation != prev_orientation) {return false;}
        }
        return true;
    }

    private int FindOrientation(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    {
        float val = (lineEnd.y - lineStart.y) * (point.x - lineEnd.x) 
                  - (lineEnd.x - lineStart.x) * (point.y - lineEnd.y);

        // Check for collinearity
        if (val == 0) {return 0;}

        // Otherwise, return orientation between lines (clockwise or counterclockwise)
        return (val > 0) ? 1 : -1;
    }

    // private bool LinesIntersect(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    // {
    //     // Check if points A and B are on opposite sides of line CD
    //     int o1 = Orientation(a, b, c);
    //     int o2 = Orientation(a, b, d);

    //     // Check if points C and D are on opposite sides of line AB
    //     int o3 = Orientation(c, d, a);
    //     int o4 = Orientation(c, d, b);

    //     // General case: straddling check
    //     if (o1 != o2 && o3 != o4) return true;

    //     // // Special cases: collinear overlap checks (optional if you want to count overlaps as intersections)
    //     // bool OnSegment(Vector3 p, Vector3 q, Vector3 r)
    //     // {
    //     //     return q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
    //     //         q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y);
    //     // }

    //     // if (o1 == 0 && OnSegment(a, c, b)) return true;
    //     // if (o2 == 0 && OnSegment(a, d, b)) return true;
    //     // if (o3 == 0 && OnSegment(c, a, d)) return true;
    //     // if (o4 == 0 && OnSegment(c, b, d)) return true;

    //     // No intersection
    //     return false;
    // }
}