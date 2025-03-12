using UnityEngine;
using System;

public class StartRoomSegment : MapSegment
{
    // Attributes

    // Getters
    public Vector3 GetPlayerSpawnPosition()
    {
        return GetEntryPoint();
    }
}
