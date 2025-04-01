using System.Collections.Generic;
using System.Linq;
//using UnityEditor.Rendering;
using UnityEngine;

public class OneWayTeleporter : Interactable
{
    // Public attributes
    public Transform[] DestinationPoints;

    // Private attributes
    private List<Vector3> _destinations = new List<Vector3>();

    // Behaviour
    void Start()
    {
        for (int i = 0; i < DestinationPoints.Length; ++i)
        {
            _destinations.Add(DestinationPoints[i].position);
        }
    }

    public void AddDestination(Vector3 destination, bool clear=false)
    {
        if (clear) {_destinations.Clear();}
        _destinations.Add(destination);
    }

    public void AddDestinations(List<Vector3> destinations, bool clear=false)
    {
        if (clear) {_destinations.Clear();}
        _destinations.AddRange(destinations);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Update position of player
            if (_destinations.Count == 1)
            {
                other.gameObject.transform.position = _destinations[0];
            }
            else
            {
                other.gameObject.transform.position = _destinations[Random.Range(0, _destinations.Count)];
            }
        }
    }

    /*
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < _destinations.Count; ++i)
        {
            Gizmos.DrawLine(transform.position, _destinations[i]);
        }

        for (int i = 0; i < DestinationPoints.Length; ++i)
        {
            Gizmos.DrawLine(transform.position, DestinationPoints[i].position);
        }
    }
    */

}
