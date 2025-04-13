using UnityEngine;
using System.Collections;
using FMOD.Studio;
using FMODUnity;

public class EnemyScript_Wheel : EnemyBase
{
    // Movement
    private Vector3 moveDirection;
    private float moveSpeed;
    private float detectionRange;
   
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;

    // Wheel to spin
    [SerializeField] Transform wheel;
    private float wheelRotationSpeed = 720f;
    private float damage = 5f;
    private float damageCooldown = 1f;
    private float lastDamageTime = -999f;

    [SerializeField] GameObject explosion;
    [SerializeField] GameObject sparkEffect;
    [SerializeField] Transform sparkPos;
    private GameObject sparkInstance;
    private EventInstance wheelSoundInstance;
    private InputManager _input;

    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;

        // Enemy stat
        resourceAmount = 50;
        maxHealth = 50;
        moveSpeed = 3f;
        currentHealth = maxHealth;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if(playerController != null)
        {
            player = playerController.GetAimPos();
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            moveDirection = new Vector3(Mathf.Sign(dirToPlayer.x), 0, 0);
            FlipSprite(moveDirection.x);
        }
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 2f; 
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sparkEffect != null && sparkPos != null)
        {
            sparkInstance = Instantiate(sparkEffect, sparkPos.position, Quaternion.identity, sparkPos);
            sparkInstance.SetActive(false);
        }
        wheelSoundInstance = _audio.GetEventInstance(_audio.Wheel, gameObject);
        wheelSoundInstance.start();


        _input = InputManager.Instance;
        _input.OnMenuPressed += PauseHandler;
    }

    private void PauseHandler()
    {
        if (MenuManager.IsPaused)
        {
            wheelSoundInstance.setPaused(true);
        }
        else
        {
            wheelSoundInstance.setPaused(false);
        }
    }


    void Update()
    {
        if (!isalive)
            return;

        if (CheckWallCollision())
        {
            ChangeDirection();
        }

        if(IsOnGround())
        {
            moveSpeed = 8f;
        }
        else
        {
            moveSpeed = 3f;
        }

        if (sparkInstance != null && !sparkInstance.activeSelf && IsOnGround())
        {
            sparkInstance.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        if (!isalive)
            return;

        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);
        wheelSoundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        RotateWheel();
    }

    private bool IsOnGround()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 1f, LayerMask.GetMask("Terrain"));
    }

    private void RotateWheel()
    {
        if(wheel == null)
        {
            return;
        }
        float direction = Mathf.Sign(moveDirection.x);
        wheel.Rotate(Vector3.forward * -direction * wheelRotationSpeed * Time.deltaTime);
    }

    private bool CheckWallCollision()
    {
        Vector2 front = transform.position + new Vector3(moveDirection.x * 1.2f, 0, 0);
        return Physics2D.Linecast(transform.position, front, LayerMask.GetMask("Terrain"));
    }


    private void ChangeDirection()
    {
        moveDirection.x = -moveDirection.x;
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox_Player"))
        {
            if (Time.time - lastDamageTime < damageCooldown)
            {
                return;
            }

            PlayerController playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null && playerController.IsAlive() && !playerController.GetPlayerInvincible())
            {
                playerController.Hurt(damage);
                lastDamageTime = Time.time;
            }
        }
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
        wheelSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        wheelSoundInstance.release();
        Destroy(explosionInstance.gameObject, 5f);
        base.Die(amount);
    }
}
