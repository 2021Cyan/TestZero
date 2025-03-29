using UnityEngine;

public class OneWayTeleporter : Interactable
{
    // Public attributes
    public Transform DestinationPoint;

    // Private attributes
    private Vector3 _destination;
    private PlayerController playerController;

    // Getters
    public Vector3 GetDestination()
    {
        return _destination;
    }

    // Setters
    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }

    // Behaviour
    void Awake()
    {
        // Set default teleport location
        if (DestinationPoint == null)
        {
            _destination = gameObject.transform.position;
        }
        else 
        {
            _destination = DestinationPoint.position;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Update position of player
            _player.transform.position = _destination;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _destination);
    }

}
