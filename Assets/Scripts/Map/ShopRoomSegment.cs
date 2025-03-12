using System.Collections.Generic;
using UnityEngine;

public class ShopRoomSegment : MapSegment
{
    // Public attributes
    private List<Door> Doors;

    // Behaviour
    new void Awake()
    {
        base.Awake();
        Doors = new List<Door>(GetComponentsInChildren<Door>());
    }

    public void PassPlayerToDoors(GameObject player)
    {
        foreach (Door door in Doors) {door.SetPlayer(player);}
    }
}
