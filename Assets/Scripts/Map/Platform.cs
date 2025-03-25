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
    private HashSet<Transform> _entitiesOnPlatform = new HashSet<Transform>();

    // Behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            // Add object to set of objects on platform
            _entitiesOnPlatform.Add(other.gameObject.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && (other.CompareTag("Player") || other.CompareTag("Enemy")))
        {
            // Remove object from set of objects on platform
            _entitiesOnPlatform.Remove(other.gameObject.transform);
        }
    }

    void Update()
    {
        // Flip direction if bounds are reached
        if (transform.position.x <= LeftPosition.position.x) {_isGoingRight = true;}
        else if (transform.position.x >= RightPosition.position.x) {_isGoingRight = false;}

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
        foreach (Transform tf in _entitiesOnPlatform)
        {
            tf.position += _movement;
            // rb.linearVelocity = new Vector2(_movement.x / Time.fixedDeltaTime, rb.linearVelocity.y);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(LeftPosition.position, 0.5f);
        Gizmos.DrawSphere(RightPosition.position, 0.5f);
        Gizmos.DrawLine(LeftPosition.position, RightPosition.position);
    }
}
