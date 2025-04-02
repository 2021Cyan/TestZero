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
    private LevelGenerator _levelGenerator;

    // Setters
    public void SetDestination(Vector3 destination)
    {
        _destination = destination;
    }

    public void SetLevelGenerator(LevelGenerator levelGenerator)
    {
        _levelGenerator = levelGenerator;
    }

    // Behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            // Add object to set of objects on platform
            _entitiesOnPlatform.Add(other.gameObject.transform);

            // Activate platform if player has touched
            if (!_activated && other.CompareTag("Player"))
            {
                _activated = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            // Remove object from set of objects on platform
            _entitiesOnPlatform.Remove(other.gameObject.transform);
        }
    }

    void Update()
    {
        // Do nothing if stopped at destination
        if (_stopped) {return;}

        // Move if activated
        if (_activated)
        {
            // Temporary
            List<Transform> toRemove = new List<Transform>();
            foreach (Transform tf in _entitiesOnPlatform)
            {
                if (tf == null)
                {
                    toRemove.Add(tf);
                    continue;
                }

                if (tf.CompareTag("Enemy"))
                {
                    EnemyBase enemy = tf.GetComponent<EnemyBase>();
                    if (enemy == null || !enemy.isalive)
                    {
                        toRemove.Add(tf);
                    }
                }
            }

            foreach (Transform tf in toRemove)
            {
                _entitiesOnPlatform.Remove(tf);
            }


            _prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, _destination, Velocity * Time.deltaTime);

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

            // Trigger level generator to start next level
            _levelGenerator.StartNextLevel();
            Debug.Log("Raft has started next level!");
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(EntryPoint.position, 0.5f);
        Gizmos.DrawSphere(ExitPoint.position, 0.5f);
    }
}