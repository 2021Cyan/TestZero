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
    public float max_hp = 100f;
    public float hp;
    public float resource = 0f;
    [SerializeField] private GameObject ResourceText;

    // Gun stats
    public float damage = 6.0f;
    public float fireRate = 4.0f;
    public int maxAmmo = 12;
    public int currentAmmo;
    public float maxSpreadAngle = 30.0f;
    public float spreadIncreaseRate = 6.0f;
    public float spreadResetSpeed = 20.0f;
    public float reloadSpeed = 2f;
    // Bullet modifier -> 0 : normal, 1: Ricochet, 2: Penetration
    public int bulletType = 0;

    // Gun Sprites
    public string gripType = "gun_grip_pistol";
    public int barrelLevel = 1;
    public int frameLevel = 1;
    public int magazineLevel = 1;

    // Player Components
    private Rigidbody2D rb;
    private Animator anim;
    private Camera maincam;
    private Vector3 mousePos;

    private bool alive = true;

    // Movement Control
    private int direction = 1;
    bool isJumping = false;

    // Movement Control (Jump)
    private float currentJumpTime = 0f;
    public float maxJumpTime = 0.5f;

    [Header("Movement Control (Dodge)")]
    private int dodgeCharges;
    public int maxDodgeCharges = 3;
    public float dodgeRechargeTime = 5f;
    private bool isDodging = false;
    public float dodgeDuration = 0.4f;
    public float dodgeCooldown = 1f;
    private float lastDodgeTime = -1000f;

    // Singleton Instance
    public static PlayerController Instance;
    private InputManager _input;
    private AudioManager _audio;

    private void Awake()
    {
        _input = InputManager.Instance;
        _audio = AudioManager.Instance;
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
        hp = max_hp;
        currentAmmo = maxAmmo;
        maincam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        dodgeCharges = maxDodgeCharges;
        _audio.PlayOneShot(_audio.Music);
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(_input.MouseInput);
        AdjustGravity();
        Restart();
        if (alive)
        {
            mousePos = maincam.ScreenToWorldPoint(_input.MouseInput);
            Hurt();
            Dodge();
            Die();
            Jump();
            Move();

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

        if (_input.JumpInput && !anim.GetBool("isJump"))
        {
            isJumping = true;
            _audio.PlayOneShot(_audio.JumpGroan);
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
        if (anim.GetBool("isJump"))
        {
            _audio.PlayOneShot(_audio.Moonwalk);
        }
        anim.SetBool("isJump", false);
    }

    void Dodge()
    {
        if (isDodging || Time.time - lastDodgeTime < dodgeCooldown)
            return;

        if (_input.DodgeInput && dodgeCharges > 0)
        {
            lastDodgeTime = Time.time;
            dodgeCharges--;
            StartCoroutine(RechargeDodge());

            rb.linearVelocity = Vector2.zero;

            Vector2 dodgeDir = new Vector2(direction, 0);
            float totalDodgeDistance = dodgePower;

            if (anim.GetBool("isRun"))
            {
                StartCoroutine(TriggerSlowMotion(1.5f, 0.3f));
                anim.SetTrigger("slide");
                _audio.PlayOneShot(_audio.Dodge);
                rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                StartCoroutine(EndDodge());
            }
            else if (anim.GetBool("isWalkBack"))
            {
                StartCoroutine(TriggerSlowMotion(1.5f, 0.3f));
                dodgeDir = new Vector2(-direction, 0);
                totalDodgeDistance = dodgePower / 2f;
                anim.SetBool("rollCheck", true);
                anim.SetTrigger("roll");
                _audio.PlayOneShot(_audio.JumpGroan);
                rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                StartCoroutine(EndDodge());
            }
            else if (anim.GetBool("isJump"))
            {
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    if (mousePos.x < transform.position.x)
                    {
                        dodgeDir = new Vector2(direction, 0).normalized;
                        totalDodgeDistance = dodgePower * 1.5f;
                        anim.SetTrigger("airdash");
                        _audio.PlayOneShot(_audio.AirDash);
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                        StartCoroutine(EndDodge());
                    }
                    else
                    {
                        dodgeDir = new Vector2(-direction, 0).normalized;
                        totalDodgeDistance = dodgePower;
                        anim.SetTrigger("airdash_back");
                        _audio.PlayOneShot(_audio.AirDash);
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                        StartCoroutine(EndDodge());
                    }
                }
                else if (Input.GetAxisRaw("Horizontal") > 0)
                {
                    if (mousePos.x < transform.position.x)
                    {
                        dodgeDir = new Vector2(-direction, 0).normalized;
                        totalDodgeDistance = dodgePower;
                        anim.SetTrigger("airdash_back");
                        _audio.PlayOneShot(_audio.AirDash);
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                        StartCoroutine(EndDodge());
                    }
                    else
                    {
                        dodgeDir = new Vector2(direction, 0).normalized;
                        totalDodgeDistance = dodgePower * 1.5f;
                        anim.SetTrigger("airdash");
                        _audio.PlayOneShot(_audio.AirDash);
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                        StartCoroutine(EndDodge());
                    }
                }
            }
        }
    }

    IEnumerator EndDodge()
    {
        yield return new WaitForSeconds(dodgeDuration);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        isDodging = false;
        if (anim.GetBool("rollCheck"))
        {
            anim.SetBool("rollCheck", false);
            _audio.PlayOneShot(_audio.Moonwalk);
        }
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
            {
                rb.AddForce(new Vector2(-4f, 1f), ForceMode2D.Impulse);
                hp = hp - 50;
            }
            else
            {
                rb.AddForce(new Vector2(4f, 1f), ForceMode2D.Impulse);
                hp = hp - 50;
            }
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

    // Increase Resource when enenmy is killed
    public void OnEnemyKilled(int amount)
    {
        AddResource(amount);
    }

    public void AddResource(int amount)
    {
        resource += amount;
        if (ResourceText != null)
        {
            GameObject resourceText = Instantiate(ResourceText, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            ResourceText textComponent = resourceText.GetComponent<ResourceText>();
            if (textComponent != null)
            {
                textComponent.SetResourceText(amount);
            }
        }
    }

    public bool IsAlive()
    {
        return alive;
    }

    IEnumerator TriggerSlowMotion(float slowMotionDuration, float slowMotionScale)
    {
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;  
        yield return new WaitForSecondsRealtime(slowMotionDuration); 
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;
    }

    public void PlayOneShotRunning()
    {
        _audio.PlayOneShot(_audio.Footstep);
    }

    public void PlayOneShotMoonWalk()
    {
        _audio.PlayOneShot(_audio.Moonwalk);
    }

}
