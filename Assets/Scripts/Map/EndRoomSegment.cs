using UnityEngine;

public class EndRoomSegment : MapSegment
{
    // Attributes
    public Teleporter TeleporterInstance;

    // Getters
    
    // Setters
    public void SetTeleporterDestination(Vector3 destination)
    {
        TeleporterInstance.SetDestination(destination);
    }

    public void SetTeleporterPlayer(GameObject player)
    {
        TeleporterInstance.SetPlayer(player);
    }

    // Behaviour
    new void Awake()
    {
        base.Awake();
        TeleporterInstance = GetComponentInChildren<Teleporter>();
    }
}
