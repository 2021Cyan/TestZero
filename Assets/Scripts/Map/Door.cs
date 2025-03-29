using System;
using UnityEngine;

public class Door : Interactable
{
    // Public attributes
    public Transform ClosedPoint;
    public Transform OpenPoint;
    public float Velocity;

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
            if (PlayerIsNear(ClosedPoint.position)) {_isOpening = true;}
            else {_isOpening = false;}

            // If opening, move toward open position
            if (_isOpening)
            {
                transform.position = Vector3.MoveTowards(transform.position, OpenPoint.position, Velocity * Time.deltaTime);
            }

            // If closing, move toward closed position
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, ClosedPoint.position, Velocity * Time.deltaTime);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 165, 0); // orange
        Gizmos.DrawLine(OpenPoint.position, ClosedPoint.position);
        Gizmos.DrawSphere(OpenPoint.position, 0.5f);
        Gizmos.DrawSphere(ClosedPoint.position, 0.5f);

        // Gizmos.DrawLine(ClosedPosition.position, ClosedPosition.position + new Vector3(ActivationDistance, 0, 0));
        // Gizmos.DrawLine(ClosedPosition.position, ClosedPosition.position + new Vector3(-ActivationDistance, 0, 0));
        // Gizmos.DrawSphere(transform.position, ActivationDistance);
    }
}
