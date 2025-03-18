using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class Platform : Interactable
{
    // Public attributes
    public Transform LeftPosition;
    public Transform RightPosition;
    public float Velocity;

    // Private attributes
    private bool _isGoingRight = false;
    private Vector3 _prevPosition;
    private Vector3 _movement;
    private HashSet<Rigidbody2D> _entitiesOnPlatform = new HashSet<Rigidbody2D>();

    // Behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            // Add object to set of objects on platform
            _entitiesOnPlatform.Add(rb);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            // Remove object from set of objects on platform
            _entitiesOnPlatform.Remove(rb);
        }
    }

    void Update()
    {
        // Flip direction if bounds are reached
        if (transform.position == LeftPosition.position) {_isGoingRight = true;}
        else {_isGoingRight = false;}

        // If going right, move right
        if (_isGoingRight)
        {
            _prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, RightPosition.position, Velocity * Time.deltaTime);
        }

        // If going left, move left
        else
        {
            _prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, LeftPosition.position, Velocity * Time.deltaTime);
        }

        // Calculate movement
        _movement = transform.position - _prevPosition;

        // Apply movement to all objects on platform
        foreach (Rigidbody2D rb in _entitiesOnPlatform)
        {
            rb.linearVelocity = new Vector2(_movement.x / Time.fixedDeltaTime, rb.linearVelocity.y);
        }
    }
}
