using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Speed of the bullet
    public float speed = 50f;
    // Lifetime of the bullet
    public float lifetime = 5f;
    // Rigidbody of the bullet
    private Rigidbody2D rb;
    // Damage of the bullet
    public float damage = 0f;
    public int bulletType = 0;
    public float searchRange = 10f;
    public int max_target = 0;
    private InputManager _Input;
    private Transform player;
    private PlayerController playerController;
    public GameObject damageTextPrefab;

    // For Tracker bullet
    public float homingTurnSpeed = 5f;
    private Vector2 currentHomingDirection;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                damage = playerController.damage;
                bulletType = playerController.bulletType;
                if (bulletType == 2)
                {
                    max_target = 3;
                }
            }
        }

    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        damage = playerController.damage;
        _Input = InputManager.Instance;
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // Calculate the direction based on the rotation angle
        float angle = transform.eulerAngles.z;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(_Input.MouseInput);
        // Adjust direction based on the mouse position
        if (mousePos.x < transform.position.x)
        {
            direction = -direction;
        }

        // Set initial velocity and initialize homing direction
        rb.linearVelocity = direction * speed;
        currentHomingDirection = direction;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Contact with the Enemy 
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Get enemy component
            Enemy enemy = other.GetComponent<Enemy>();

            // Enemy is not null
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
                Vector3 hitPosition = other.ClosestPoint(transform.position);
                ShowDamageText((int)damage, hitPosition);
                if (bulletType == 2)
                {
                    max_target--;
                    damage *= 0.5f;
                    if (damage < 1) damage = 1;
                }
                if (max_target <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (bulletType == 10)
        {
            Track();
        }
        Vector2 moveDirection = rb.linearVelocity.normalized;
        float moveDistance = speed * Time.fixedDeltaTime;

        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, moveDirection, moveDistance, LayerMask.GetMask("Terrain"));
        if (wallHit.collider != null)
        {
            transform.position = wallHit.point + wallHit.normal * 0.1f;

            if (bulletType == 1)
            {
                Ricochet();
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Tracking bullet
    private void Track()
    {
        GameObject target = FindEnemyClosestToMouse();
        if (target != null)
        {
            Vector2 desiredDirection = (target.transform.position - transform.position).normalized;
            currentHomingDirection = Slerp(currentHomingDirection, desiredDirection, homingTurnSpeed * Time.fixedDeltaTime);
            rb.linearVelocity = currentHomingDirection * speed;
            float newAngle = Mathf.Atan2(currentHomingDirection.y, currentHomingDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
    }

    // Helper function for Tracker bullet
    private Vector2 Slerp(Vector2 a, Vector2 b, float t)
    {
        float dot = Mathf.Clamp(Vector2.Dot(a, b), -1f, 1f);
        float theta = Mathf.Acos(dot) * t;
        Vector2 relativeVec = (b - a * dot).normalized;
        return a * Mathf.Cos(theta) + relativeVec * Mathf.Sin(theta);
    }

    // Helper function for Tracker bullet
    private GameObject FindEnemyClosestToMouse()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(_Input.MouseInput);
        mousePos.z = 0f;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, mousePos);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }
        return closestEnemy;
    }

    // Bouncing bullet
    private void Ricochet()
    {
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            Vector3 targetDirection = (nearestEnemy.transform.position - transform.position).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetDirection, Vector2.Distance(transform.position, nearestEnemy.transform.position), LayerMask.GetMask("Terrain"));

            if (hit.collider == null)
            {
                float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                rb.linearVelocity = targetDirection * speed;
                damage *= 0.5f;
                if (damage < 1) damage = 1;
                return;
            }
        }

        Vector2 incomingDirection = rb.linearVelocity.normalized;
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, incomingDirection, 1f, LayerMask.GetMask("Terrain"));

        if (hitWall.collider != null)
        {
            transform.position = hitWall.point + hitWall.normal * 0.1f;

            Vector2 reflectedDirection = Vector2.Reflect(incomingDirection, hitWall.normal);
            float angle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            rb.linearVelocity = reflectedDirection * speed;
            damage *= 0.5f;
            if (damage < 1) damage = 1;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Helper function for Bouncing bullet
    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float shortestDistance = searchRange;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (!Physics2D.Linecast(transform.position, enemy.transform.position, LayerMask.GetMask("Terrain")) && distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        return nearestEnemy;
    }

    // Shows damage text prefab when hitting enemy
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
