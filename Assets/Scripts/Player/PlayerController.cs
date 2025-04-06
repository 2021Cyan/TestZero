using System.Collections;
using FMODUnity;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using EasyTransition;

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
    public int currentLevel;
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
    private CinemachineCamera nearCinemachineCamera;
    public CinemachineCamera farCinemachineCamera;
    public CinemachineCamera lavaCinemachineCamera;
    private Vector3 mousePos;
    [SerializeField] ParticleSystem sparkFootEffectPrefab;
    private ParticleSystem sparkFootEffect;
    private ParticleSystem healingEffect;

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

    // Singleton Instance
    public static PlayerController Instance;
    private InputManager _input;
    private AudioManager _audio;
    [SerializeField] Transform center;
    public TransitionSettings transition;
    public float startDelay;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindSceneReferences();
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void FindSceneReferences()
    {
        SetCursorVisible(false);
        _input = InputManager.Instance;
        _audio = AudioManager.Instance;
        hp = max_hp;
        currentAmmo = maxAmmo;
        currentLevel = 1;
        maincam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        nearCinemachineCamera = GameObject.FindGameObjectWithTag("NearCinemachineCamera").GetComponent<CinemachineCamera>();
        farCinemachineCamera = GameObject.FindGameObjectWithTag("FarCinemachineCamera").GetComponent<CinemachineCamera>();
        lavaCinemachineCamera = GameObject.FindGameObjectWithTag("LavaCC").GetComponent<CinemachineCamera>();
        lavaCinemachineCamera.Follow = transform;
        farCinemachineCamera.Follow = transform;

        GameObject tempSpark = GameObject.FindGameObjectWithTag("SparkSlide");
        if (tempSpark != null)
        {
            if (tempSpark.transform.childCount > 0)
            {
                Transform tempChild = tempSpark.transform.GetChild(0);
                Destroy(tempChild.gameObject);
            }
            sparkFootEffect = Instantiate(sparkFootEffectPrefab, tempSpark.transform);
            sparkFootEffect.transform.Rotate(0, -90, 0);
            sparkFootEffect.transform.SetParent(tempSpark.transform);
        }

        GameObject healingParticle = GameObject.FindGameObjectWithTag("HealingParticle");
        if (healingParticle != null)
        {
            healingEffect = healingParticle.GetComponent<ParticleSystem>();
        }


        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        _audio.PlayOneShot(_audio.Lobby);
        InputManager.Input.Enable();

        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            nearCinemachineCamera.Priority = -2;
            Cursor.visible = false;
            _input.EnableInput();
            FinishIntroAinm();
        }
    }

    private void Update()
    {
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

        if (IsGrounded())
        {
            rb.gravityScale = 1f;
            currentJumpTime = 0f;
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
        }
    }

    bool IsGrounded()
    {
        float rayLength = 0.5f;
        float raySpacing = 0.3f;
        Vector2 position = transform.position;

        Vector2 center = position;
        Vector2 left = position + Vector2.left * raySpacing;
        Vector2 right = position + Vector2.right * raySpacing;

        LayerMask groundMask = LayerMask.GetMask("Terrain");

        RaycastHit2D centerRay = Physics2D.Raycast(center, Vector2.down, rayLength, groundMask);
        RaycastHit2D leftRay = Physics2D.Raycast(left, Vector2.down, rayLength, groundMask);
        RaycastHit2D rightRay = Physics2D.Raycast(right, Vector2.down, rayLength, groundMask);

        bool isGround = false;

        if (centerRay.collider != null)
        {
            if (centerRay.collider.CompareTag("Terrain") && centerRay.normal.y > 0.5f)
            {
                isGround = true;
            }
        }

        if (leftRay.collider != null)
        {
            if (leftRay.collider.CompareTag("Terrain") && leftRay.normal.y > 0.5f)
            {
                isGround = true;
            }
        }

        if (rightRay.collider != null)
        {
            if (rightRay.collider.CompareTag("Terrain") && rightRay.normal.y > 0.5f)
            {
                isGround = true;
            }
        }
        return isGround;
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


        if (_input.MoveInput.x < 0)
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
        if (_input.MoveInput.x > 0)
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
        if (isDodging || rb.linearVelocity.y != 0)
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
                Vector3 temp = transform.position;
                temp.z = -1;
                sparkFootEffect.transform.position = temp;
                sparkFootEffect.Play();
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
        sparkFootEffect.Stop();
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

    IEnumerator ActivateInvincibility()
    {
        isInvincible = true;

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        float blinkDuration = 0.05f;
        float totalDuration = 0.4f;
        float timer = 0f;

        while (timer < totalDuration)
        {
            foreach (var sr in sprites)
            {
                sr.color = new Color(1f, 1f, 1f, 0.3f);
            }

            yield return new WaitForSeconds(blinkDuration);

            foreach (var sr in sprites)
            {
                sr.color = new Color(1f, 1f, 1f, 1f);
            }

            yield return new WaitForSeconds(blinkDuration);
            timer += blinkDuration * 2f;
        }

        foreach (var sr in sprites)
        {
            sr.color = new Color(1f, 1f, 1f, 1f);
        }

        isInvincible = false;
    }

    public void Hurt(float amount)
    {
        if (!isInvincible)
        {
            PlayerUI.Instance.ShowHurtEffect();
            _audio.PlayOneShot(_audio.Hurt);
            hp = hp - amount;
            StartCoroutine(ActivateInvincibility());
        }
    }

    public void Restore(float amount)
    {
        if (healingEffect != null && amount > 0) healingEffect.Play();
        if (amount > 5f) _audio.PlayOneShot(_audio.Heal);
        hp = Mathf.Min(hp + amount, max_hp);
    }

    void Die()
    {
        if (hp <= 0)
        {
            _audio.PlayOneShot(_audio.Death);
            anim.SetTrigger("die");
            alive = false;
            StartCoroutine(RestartAfterDelay(3f));
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
        TransitionManager.Instance().Transition("MainMenu", transition, startDelay);
    }


    private void Restart()
    {
        if (_input.ResetInput)
        {
            if (PodScript.Instance != null)
            {
                Destroy(PodScript.Instance.gameObject);
                PodScript.Instance = null;
            }
            Destroy(Instance.gameObject);
            Instance = null;
            RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _input.SetResetInput(false);
            TransitionManager.Instance().Transition("MainMenu", transition, startDelay);
        }
    }

    public void RestartGame()
    {
        _input.SetResetInput(true);
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
        _audio.PlayOneShot(_audio.Reload);
        Shooting shooting = GetComponent<Shooting>();
        if (shooting != null)
        {
            shooting.ForceReloadComplete();
        }

        // Global slow-motion (Enemy, Bullet...etc)
        Time.timeScale = enemyTimeScale;
        Time.fixedDeltaTime = 0.02f * enemyTimeScale;

        // Adjust player animation speed
        anim.speed = 0.9f;
        float originalFireRate = fireRate;
        float originalReloadSpeed = reloadSpeed;
        fireRate *= 2f;
        reloadSpeed /= 4f;


        float depletionRate = bulletTimeMaxGauge / bulletTimeDuration;
        while (bulletTimeGauge > 0)
        {
            if (Time.timeScale > 0f)
            {
                bulletTimeGauge -= depletionRate * Time.unscaledDeltaTime;
                bulletTimeGauge = Mathf.Max(0, bulletTimeGauge);
            }
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

    public void SetCursorVisible(bool visible)
    {
        Cursor.visible = visible;
    }

    public void FinishIntroAinm()
    {
        anim.SetTrigger("FinishIntro");
    }

    public void UseFarCamera()
    {
        nearCinemachineCamera.Priority = -2;
    }

    public void EnableInput()
    {
        _input.EnableInput();
    }
}
