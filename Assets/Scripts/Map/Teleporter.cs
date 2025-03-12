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
    new void Awake()
    {
        base.Awake();

        // Set default teleport location
        _destination = gameObject.transform.position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && other.CompareTag("Player"))
        {
            // Get collider's parent entity (ideally the player) and update position
            // Debug.Log(other.transform.parent.name + "teleported");
            _player.transform.position = _destination;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, _destination);
    }

}
