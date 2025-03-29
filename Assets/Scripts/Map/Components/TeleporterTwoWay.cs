using System.Collections.Generic;
using UnityEngine;

public class TeleporterTwoWay : Interactable
{
    // Public attributes
    public TeleporterTwoWay LinkedTeleporter;
    public float CooldownTime;

    // Private attributes
    private bool _isCoolingDown = true;
    private float _timer;
    private HashSet<Transform> _entitiesWithin = new HashSet<Transform>();

    // Behaviour
    void Update()
    {
        // If cooling down
        if (_isCoolingDown)
        {   
            // Update timer
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                _isCoolingDown = false;
                _timer = 0f;
                UpdateAppearance();
            }
        }

        // Otherwise
        else
        {
            if (_entitiesWithin.Count > 0)
            {
                // Teleport all entities within hitbox
                foreach (Transform entity in _entitiesWithin)
                {
                    entity.position = LinkedTeleporter.transform.position;
                    LinkedTeleporter.AddEntity(entity);
                }
                _entitiesWithin.Clear();

                // Cooldown both teleporters
                Cooldown();
                LinkedTeleporter.Cooldown();

                // Set timer
                _timer = CooldownTime;
            }
        }
    }

    public void AddEntity(Transform entity)
    {
        _entitiesWithin.Add(entity);
    }

    public void Cooldown()
    {
        _isCoolingDown = true;
        _timer = CooldownTime;
        UpdateAppearance();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Remember entity as being within teleporter hitbox
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            _entitiesWithin.Add(other.gameObject.transform);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Forget entity as being within teleporter hitbox
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            _entitiesWithin.Remove(other.gameObject.transform);
        }
    }

    void UpdateAppearance()
    {
        // Display inactive appearance
        if (_isCoolingDown)
        {
            // TODO
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }

        // Display active appearance
        else
        {
            // TODO
            gameObject.GetComponent<Renderer>().material.color = Color.cyan;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (LinkedTeleporter != null)
        {
            Gizmos.DrawLine(transform.position, LinkedTeleporter.transform.position);
        }
    }
}