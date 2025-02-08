using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player stats
    public float movePower = 0f;
    public float walkPower = 0f;
    public float dodgePower = 0f;
    public float jumpPower = 0f;
    public float hp = 100f;
    public float resource = 0f;

    // Gun stats
    public float fireRate = 0f;
    public int maxAmmo = 0;
    public int currentAmmo;
    public float maxSpreadAngle = 0f;
    public float spreadIncreaseRate = 0f;
    public float spreadResetSpeed = 0f;

    // Player Components
    private Rigidbody2D rb;
    private Animator anim;
    Vector3 movement;
    private int direction = 1;
    bool isJumping = false;
    private bool alive = true;
    private Camera maincam;
    private Vector3 mousePos;

    [SerializeField] public RandomizeSprites rs;

    // Gradually increases gravity when ascending.
    private float currentJumpTime = 0f;
    public float maxJumpTime = 0.5f;  

    // Dodge 
    private int dodgeCharges;
    public int maxDodgeCharges = 3;
    public float dodgeRechargeTime = 5f;  
    private bool isDodging = false;  
    public float dodgeDuration = 0.4f;  
    public float dodgeCooldown = 1f;   
    private float lastDodgeTime = -1000f;

    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentAmmo = maxAmmo;
        maincam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        dodgeCharges = maxDodgeCharges;
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (rb.linearVelocity.y > 0)
        {
            // While ascending, gradually increase gravity
            currentJumpTime += Time.deltaTime;
            rb.gravityScale = Mathf.Lerp(1f, 5f, Mathf.Clamp01(currentJumpTime / maxJumpTime));
        }
        else if (rb.linearVelocity.y < 0)
        {
            // While falling, immediately use the higher gravity
            rb.gravityScale = 5f;
            currentJumpTime = 0f;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        Restart();

        if (alive)
        {
            mousePos = maincam.ScreenToWorldPoint(Input.mousePosition);
            //Hurt();
            Dodge();
            Die();
            Jump();
            Move();
            Randomize();

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        anim.SetBool("isJump", false);
    }

    void Move()
    {
        if (isDodging)
            return;

        Vector3 moveVelocity = Vector3.zero;
        anim.SetBool("isRun", false);
        anim.SetBool("isWalkBack", false);

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            // Mouse is on the left, character should face left
            if (mousePos.x < transform.position.x)
            {
                direction = -1;
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
            else
            {
                direction = 1;
                moveVelocity = Vector3.left;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isWalkBack", true);
            }
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            if (mousePos.x < transform.position.x)
            {
                direction = -1;
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isWalkBack", true);
            }
            else
            {
                direction = 1;
                moveVelocity = Vector3.right;
                transform.localScale = new Vector3(direction, 1, 1);
                if (!anim.GetBool("isJump"))
                    anim.SetBool("isRun", true);
            }
        }
        if (anim.GetBool("isWalkBack"))
        {
            transform.position += moveVelocity * walkPower * Time.deltaTime;
        }
        else
        {
            transform.position += moveVelocity * movePower * Time.deltaTime;
        }
    }

    void Jump()
    {
        if (isDodging)
            return;

        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("isJump"))
        {
            isJumping = true;
            anim.SetBool("isJump", true);
        }
        if (!isJumping)
        {
            return;
        }

        // Reset current motion before jumping.
        rb.linearVelocity = Vector2.zero;

        Vector2 jumpVelocity = new Vector2(0, jumpPower);
        rb.AddForce(jumpVelocity, ForceMode2D.Impulse);

        isJumping = false;
    }

    void Dodge()
    {
        if (isDodging || Time.time - lastDodgeTime < dodgeCooldown)
            return;

        // Check if LeftControl is pressed and if a dodge charge is available.
        if (Input.GetKeyDown(KeyCode.LeftControl) && dodgeCharges > 0)
        {
            // Record the time of this dodge.
            lastDodgeTime = Time.time;

            dodgeCharges--;
            StartCoroutine(RechargeDodge());

            // Remove any current momentum.
            rb.linearVelocity = Vector2.zero;

            // Determine dodge direction based on animation state
            Vector3 dodgeDir = Vector3.zero;
            float totalDodgeDistance = 0f;

            if (anim.GetBool("isRun"))
            {
                dodgeDir = new Vector3(direction, 0, 0);
                totalDodgeDistance = dodgePower;
                anim.SetTrigger("slide");
            }
            else if (anim.GetBool("isWalkBack"))
            {
                dodgeDir = new Vector3(-direction, 0, 0);
                totalDodgeDistance = dodgePower / 2f;
                anim.SetTrigger("roll");
            }
            StartCoroutine(PerformDodge(dodgeDir, totalDodgeDistance, dodgeDuration));
        }
    }


    IEnumerator PerformDodge(Vector3 dodgeDir, float totalDistance, float duration)
    {
        // Gradually moves the character in the given direction over a set duration.
        isDodging = true;
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, startPos + dodgeDir * totalDistance, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = startPos + dodgeDir * totalDistance;
        isDodging = false;
    }

    IEnumerator RechargeDodge()
    {
        yield return new WaitForSeconds(dodgeRechargeTime);
        if (dodgeCharges < maxDodgeCharges)
        {
            dodgeCharges++;
        }
    }

    void Hurt()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetTrigger("hurt");
            if (direction == 1)
                rb.AddForce(new Vector2(-5f, 1f), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(5f, 1f), ForceMode2D.Impulse);
        }
    }

    void Randomize()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            rs.RandomizeParts();
        }
    }

    void Die()
    {
        if (hp <= 0)
        {
            anim.SetTrigger("die");
            alive = false;
        }
    }

    void Restart()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            anim.SetTrigger("idle");
            alive = true;
        }
    }

    // Increase EXP and handle level up.
    public void OnEnemyKilled()
    {
        resource += 160f;
    }
}
