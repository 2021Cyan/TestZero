using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool isDodging = false;
    public float dodgeDuration = 0.4f;
    public float dodgeCooldown = 1f;
    private float lastDodgeTime = -1000f;
    private bool isInvincible = false;

    // Bullettime variables
    public float bulletTimeGauge = 0f;
    public float bulletTimeMaxGauge = 100f;
    public float bulletTimeFillRate = 20f; 
    public float bulletTimeDuration = 5f; 
    public bool isBulletTimeActive = false;
    [SerializeField] private float enemyTimeScale = 0.5f;      
    [SerializeField] private float playerTimeMultiplier = 0.75f; 

    // Singleton Instance
    public static PlayerController Instance;
    private InputManager _input;
    private AudioManager _audio;
    [SerializeField] Transform center;

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
        Cursor.visible = false;
        _input = InputManager.Instance;
        _audio = AudioManager.Instance;
        hp = max_hp;
        currentAmmo = maxAmmo;
        maincam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        _audio.PlayOneShot(_audio.Lobby);
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(_input.MouseInput);
        AdjustGravity();
        Restart();
        if (alive)
        {
            mousePos = maincam.ScreenToWorldPoint(_input.MouseInput);
            Dodge();
            Die();
            Jump();
            Move();
            BulletTime();
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
            currentJumpTime = 0f; 
        }
    }

    void Move()
    {
        if (isDodging)
            return;

        Vector3 moveVelocity = Vector3.zero;
        anim.SetBool("isRun", false);
        anim.SetBool("isWalkBack", false);

        // Adjust speed while bullettime
        float speedMultiplier;

        if (isBulletTimeActive)
        {
            speedMultiplier = 2f;
        }
        else
        {
            speedMultiplier = 1f;
        }

        float delta = Time.deltaTime * speedMultiplier;


        if (Input.GetAxisRaw("Horizontal") < 0)
        {
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
            transform.position += moveVelocity * walkPower * delta;
        }
        else
        {
            transform.position += moveVelocity * movePower * delta;
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
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

        if (_input.DodgeInput)
        {
            lastDodgeTime = Time.time;
            isDodging = true;
            StartCoroutine(ActivateDodgeInvincibility());

            rb.linearVelocity = Vector2.zero;
            Vector2 dodgeDir = new Vector2(direction, 0);
            float totalDodgeDistance = dodgePower;

            if (anim.GetBool("isRun"))
            {
                anim.SetTrigger("slide");
                _audio.PlayOneShot(_audio.Dodge);
                rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
            }
            else if (anim.GetBool("isWalkBack"))
            {
                dodgeDir = new Vector2(-direction, 0);
                totalDodgeDistance = dodgePower / 2f;
                anim.SetBool("rollCheck", true);
                anim.SetTrigger("roll");
                _audio.PlayOneShot(_audio.JumpGroan);
                rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
            }
            else if (anim.GetBool("isJump"))
            {
                if (Input.GetAxisRaw("Horizontal") < 0)
                {
                    if (mousePos.x < transform.position.x)
                    {
                        dodgeDir = new Vector2(direction, 0).normalized;
                        totalDodgeDistance = dodgePower * 1.2f;
                        anim.SetTrigger("airdash");
                        _audio.PlayOneShot(_audio.AirDash);
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                    }
                    else
                    {
                        dodgeDir = new Vector2(-direction, 0).normalized;
                        totalDodgeDistance = dodgePower;
                        anim.SetTrigger("airdash_back");
                        _audio.PlayOneShot(_audio.AirDash);
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
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
                    }
                    else
                    {
                        dodgeDir = new Vector2(direction, 0).normalized;
                        totalDodgeDistance = dodgePower * 1.2f;
                        anim.SetTrigger("airdash");
                        _audio.PlayOneShot(_audio.AirDash);
                        rb.AddForce(dodgeDir * totalDodgeDistance, ForceMode2D.Impulse);
                    }
                }
            }
            StartCoroutine(EndDodge());
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

    IEnumerator ActivateDodgeInvincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(dodgeDuration); 
        isInvincible = false;
    }

    public void Hurt(float amount)
    {
        if (!isInvincible)
        {
            PlayerUI.Instance.ShowHurtEffect();
            _audio.PlayOneShot(_audio.Hurt);
            if (mousePos.x > transform.position.x)
            {
                hp = hp - amount;
            }
            else
            {
                hp = hp - amount;
            }
        }
    }

    public void Restore(float amount)
    {
        if (PlayerUI.Instance != null)
        {
            PlayerUI.Instance.ShowRestoreEffect();
        }
        hp = Mathf.Min(hp + amount, max_hp);
    }

    void Die()
    {
        if (hp <= 0)
        {
            _audio.PlayOneShot(_audio.Death);
            anim.SetTrigger("die");
            alive = false;
            StartCoroutine(RestartAfterDelay(5f));
        }
    }

    IEnumerator RestartAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (PodScript.Instance != null)
        {
            Destroy(PodScript.Instance.gameObject);
            PodScript.Instance = null;
        }
        Destroy(Instance.gameObject);
        Instance = null;
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    void Restart()
    {
        if(_input.ResetInput)
        {
            if (PodScript.Instance != null)
            {
                Destroy(PodScript.Instance.gameObject);
                PodScript.Instance = null;
            }
            Destroy(Instance.gameObject);
            Instance = null;
            RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    private void BulletTime()
    {
        // Recharge Energy when bullet time is not activated
        if (!isBulletTimeActive)
        {
            bulletTimeGauge += Time.deltaTime * bulletTimeFillRate;
            bulletTimeGauge = Mathf.Min(bulletTimeGauge, bulletTimeMaxGauge);
        }

        // Press Q to activate bullet time
        if (_input.BulletTimeInput && bulletTimeGauge >= bulletTimeMaxGauge && !isBulletTimeActive)
        {
            StartCoroutine(ActivateBulletTime());
        }
    }

    IEnumerator ActivateBulletTime()
    {
        _audio.SetPitch(0.5f);
        isBulletTimeActive = true;

        // Global slow-motion (Enemy, Bullet...etc)
        Time.timeScale = enemyTimeScale;
        Time.fixedDeltaTime = 0.02f * enemyTimeScale;

        // Adjust player animation speed
        anim.speed = 0.9f;
        float originalFireRate = fireRate;
        float originalReloadSpeed = reloadSpeed;
        fireRate *= 2f;
        reloadSpeed /= 2f;


        float depletionRate = bulletTimeMaxGauge / bulletTimeDuration;
        while (bulletTimeGauge > 0)
        {
            bulletTimeGauge -= depletionRate * Time.unscaledDeltaTime;
            bulletTimeGauge = Mathf.Max(0, bulletTimeGauge);
            yield return null;
        }

        _audio.SetPitch(1.0f);
        // Restore speed setting
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        bulletTimeGauge = 0f;
        isBulletTimeActive = false;

        // Restore player animation speed
        anim.speed = 1f;
        fireRate = originalFireRate;
        reloadSpeed = originalReloadSpeed;
    }

    public Transform GetAimPos()
    {
        return center.transform;
    }

    public bool GetPlayerInvincible()
    {
        return isInvincible;
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
