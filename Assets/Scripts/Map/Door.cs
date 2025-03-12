using System;
using UnityEngine;

public class Door : Interactable
{
    // Public attributes
    public Transform ClosedPosition;
    public Transform OpenPosition;
    public float Velocity;
    public float ActivationDistance;

    // Private attributes
    private bool isOpening = false;

    // Behaviour
    void Update()
    {
        // If player is within activation distance, open door
        if (Math.Abs(_player.transform.position.x - transform.position.x) < ActivationDistance) {isOpening = true;}
        else {isOpening = false;}

        // If opening, move toward open position
        if (isOpening)
        {
            transform.position = Vector3.MoveTowards(transform.position, OpenPosition.position, Velocity * Time.deltaTime);
        }

        // If closing, move toward closed position
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, ClosedPosition.position, Velocity * Time.deltaTime);
        }
    }
}
