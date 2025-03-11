using UnityEngine;
using System;

public class StartRoomSegment : MapSegment
{
    // // Attributes
    // public Transform PlayerSpawnPoint;

    // Getters
    public Vector3 GetPlayerSpawnPosition()
    {
        // if (PlayerSpawnPoint == null) {
        //     throw new Exception("Player spawn point cannot be null in " + GetParent().name);
        // }
        return GetEntryPoint();//PlayerSpawnPoint.position;
    }
}
