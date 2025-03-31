using UnityEngine;

public class RaftSectionBlock : MonoBehaviour
{
    // Public attributes
    public Transform EntryPoint;
    public Transform ExitPoint;
    public Transform EventSpawnPoint;

    // Private attributes
    private GameObject _event;
    private bool _hasSpawnedEvent= false;

    // Setters
    public void SetEvent(GameObject newEvent)
    {
        _event = newEvent;
    }

    // Behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore trigger if event has already been spawned
        if (_hasSpawnedEvent) {return;}

        // Spawn event if player has entered block
        if (other.CompareTag("Player"))
        {
            if (_event != null)
            {
                SpawnEvent();
            }
        }
    }

    private void SpawnEvent()
    {
        // Spawn event at denoted spawn point
        Instantiate(_event, EventSpawnPoint.position, Quaternion.identity);
        _hasSpawnedEvent = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (EntryPoint != null)
        {
            Gizmos.DrawSphere(EntryPoint.position, 0.5f);
        }
        if (ExitPoint != null)
        {
            Gizmos.DrawSphere(ExitPoint.position, 0.5f);
        }
        Gizmos.color = Color.red;
        if (EventSpawnPoint != null)
        {
            Gizmos.DrawSphere(EventSpawnPoint.position, 0.5f);
        }
    }
}