using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class Elevator : MonoBehaviour
{
    // Public attributes
    public Transform TopPoint;
    public Transform BottomPoint;
    public float Velocity;
    public bool WaitForPlayer = false;

    // Private attributes
    private bool _isGoingUp;
    private bool _playerIsOn = false;
    private Vector3 _prevPosition;
    private Vector3 _movement;
    private HashSet<Transform> _entitiesOnPlatform = new HashSet<Transform>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            // Add object to set of objects on platform
            _entitiesOnPlatform.Add(other.gameObject.transform);

            if (other.CompareTag("Player")) {_playerIsOn = true;}
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            // Remove object from set of objects on platform
            _entitiesOnPlatform.Remove(other.gameObject.transform);

            if (other.CompareTag("Player")) {_playerIsOn = false;}
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Flip direction if bounds are reached
        if (transform.position.y <= BottomPoint.position.y) {_isGoingUp = true;}
        else if (transform.position.y >= TopPoint.position.y) {_isGoingUp = false;}

        // If going up, move up
        if (_isGoingUp)
        {
            _prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, TopPoint.position, Velocity * Time.deltaTime);
        }

        // If going down, move down
        else
        {
            _prevPosition = transform.position;
            transform.position = Vector3.MoveTowards(transform.position, BottomPoint.position, Velocity * Time.deltaTime);
        }

        // Calculate movement
        _movement = transform.position - _prevPosition;

        // Apply movement to all objects on platform
        foreach (Transform tf in _entitiesOnPlatform)
        {
            tf.position += _movement;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(TopPoint.position, BottomPoint.position);
    }
}
