using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class OneWayTeleporter : Interactable
{
    // Public attributes
    public Transform[] DestinationPoints;

    // Private attributes
    private List<Vector3> _destinations = new List<Vector3>();

    [Header("Portal")]
    [SerializeField] private GameObject _portal;
    [SerializeField] private ParticleSystem _innerEmbers;
    [SerializeField] private ParticleSystem _outterEmbers;
    [SerializeField] private float _moveRange = 0.075f;
    [SerializeField] private float _moveTime = 1f;


    void Start()
    {
        for (int i = 0; i < DestinationPoints.Length; ++i)
        {
            _destinations.Add(DestinationPoints[i].position);
        }
        StartCoroutine(MovePortal(_portal));
    }

    private IEnumerator MovePortal(GameObject _portal)
    {
        Vector3 originalPosition = _portal.transform.position;
        
        while (true)
        {
            // Recalculate positions each cycle to account for potential _moveRange changes
            Vector3 upPosition = originalPosition + Vector3.up * _moveRange;
            Vector3 downPosition = originalPosition + Vector3.down * _moveRange;
            
            // Move up
            yield return MoveToPosition(_portal, originalPosition, upPosition, _moveTime);
            
            // Move back to center
            yield return MoveToPosition(_portal, upPosition, originalPosition, _moveTime);
            
            // Move down
            yield return MoveToPosition(_portal, originalPosition, downPosition, _moveTime);
            
            // Move back to center
            yield return MoveToPosition(_portal, downPosition, originalPosition, _moveTime);
        }
    }
    
    private IEnumerator MoveToPosition(GameObject _portal, Vector3 start, Vector3 end, float _time)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < _time)
        {
            _portal.transform.position = Vector3.Lerp(start, end, elapsedTime / _time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we reach the exact position
        _portal.transform.position = end;
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

}
