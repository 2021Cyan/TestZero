using UnityEngine;
using System.Collections.Generic;

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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // Initialize lists
        exitPoints = new List<Transform>;
        terrainComponents = = new List<GameObject>;
        allPoints = new List<Vector2>;

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
        CreateConvexHull();
    }

    void CreateConvexHull()
    {
        // Get points from all terrain objects
        foreach (GameObject terrainComp in terrainComponents)
        {
            BoxCollider2D collider = child.GetComponent<BoxCollider2D>();
            Debug.Assert(collider != null);
            allPoints.AddRange(GetBoxCorners(collider));
        }
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
        double cross = (a.X - b.X) * (c.Y - b.Y) - (a.Y - b.Y) * (c.X - b.X);
        double dot = (a.X - b.X) * (c.X - b.X) + (a.Y - b.Y) * (c.Y - b.Y);
        return (cross < 0d || (cross == 0d && dot <= 0d));
    }

    Vector2[]

   

    // Update is called once per frame
    void Update()
    {
        
    }
}
