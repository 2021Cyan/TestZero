using System;
using UnityEngine;

public class Platform : Interactable
{
    // Public attributes
    public Transform LeftPosition;
    public Transform RightPosition;
    public float Velocity;

    // Private attributes
    private bool _isGoingRight = false;

    // Behaviour
    void Update()
    {
        // If player is within activation distance, open door
        if (transform.position == LeftPosition.position) {_isGoingRight = true;}
        else {_isGoingRight = false;}

        // If opening, move toward open position
        if (_isGoingRight)
        {
            transform.position = Vector3.MoveTowards(transform.position, RightPosition.position, Velocity * Time.deltaTime);
        }

        // If closing, move toward closed position
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, LeftPosition.position, Velocity * Time.deltaTime);
        }
    }
}
