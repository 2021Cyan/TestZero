using UnityEngine;
using System.Collections;

public class EnemyScript_Boss : EnemyBase
{

    // Floating effect
    private float floatStrength = 0.1f;
    private float floatSpeed = 5f;    
    private Vector3 initialPosition;

    // Enemy shooting
    [SerializeField] Transform turret;
    [SerializeField] Transform turret_firePoint;
    [SerializeField] GameObject turret_bullet;
    private LineRenderer lineRenderer;

    // Enemy missile
    [SerializeField] GameObject missile;

    private bool isPlayerNearby;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;



    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;
        resourceAmount = 200;
        maxHealth = 50000;
        currentHealth = maxHealth;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player = playerController.GetAimPos();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        initialPosition = transform.position;
    }

    void Update()
    {
        isPlayerNearby = CheckNearbyPlayers();
        ApplyFloatingEffect();
    }

    private void ApplyFloatingEffect()
    {
        float floatingY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = initialPosition + new Vector3(0, floatingY, 0);
    }


    private bool CheckNearbyPlayers()
    {
        if (player == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, player.position) <= 30f ;
    }


    // Function to deal damage to the enemy
    public override void TakeDamage(float damage)
    {
        if (!isalive)
        {
            return;
        }

        base.TakeDamage(damage);
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    // Function that handles the enemy death
    protected override void Die(int amount)
    {
        isalive = false;
        base.Die(resourceAmount);
    }
}
