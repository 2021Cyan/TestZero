using UnityEngine;

public class MissileScript_Pod : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float rotationSpeed = 200f;
    public float explosionRadius = 3f;
    public float damage = 100f;
    public GameObject explosionEffect;
    public GameObject enemylock;

    private Transform target;
    private Rigidbody2D rb;
    private bool isExploded = false;

    public GameObject hitmarkerPrefab;
    public GameObject damageTextPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindClosestEnemy();
    }
    void Update()
    {
        if (isExploded)
        {
            return;
        }
        UpdateTarget();  
        UpdateEnemyLock();
        Move();
    }

    private void Move()
    {
        if (target != null)
        {
            Enemy_Soldier es = target.GetComponent<Enemy_Soldier>();
            Vector2 direction;
            if (es != null) 
            {
                direction = (es.getAimPos().position - transform.position).normalized;
            }
            else
            {
                direction = (target.position - transform.position).normalized;
            }
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float currentAngle = transform.rotation.eulerAngles.z;
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
            rb.linearVelocity = transform.right * moveSpeed;
        }
    }

    private void UpdateTarget()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            target = FindClosestEnemy();
        }
    }

    private Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy.transform;
            }
        }
        return closestEnemy;
    }

    private void UpdateEnemyLock()
    {
        if (enemylock != null)
        {
            if (target != null)
            {
                enemylock.SetActive(true);
                Enemy_Soldier es = target.GetComponent<Enemy_Soldier>();

                if (es != null)
                {
                    enemylock.transform.position = es.getAimPos().position;
                }
                else
                {
                    enemylock.transform.position = target.position;
                }

                enemylock.transform.Rotate(Vector3.forward * 100f * Time.deltaTime);
                float distance = Vector3.Distance(transform.position, target.position);
                float scaleFactor = Mathf.Clamp(distance * 0.1f, 0.3f, 5f);
                enemylock.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
            }
            else
            {
                enemylock.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isExploded) return;
        if (other.CompareTag("Enemy") || other.CompareTag("Terrain"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (isExploded)
        {
            return;
        }

        isExploded = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (enemylock != null)
        {
            Destroy(enemylock);
        }

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Enemy"))
            {
                EnemyBase enemy = hit.GetComponent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.TakeDamage((int)damage); 
                    Vector3 hitPosition = hit.ClosestPoint(transform.position);
                    ShowDamageText((int)damage, hitPosition); 
                }
            }
        }
        if (enemylock != null)
        {
            Destroy(enemylock);
        }
        Destroy(gameObject);
    }

    private void ShowDamageText(int damageAmount, Vector3 position)
    {
        if (damageTextPrefab != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab, position, Quaternion.identity);
            DamageText textComponent = damageText.GetComponent<DamageText>();
            if (textComponent != null)
            {
                textComponent.SetDamageText(damageAmount);
            }
        }
    }
}
