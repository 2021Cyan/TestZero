using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class TeleporterTwoWay : Interactable
{
    // Public attributes
    public TeleporterTwoWay LinkedTeleporter;
    public float CooldownTime;

    // Private attributes
    private bool _isCoolingDown = true;
    private float _cooldownTimer;
    private HashSet<Transform> _entitiesWithin = new HashSet<Transform>();

    [Header("Portal")]
    [SerializeField] private GameObject _portal;
    [SerializeField] private ParticleSystem _innerEmbers;
    [SerializeField] private ParticleSystem _outterEmbers;
    [SerializeField] private GameObject _objWithLight;
    [SerializeField] private float _moveRange = 0.075f;
    [SerializeField] private float _moveTime = 1f;
    private Light2D _light2D;

    // Behaviour
    void Update()
    {
        // If cooling down
        if (_isCoolingDown)
        {   
            // Update timer
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f)
            {
                _isCoolingDown = false;
                _cooldownTimer = 0f;
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
                _cooldownTimer = CooldownTime;
            }
        }
    }

    private void Start()
    {
       _objWithLight.TryGetComponent(out _light2D);
       WaitFor(Random.Range(0, _moveTime));
       StartCoroutine(MovePortal(_portal));
    }

    private IEnumerator MovePortal(GameObject _portal)
    {
        Vector3 originalPosition = _portal.transform.position;
        
        while (true)
        {
            // Recalculate positions each cycle to account for potential _moveRange changes
            Vector3 upPosition = originalPosition + Vector3.up * _moveRange;
            Vector3 downPosition = originalPosition + Vector3.down * _moveRange;
            
            // Move up
            yield return MoveToPosition(_portal, originalPosition, upPosition, _moveTime);
            
            // Move back to center
            yield return MoveToPosition(_portal, upPosition, originalPosition, _moveTime);
            
            // Move down
            yield return MoveToPosition(_portal, originalPosition, downPosition, _moveTime);
            
            // Move back to center
            yield return MoveToPosition(_portal, downPosition, originalPosition, _moveTime);
        }
    }
    
    private IEnumerator MoveToPosition(GameObject _portal, Vector3 start, Vector3 end, float _time)
    {
        float elapsedTime = 0;
        
        while (elapsedTime < _time)
        {
            _portal.transform.position = Vector3.Lerp(start, end, elapsedTime / _time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Ensure we reach the exact position
        _portal.transform.position = end;
    }

    public void AddEntity(Transform entity)
    {
        _entitiesWithin.Add(entity);
    }

    public void Cooldown()
    {
        _isCoolingDown = true;
        _cooldownTimer = CooldownTime;
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
            // Set the material color to red to indicate inactive state
            var innerEmbersColor = _innerEmbers.colorOverLifetime;
            innerEmbersColor.color = new ParticleSystem.MinMaxGradient(gradientRed);

            var _outterEmbersMain = _outterEmbers.main;
            _outterEmbersMain.startColor = new ParticleSystem.MinMaxGradient(redPortal);

            StartCoroutine(ChangeLightColor(_light2D, _light2D.color, redPortal, 1f));
        }
        else
        {
            // Adjust the particle system to reflect inactive state
            var innerEmbersColor = _innerEmbers.colorOverLifetime;
            innerEmbersColor.color = new ParticleSystem.MinMaxGradient(gradientBlue);

            var _outterEmbersMain = _outterEmbers.main;
            _outterEmbersMain.startColor = new ParticleSystem.MinMaxGradient(bluePortal);

            StartCoroutine(ChangeLightColor(_light2D, _light2D.color, bluePortal, 1f));
        }
    }

    private IEnumerator ChangeLightColor(Light2D light, Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            light.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        light.color = endColor;
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