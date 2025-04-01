using UnityEngine;

public class RaftSectionBlock : MonoBehaviour
{
    // Public attributes
    public Transform EntryPoint;
    public Transform ExitPoint;
    public Transform EventSpawnPoint;
    public Transform DroneEventSpawnPoint;

    // Private attributes
    private GameObject _event;
    private bool _hasSpawnedEvent= false;
    private int _levelNumber = 0;

    // Setters
    public void SetEventAndLevelNumber(GameObject newEvent, int levelNumber)
    {
        _event = newEvent;
        _levelNumber = levelNumber;
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
        // If drone event, spawn at exit point
        RaftEvent raftEvent = _event.GetComponent<RaftEvent>();
        if (raftEvent != null && raftEvent.HasDrones)
        {
            raftEvent = Instantiate(_event, DroneEventSpawnPoint.position, Quaternion.identity).GetComponent<RaftEvent>();
            _hasSpawnedEvent = true;
        }

        // Otherwise, spawn at event point
        else
        {
            raftEvent = Instantiate(_event, EventSpawnPoint.position, Quaternion.identity).GetComponent<RaftEvent>();
            _hasSpawnedEvent = true;
        }

        // Convey level number to each enemy in event
        foreach (EnemyBase enemy in raftEvent.GetComponentsInChildren<EnemyBase>())
        {
            enemy.SetLevelNumber(_levelNumber);
        }
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
        if (DroneEventSpawnPoint != null)
        {
            Gizmos.DrawSphere(DroneEventSpawnPoint.position, 0.5f);
        }
    }
}