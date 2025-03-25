using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Raft : Interactable
{
    // Public attributes
    public float Velocity;
    public Transform EntryPoint;
    public Transform ExitPoint;

    // Private attributes
    private bool _activated = false;
    private bool _stopped = false;
    private Vector3 _destination;
    private Vector3 _prevPosition;
    private Vector3 _movement;
    private HashSet<Transform> _entitiesOnPlatform = new HashSet<Transform>();

    // Setters
    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }

    // Behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            // Add object to set of objects on platform
            _entitiesOnPlatform.Add(other.gameObject.transform);

            // Activate platform if player has touched
            if (!_activated && other.CompareTag("Player"))
            {
                _activated = true;
            }

            
            // TO DELETE
            if (other.CompareTag("Player")){Debug.Log("Entered");}
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            // Remove object from set of objects on platform
            _entitiesOnPlatform.Remove(other.gameObject.transform);

            // TO DELETE
            if (other.CompareTag("Player")){Debug.Log("Left");}
        }
    }

    void Update()
    {
        // Do nothing if stopped at destination
        if (_stopped) {return;}

        // Move if activated
        if (_activated)
        {
            _prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, _destination, Velocity);

            // Calculate and apply movement to all objects on platform
            _movement = transform.position - _prevPosition;
            foreach (Transform tf in _entitiesOnPlatform)
            {
                tf.position += _movement;
                // rb.linearVelocity = new Vector2(_movement.x / Time.fixedDeltaTime, rb.linearVelocity.y);
            }
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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(EntryPoint.position, 0.5f);
        Gizmos.DrawSphere(ExitPoint.position, 0.5f);
    }
}