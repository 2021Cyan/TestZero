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
    private Camera maincam;
    private Vector3 mousePos;

    private bool alive = true;

    [SerializeField] public RandomizeSprites rs;

    // Movement Control
    private int direction = 1;
    bool isJumping = false;

    // Movement Control (Jump)
    private float currentJumpTime = 0f;
    public float maxJumpTime = 0.5f;

    // Movement Control (Dodge)
    private int dodgeCharges;
    public int maxDodgeCharges = 3;
    public float dodgeRechargeTime = 5f;  
    private bool isDodging = false;  
    public float dodgeDuration = 0.4f;  
    public float dodgeCooldown = 1f;   
    private float lastDodgeTime = -1000f;

    // Singleton Instance
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
        AdjustGravity();
        Restart();
        if (alive)
        {
            mousePos = maincam.ScreenToWorldPoint(Input.mousePosition);
            Hurt();
            Dodge();
            Die();
            Jump();
            Move();
            Randomize();

        }
    }

    void AdjustGravity()
    {
        if (isDodging)
        {
            return;
        }

        if (rb.linearVelocity.y > 0)
        {
            currentJumpTime += Time.deltaTime;
            rb.gravityScale = Mathf.Lerp(1f, 5f, Mathf.Clamp01(currentJumpTime / maxJumpTime));
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = 5f;
            currentJumpTime = 0f;
        }
        else
        {
            rb.gravityScale = 1f;
        }
    }

    void Move()
    {
        if (isDodging)
        {
            return;
        }

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        anim.SetBool("isJump", false);
    }

    void Dodge()
    {
        if (isDodging || Time.time - lastDodgeTime < dodgeCooldown)
            return;

        if (Input.GetKeyDown(KeyCode.LeftControl) && dodgeCharges > 0)
        {
            lastDodgeTime = Time.time;
            dodgeCharges--;
            StartCoroutine(RechargeDodge());

            rb.linearVelocity = Vector2.zero;

            Vector2 dodgeDir = new Vector2(direction, 0);
            float totalDodgeDistance = dodgePower;

            if (anim.GetBool("isRun"))
            {
                anim.SetTrigger("slide");
                rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                Debug.Log(dodgeDir * totalDodgeDistance);
                StartCoroutine(EndDodge());
            }
            else if (anim.GetBool("isWalkBack"))
            {
                dodgeDir = new Vector2(-direction, 0);
                totalDodgeDistance = dodgePower / 2f;
                anim.SetTrigger("roll");
                rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                Debug.Log(dodgeDir * totalDodgeDistance);
                StartCoroutine(EndDodge());
            }
            else if (anim.GetBool("isJump"))
            {
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    if (mousePos.x < transform.position.x)
                    {
                        dodgeDir = new Vector2(-direction, 0).normalized;
                        totalDodgeDistance = dodgePower;
                        anim.SetTrigger("airdash");
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                        Debug.Log(dodgeDir * totalDodgeDistance);
                        StartCoroutine(EndDodge());
                    }
                    else
                    {
                        dodgeDir = new Vector2(-direction, 0).normalized;
                        totalDodgeDistance = dodgePower;
                        anim.SetTrigger("airdash_back");
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                        Debug.Log(dodgeDir * totalDodgeDistance);
                        StartCoroutine(EndDodge());
                    }
                }
                if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    if (mousePos.x < transform.position.x)
                    {
                        dodgeDir = new Vector2(-direction, 0).normalized;
                        totalDodgeDistance = dodgePower;
                        anim.SetTrigger("airdash_back");
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                        Debug.Log(dodgeDir * totalDodgeDistance);
                        StartCoroutine(EndDodge());
                    }
                    else
                    {
                        dodgeDir = new Vector2(-direction, 0).normalized;
                        totalDodgeDistance = dodgePower;
                        anim.SetTrigger("airdash");
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                        Debug.Log(dodgeDir * totalDodgeDistance);
                        StartCoroutine(EndDodge());
                    }
                }
                else
                {
                    dodgeDir = new Vector2(direction, 0).normalized;
                    totalDodgeDistance = dodgePower;
                    anim.SetTrigger("airdash");
                    rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                    Debug.Log(dodgeDir * totalDodgeDistance);
                    StartCoroutine(EndDodge());
                }
            }
        }
    }

    IEnumerator EndDodge()
    {
        isDodging = true;
        yield return new WaitForSeconds(dodgeDuration);
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
            if (mousePos.x > transform.position.x)
                rb.AddForce(new Vector2(-4f, 1f), ForceMode2D.Impulse);
            else
                rb.AddForce(new Vector2(4f, 1f), ForceMode2D.Impulse);
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
