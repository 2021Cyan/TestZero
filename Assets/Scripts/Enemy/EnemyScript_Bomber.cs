using UnityEngine;
using System.Collections;

public class EnemyScript_Bomber : EnemyBase
{
    // Movement
    private Vector3 moveDirection;
    private float moveSpeed;
    private float detectionRange;

    // Hover
    private float hoverHeight = 1.5f; 
    private float hoverForce = 10f; 
    private float hoverDamping = 5f; 

    // Explosion
    [SerializeField] GameObject explosionPrefab;
    private float explosionDelay;
    private float explosionDamage;           

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;
    private bool explosionTriggered = false;
    [SerializeField] GameObject explosion;


    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;

        // Enemy stat
        resourceAmount = 200;
        maxHealth = 100;
        moveSpeed = 3f;
        detectionRange = 15f;
        explosionDelay = 2f;
        explosionDamage = 40f;

        currentHealth = maxHealth;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if(playerController != null)
        {
            player = playerController.GetAimPos();
        }
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f; 
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChangeDirection();
    }

    void Update()
    {
        if (!isalive || explosionTriggered)
        {
            return;
        }

        if (CheckNearbyPlayers())
        {
            moveSpeed = 5f;
            float directionX = Mathf.Sign(player.position.x - transform.position.x);
            FlipSprite(directionX);
            moveDirection = new Vector3(directionX, 0, 0);
        }
        else
        {
            moveSpeed = 3f;
            if (CheckWallCollision())
            {
                ChangeDirection(true);
            }
        }

        MaintainHoverHeight();
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
    }

    private void MaintainHoverHeight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, hoverHeight + 0.5f, LayerMask.GetMask("Terrain"));

        if (hit.collider != null)
        {
            float distanceToGround = hit.distance;
            float heightError = hoverHeight - distanceToGround;
            float force = heightError * hoverForce - rb.linearVelocity.y * hoverDamping;
            rb.AddForce(Vector2.up * force, ForceMode2D.Force);
        }
    }

    private bool CheckWallCollision()
    {
        Vector2 front = transform.position + new Vector3(moveDirection.x * 1.2f, 0, 0);
        return Physics2D.Linecast(transform.position, front, LayerMask.GetMask("Terrain"));
    }

    private bool CheckNearbyPlayers()
    {
        if (player == null)
        {
            return false;
        }
        bool isPathClear = !Physics2D.Linecast(transform.position, player.position, LayerMask.GetMask("Terrain"));
        return Vector3.Distance(transform.position, player.position) <= detectionRange && isPathClear;
    }

    private void ChangeDirection(bool collisonHappen = default(bool))
    {
        if (!collisonHappen)
        {
            moveDirection = Random.Range(0, 2) == 1 ? Vector3.right : Vector3.left;
        }
        else
        {
            moveDirection.x = -rb.linearVelocity.normalized.x;
        }
        transform.localScale = new Vector3(Mathf.Sign(moveDirection.x), 1, 1);
        FlipSprite(moveDirection.x);
    }
    private void FlipSprite(float directionX)
    {
        if (directionX != 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * Mathf.Sign(directionX), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!explosionTriggered && other.gameObject.layer == LayerMask.NameToLayer("Hitbox_Player"))
        {
            explosionTriggered = true;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
            StartCoroutine(ExplosionCountdown());
        }
    }

    private IEnumerator ExplosionCountdown()
    {
        float timer = 0f;
        Vector3 originalScale = transform.localScale;

        while (timer < explosionDelay)
        {
            spriteRenderer.color = (spriteRenderer.color == Color.white) ? Color.red : Color.white;
            float randomScaleFactor = Random.Range(0.8f, 1.2f);
            transform.localScale = originalScale * randomScaleFactor;
            yield return new WaitForSeconds(0.05f);
            timer += 0.1f;
        }

        spriteRenderer.color = Color.white;
        transform.localScale = originalScale;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        _audio.PlayOneShot(_audio.Explosion, transform.position);

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, 5f);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController != null && !playerController.GetPlayerInvincible() && playerController.IsAlive())
                {
                    playerController.Hurt(explosionDamage);
                }
                break;
            }
        }

        base.Die(0);
    }

    public override void TakeDamage(float damage)
    {
        if (!isalive)
            return;

        base.TakeDamage(damage);
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    protected override void Die(int amount)
    {
        isalive = false;
        Vector3 explosionPos = transform.position;
        GameObject explosionInstance = Instantiate(explosion, explosionPos, Quaternion.identity);
        _audio.PlayOneShot(_audio.Explosion, transform.position);
        Destroy(explosionInstance.gameObject, 5f);
        base.Die(amount);
    }
}
