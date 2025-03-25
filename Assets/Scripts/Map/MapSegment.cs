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
    public int MinEnemies;
    public int MaxEnemies;

    // Private attributes
    private List<GameObject> _terrainComponents;
    private List<Vector3> _allPoints;
    private List<Vector3> _hull;
    private Transform _entryPoint;
    private List<Transform> _exitPoints;
    private List<Interactable> _interactables;
    private List<EnemySpawnPoint> _spawnPoints;
    private int _numberOfEnemies;

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

    public List<Interactable> GetInteractables()
    {
        return _interactables;
    }

    // Methods
    protected void Awake()
    {
        // Initialize lists
        _exitPoints = new List<Transform>();
        _terrainComponents = new List<GameObject>();
        _allPoints = new List<Vector3>();
        _hull = new List<Vector3>();

        // Find sole entry
        try 
        {
            _entryPoint = transform.Find("Entry").Find("EntryPoint");
        } 
        catch 
        {
            _entryPoint = null;//transform.Find("PlayerSpawnPoint");
        }
        

        // Find all exit points that exist
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Exit"))
            {
                _exitPoints.Add(child.Find("ExitPoint"));
            }
        }
        // Debug.Log("Segment " + name + " has " + _exitPoints.Count + " exits.");

        // Find all enemy spawn points
        _spawnPoints = new List<EnemySpawnPoint>(GetComponentsInChildren<EnemySpawnPoint>());

        // Find all interactables
        _interactables = new List<Interactable>(GetComponentsInChildren<Interactable>());
    }

    public void PassPlayerToInteractables(GameObject player)
    {
        // Pass reference to player object to all interactables in segment
        for (int i = 0; i < _interactables.Count; ++i)
        {
            _interactables[i].SetPlayer(player);
        }
    }

    public void SpawnEnemies(int levelNumber, float scaling)
    {
        // Determine number of enemies
        _numberOfEnemies = Math.Min(
            UnityEngine.Random.Range(
                (int) (MinEnemies + (MinEnemies * levelNumber * scaling)),
                (int) (MaxEnemies + (MaxEnemies * levelNumber * scaling))
            ),
            _spawnPoints.Count
        );

        // Spawn enemies
        HashSet<int> used = new HashSet<int>();
        int spawnPointIndex;
        for (int i = 0; i < _numberOfEnemies; ++i)
        {
            // Pick random point that hasn't been used before
            while (true)
            {
                spawnPointIndex = UnityEngine.Random.Range(0, _spawnPoints.Count);
                if (!used.Contains(spawnPointIndex))
                {
                    used.Add(i);
                    break;
                }
            }
            
            // Spawn random enemy at chosen point
            _spawnPoints[i].Spawn(levelNumber);
        }
    }

    // Convex hull functions
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

    public bool IsPointInsideSegment(Vector3 point)
    {
        return IsPointInsidePolygon(point, _hull);
    }

    private bool IsPointInsidePolygon(Vector3 point, List<Vector3> polygon)
    {
        int? prev_orientation = null;

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
}