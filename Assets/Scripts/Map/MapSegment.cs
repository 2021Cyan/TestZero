using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class MapSegment : MonoBehaviour
{
    public string segmentName;
    public bool canBeStart;
    public bool canFlipX;
    public bool canFlipY;
    public int likelihood; // value between 1 and 10 representing how often segment should occur
    public Transform entryPoint;
    public List<Transform> exitPoints;
    public List<GameObject> terrainComponents;
    public List<Vector2> allPoints;
    public List<Vector2> hull;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Initialize lists
        exitPoints = new List<Transform>();
        terrainComponents = new List<GameObject>();
        allPoints = new List<Vector2>();
        hull = new List<Vector2>();

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

        // Find all terrain pieces (walls, ceilings, floors, etc.)
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("Terrain"))
            {
                terrainComponents.Add(child.gameObject);
            }
        }

        // Create hull of segment
        FindHull();
    }

    void FindHull()
    {
        // Get points from all terrain objects
        foreach (GameObject terrainComp in terrainComponents)
        {
            BoxCollider2D collider = terrainComp.GetComponent<BoxCollider2D>();
            Debug.Assert(collider != null);
            allPoints.AddRange(GetBoxCorners(collider));
        }

        // Sort points by x-coordinate
        allPoints.Sort((a, b) => a.x.CompareTo(b.x));

        // Calculate convex hull
        convex_hull();
    }

    // Convex hull helper functions
    Vector2[] GetBoxCorners(BoxCollider2D collider)
    {
        // Create array and find cen
        Vector2[] corners = new Vector2[4];
        Vector2 center = collider.bounds.center;
        Vector2 extents = collider.bounds.extents;

        // Calculate 4 corners
        corners[0] = new Vector2(center.x - extents.x, center.y - extents.y); // Bottom Left
        corners[1] = new Vector2(center.x + extents.x, center.y - extents.y); // Bottom Right
        corners[2] = new Vector2(center.x - extents.x, center.y + extents.y); // Top Left
        corners[3] = new Vector2(center.x + extents.x, center.y + extents.y); // Top Right

        return corners;
    }

    // Adapted algorithm from PyRival (https://github.com/cheran-senthil/PyRival/blob/master/pyrival/geometry/convex_hull.py)
    bool remove_middle(Vector2 a, Vector2 b, Vector2 c)
    {
        double cross = (a.x - b.x) * (c.y - b.y) - (a.y - b.y) * (c.x - b.x);
        double dot = (a.x - b.x) * (c.x - b.x) + (a.y - b.y) * (c.y - b.y);
        return cross < 0d || (cross == 0d && dot <= 0d);
    }

    void convex_hull()
    {
        // Create new list for calculations
        List<Vector2> points = new List<Vector2>();

        // Add all sorted points
        for (int i = 0; i < allPoints.Count; ++i) {points.Add(allPoints[i]);}

        // Add all reverse points
        for (int i = allPoints.Count - 1; i >= 0; --i) {points.Add(allPoints[i]);}

        // Perform algorithm
        foreach (Vector2 point in points)
        {
            while (hull.Count >= 2 && remove_middle(hull[hull.Count - 2], hull[hull.Count - 1], point))
            {
                hull.RemoveAt(hull.Count - 1);
            }
            hull.Add(point);
        }
        hull.RemoveAt(hull.Count - 1);
    }

    // Determine if hulls overlap
    public bool Overlaps(List<Vector2> otherHull)
    {
        
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
