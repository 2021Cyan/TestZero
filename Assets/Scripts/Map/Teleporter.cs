using UnityEngine;

public class Teleporter : Interactable
{
    // Attributes
    private Vector3 _destination;

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
        _destination = gameObject.transform.position;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Update position of player
            // Debug.Log(other.transform.parent.name + "teleported");
            _player.transform.position = _destination;
        }
    }

    void OnDrawGizmos()
    {
        return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _destination);
    }

}
