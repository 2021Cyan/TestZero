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
    private bool _isOpening = false;
    private bool _isDisabled = false;

    // Setters
    public void Disable() {_isDisabled = true;}
    public void Enable() {_isDisabled = false;}

    // Behaviour
    void Update()
    {
        // If door isn't disabled:
        if (!_isDisabled)
        {
            // If player is within activation distance, open door
            if (Math.Abs(_player.transform.position.x - transform.position.x) < ActivationDistance) {_isOpening = true;}
            else {_isOpening = false;}

            // If opening, move toward open position
            if (_isOpening)
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
}
