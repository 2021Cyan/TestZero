using System;
using Unity.VisualScripting;
using UnityEngine;

public class Raft : Interactable
{
    // Public attributes
    public float Velocity;
    public float ActivationDistance;

    // Private attributes
    private bool _playerHasEntered = false;
    private bool _stopped = false;
    private Vector3 _destination;

    // Setters
    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }

    // Behaviour
    void Update()
    {
        // Do nothing if stopped at destination
        if (_stopped) {return;}

        // Activate if player is close enough
        if (!_playerHasEntered && Vector3.Distance(transform.position, _player.transform.position) < ActivationDistance)
        {
            _playerHasEntered = true;
        }

        // Move if activated
        if (_playerHasEntered)
        {
            transform.position = Vector3.MoveTowards(transform.position, _destination, Velocity);
        }

        // If destination has been reached, stop
        if (transform.position == _destination) 
        {
            // Stop forever
            _stopped = true;

            // Increment player level number
            // TODO

            // Spawn enemies in next level
            // TODO
        }
    }
}