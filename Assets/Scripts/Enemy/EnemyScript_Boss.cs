using UnityEngine;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

public class EnemyScript_Boss : EnemyBase
{

    // Floating effect
    private float floatStrength = 0.1f;
    private float floatSpeed = 5f;
    private Vector3 initialPosition;

    // Boss shooting
    [SerializeField] GameObject turret_bullet;
    [SerializeField] Transform[] turrets;
    [SerializeField] Transform[] turret_firePoints;
    [SerializeField] LineRenderer[] lineRenderers;
    public float fireRate;
    private float lastFireTime = 0f;
    private int currentTurretIndex = 0;
    private bool isAiming = false;
    private bool isShooting = false;
    private bool isPatternRunning = false;

    // Enemy missile
    [SerializeField] Transform[] missile_firePoints;
    [SerializeField] GameObject missile;

    // Enemy Zap
    [SerializeField] Transform laserOrigin;
    [SerializeField] LineRenderer laserLine;
    [SerializeField] float laserLength = 50f;
    [SerializeField] private GameObject zapEffectPrefabSingle;
    private GameObject sweepEffect;

    // Enemy Zap2
    [SerializeField] LineRenderer[] crossLasers;
    private float[] baseAngles = { 0f, 120f, 240f };
    private float spinStartTime;
    private bool isSpinning = false;
    private float spinSpeed = 90f;
    [SerializeField] private GameObject zapEffectPrefab;
    private GameObject[] zapEffects;
    private EventInstance[] crossLaserBeamInstances;
    private EventInstance laserBeamInstance;

    // Enemy Summon
    [SerializeField] GameObject Bomber;
    [SerializeField] GameObject Wheel;
    [SerializeField] Transform[] SummonPoints;

    // Pattern Management
    private delegate IEnumerator Pattern();
    private Pattern[] patternList;

    private bool isPlayerNearby;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;

    private bool isFalling = false;
    private PolygonCollider2D polyCol;
    private BoxCollider2D boxCol;
    [SerializeField] GameObject explosion;

    private InputManager _input;

    
    void Start()
    {
        _audio = AudioManager.Instance;
        _input = InputManager.Instance;
        _input.OnMenuPressed += OnPaused;
        isalive = true;
        resourceAmount = 0;
        maxHealth = 10000;
        currentHealth = maxHealth;
        fireRate = 0.05f;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController != null)
        {
            player = playerController.GetAimPos();
        }
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;

        if (zapEffectPrefabSingle != null)
        {
            sweepEffect = Instantiate(zapEffectPrefabSingle, Vector3.zero, Quaternion.identity);
            sweepEffect.SetActive(false);
        }

        zapEffects = new GameObject[crossLasers.Length];
        if (zapEffectPrefab != null)
        {
            for (int i = 0; i < zapEffects.Length; i++)
            {
                zapEffects[i] = Instantiate(zapEffectPrefab, Vector3.zero, Quaternion.identity);
                zapEffects[i].SetActive(false);
            }
        }
        
        // Cross laser beam instance
        crossLaserBeamInstances = new EventInstance[3];
        for (int i = 0; i < 3; i++)
        {
            crossLaserBeamInstances[i] = _audio.GetEventInstance(_audio.CrossLaserBeam);
            crossLaserBeamInstances[i].set3DAttributes(RuntimeUtils.To3DAttributes(laserOrigin.position));
        }

        polyCol = GetComponent<PolygonCollider2D>();
        if (polyCol != null)
        {
            polyCol.enabled = false;
        }

        boxCol = GetComponent<BoxCollider2D>();
        if (boxCol != null)
        {
            boxCol.enabled = false;
        }

        patternList = new Pattern[] { Pattern1, Pattern2, Pattern3, Pattern4, Pattern5 };
        StartCoroutine(BossIntroMovement());
    }
    
    public override void OnPaused()
    {
        if (!MenuManager.IsPaused)
        {
            foreach (var cross in crossLaserBeamInstances)
            {
                cross.setPaused(true);
            }
            laserBeamInstance.setPaused(true);
        }
        else
        {
            foreach (var cross in crossLaserBeamInstances)
            {
                cross.setPaused(false);
            }
            laserBeamInstance.setPaused(false);
        }
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.OnMenuPressed -= OnPaused;
        }
        foreach (var cross in crossLaserBeamInstances)
        {
            if (cross.isValid())
            {
                cross.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                cross.release();
            }
        }

        if (laserBeamInstance.isValid())
        {
            laserBeamInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            laserBeamInstance.release();
        }
    }

    void Update()
    {
        if (isFalling || !isalive)
        {
            return;
        }

        isPlayerNearby = CheckNearbyPlayers();
        ApplyFloatingEffect();

        if (isalive && currentHealth <= maxHealth * 0.5f)
        {
            if (!isSpinning)
            {
                spinStartTime = Time.time;
                isSpinning = true;

                float elapsed = Time.time - spinStartTime;
                float rotationOffset = elapsed * spinSpeed;

                for (int i = 0; i < crossLasers.Length; i++)
                {
                    float angle = baseAngles[i] + rotationOffset;
                    Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;

                    RaycastHit2D hitTerrain = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Terrain"));
                    Vector3 endPoint = hitTerrain.collider != null ? hitTerrain.point : laserOrigin.position + dir * laserLength;
                    
                    crossLaserBeamInstances[i].set3DAttributes(RuntimeUtils.To3DAttributes(endPoint));
                    crossLaserBeamInstances[i].start();
                    crossLasers[i].enabled = true;
                }
            }
            RotateCrossLasers();
        }

        if (isPatternRunning && isalive)
        {
            if (isAiming)
            {
                Aim();
            }
            else
            {
                ResetTurretRotation();
            }

            if (isShooting)
            {
                Shoot();
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFalling && other.CompareTag("Terrain"))
        {
            Vector3 explosionPos = transform.position + new Vector3(0, -1f, 0);
            GameObject explosionInstance = Instantiate(explosion, explosionPos, Quaternion.identity);
            _audio.PlayOneShot(_audio.BossExplosion, transform.position);
            Destroy(explosionInstance.gameObject, 5f);
            BossSpawnTrigger trigger = FindAnyObjectByType<BossSpawnTrigger>();
            if (trigger != null)
            {
                trigger.OnBossDefeated();
            }
            CleanUp();
            base.Die(resourceAmount);

            RuntimeManager.GetBus("bus:/Music").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _audio.PlayOneShot(_audio.Lobby);
        }
    }

    private void CleanUp()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj == this.gameObject)
            {
                continue;
            }

            float distance = Vector3.Distance(player.position, enemyObj.transform.position);
            if (distance <= 50f)
            {
                EnemyBase enemy = enemyObj.GetComponent<EnemyBase>();
                if (enemy != null && enemy.isalive)
                {
                    enemy.TakeDamage(float.MaxValue);
                }
            }
        }
        foreach (var cross in crossLaserBeamInstances)
        {
            cross.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            cross.release();
        }
    }

    private IEnumerator BossIntroMovement()
    {
        _audio.PlayOneShot(_audio.Battle);
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0f, -5f, 0f);
        float duration = 4f;
        float time = 0f;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        initialPosition = endPos;
        yield return new WaitForSeconds(0.5f);
        if (polyCol != null)
        {
            polyCol.enabled = true;
        }
        StartCoroutine(ManagePatterns());
    }

    private IEnumerator ManagePatterns()
    {
        isPatternRunning = true;

        while (isalive)
        {
            int patternCount = patternList.Length;

            if (currentHealth <= maxHealth * 0.3f)
            {
                int indexA = Random.Range(0, patternCount);
                int indexB = Random.Range(0, patternCount);
                while (indexB == indexA) indexB = Random.Range(0, patternCount);
                yield return StartCoroutine(RunPatternsSimultaneously(patternList[indexA], patternList[indexB]));
            }
            else
            {
                int randomIndex = Random.Range(0, patternCount);
                // randomIndex = 2;
                yield return StartCoroutine(patternList[randomIndex]());
            }

            yield return new WaitForSeconds(1f);
        }
    }

    // Pattern 1: Aim and shoot laser at player
    private IEnumerator Pattern1()
    {
        isAiming = true;
        isShooting = false;
        yield return new WaitForSeconds(1f);

        isShooting = true;
        yield return new WaitForSeconds(4f);

        isAiming = false;
        isShooting = false;
    }

    // Pattern 2: Fire missiles from all fire points
    private IEnumerator Pattern2()
    {
        for (int i = 0; i < missile_firePoints.Length; i++)
        {
            Transform firePoint = missile_firePoints[i];
            _audio.PlayOneShot(_audio.MissileLaunch, firePoint.position);
            Instantiate(missile, firePoint.position, firePoint.rotation);
            yield return new WaitForSeconds(0.4f);
        }
    }

    // Pattern 3: Sweep laser in a pattern
    private IEnumerator Pattern3()
    {
        float startAngle, endAngle;
        if (Random.value < 0.5f)
        {
            startAngle = -90f;
            endAngle = 90f;
        }
        else
        {
            startAngle = 90f;
            endAngle = -90f;
        }

        float currentAngle = startAngle;
        float totalAngle = Mathf.Abs(endAngle - startAngle);
        float traveledAngle = 0f;

        float startSpeed = 50f;
        float endSpeed = 300f;

        bool hasHitPlayer = false;

        laserLine.enabled = true;

       
        Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * Vector3.down;
        RaycastHit2D hit = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Terrain"));
        Vector3 laserEnd = hit.collider != null ? hit.point : laserOrigin.position + dir * laserLength;
        laserBeamInstance = _audio.GetEventInstance(_audio.LaserBeam);
        laserBeamInstance.start();
        if (sweepEffect != null)
        {
            sweepEffect.SetActive(true);
            sweepEffect.transform.position = laserEnd;
        }
        laserBeamInstance.set3DAttributes(RuntimeUtils.To3DAttributes(laserEnd));
        while ((endAngle > startAngle && currentAngle < endAngle) ||
               (endAngle < startAngle && currentAngle > endAngle))
        {
            float t = traveledAngle / totalAngle;
            float currentSpeed = Mathf.Lerp(startSpeed, endSpeed, t);
            dir = Quaternion.Euler(0, 0, currentAngle) * Vector3.down;
            hit = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Terrain"));
            laserEnd = hit.collider != null ? hit.point : laserOrigin.position + dir * laserLength;

            laserLine.SetPosition(0, laserOrigin.position);
            laserLine.SetPosition(1, laserEnd);

            // Update audio position to follow the laser end point
            laserBeamInstance.set3DAttributes(RuntimeUtils.To3DAttributes(laserEnd));
            sweepEffect.transform.position = laserEnd;

            if (!hasHitPlayer)
            {
                RaycastHit2D hitPlayer = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Hitbox_Player"));
                if (hitPlayer.collider != null)
                {
                    PlayerController playerController = hitPlayer.collider.GetComponentInParent<PlayerController>();
                    if (playerController != null && !playerController.GetPlayerInvincible() && playerController.IsAlive())
                    {
                        playerController.Hurt(10);
                        hasHitPlayer = true;
                    }
                }
            }
            float step = currentSpeed * Time.deltaTime;
            traveledAngle += step;
            currentAngle = Mathf.MoveTowards(currentAngle, endAngle, step);

            yield return null;
        }

        laserBeamInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        laserBeamInstance.release();
        sweepEffect.SetActive(false);
        laserLine.enabled = false;
    }

    // Pattern 4: Summon Bombers in a pattern
    private IEnumerator Pattern4()
    {
        for (int i = 0; i < SummonPoints.Length; i++)
        {
            Transform summonPoint = SummonPoints[i];
            Instantiate(Bomber, summonPoint.position, summonPoint.rotation);
            yield return new WaitForSeconds(0.75f);
        }
    }

    // Pattern 5: Summon Wheels in a pattern
    private IEnumerator Pattern5()
    {
        for (int i = 0; i < SummonPoints.Length; i++)
        {
            Transform summonPoint = SummonPoints[i];
            Instantiate(Wheel, summonPoint.position, summonPoint.rotation);
            yield return new WaitForSeconds(0.5f);
            Instantiate(Wheel, summonPoint.position, summonPoint.rotation);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void RotateCrossLasers()
    {
        float elapsed = Time.time - spinStartTime;
        float rotationOffset = elapsed * spinSpeed;

        for (int i = 0; i < crossLasers.Length; i++)
        {
            float angle = baseAngles[i] + rotationOffset;
            Vector3 dir = Quaternion.Euler(0, 0, angle) * Vector3.up;

            RaycastHit2D hitTerrain = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Terrain"));
            Vector3 endPoint = hitTerrain.collider != null ? hitTerrain.point : laserOrigin.position + dir * laserLength;
            crossLaserBeamInstances[i].set3DAttributes(RuntimeUtils.To3DAttributes(endPoint));
            crossLasers[i].SetPosition(0, laserOrigin.position);
            crossLasers[i].SetPosition(1, endPoint);
            
            if (zapEffects[i] != null)
            {
                zapEffects[i].SetActive(true);
                zapEffects[i].transform.position = endPoint;
            }

            RaycastHit2D hitPlayer = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Hitbox_Player"));
            if (hitPlayer.collider != null)
            {
                PlayerController playerController = hitPlayer.collider.GetComponentInParent<PlayerController>();
                if (playerController != null && playerController.IsAlive() && !playerController.GetPlayerInvincible())
                {
                    playerController.Hurt(1f);
                }
            }
        }
    }

    private IEnumerator RunPatternsSimultaneously(Pattern a, Pattern b)
    {
        Coroutine routineA = StartCoroutine(a());
        Coroutine routineB = StartCoroutine(b());
        yield return routineA;
        yield return routineB;
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
        return Vector3.Distance(transform.position, player.position) <= 30f;
    }

    private void Aim()
    {
        if (isPlayerNearby)
        {
            for (int i = 0; i < turrets.Length; i++)
            {
                if (lineRenderers.Length > i)
                {
                    lineRenderers[i].enabled = true;
                    lineRenderers[i].SetPosition(0, turret_firePoints[i].position);
                    lineRenderers[i].SetPosition(1, player.position);
                }

                Vector3 direction = (player.position - turrets[i].position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                turrets[i].rotation = Quaternion.Euler(0, 0, angle + 90);
                turret_firePoints[i].rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else
        {
            foreach (var l in lineRenderers)
            {
                l.enabled = false;
            }
        }
    }

    private void Shoot()
    {
        if (!isPlayerNearby || Time.time < lastFireTime + fireRate)
        {
            return;
        }

        lastFireTime = Time.time;

        if (turret_bullet != null && turret_firePoints.Length > 0)
        {
            Transform firePoint = turret_firePoints[currentTurretIndex];

            _audio.PlayOneShot(_audio.Laser, firePoint.position);
            Instantiate(turret_bullet, firePoint.position, firePoint.rotation);

            currentTurretIndex++;
            if (currentTurretIndex >= turret_firePoints.Length)
            {
                currentTurretIndex = 0;
            }
        }
    }

    private void ResetTurretRotation()
    {
        for (int i = 0; i < turrets.Length; i++)
        {
            if (turrets[i] != null)
            {
                turrets[i].rotation = Quaternion.Euler(0, 0, 0);
            }
            if (turret_firePoints.Length > i && turret_firePoints[i] != null)
            {
                turret_firePoints[i].rotation = Quaternion.Euler(0, 0, 0);
            }
            if (lineRenderers.Length > i && lineRenderers[i] != null)
            {
                lineRenderers[i].enabled = false;
            }
        }
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
        if (!isFalling)
        {
            isFalling = true;
            isalive = false;

            if (polyCol != null)
            {
                polyCol.enabled = false;
            }
            if (boxCol != null)
            {
                boxCol.enabled = true;
            }

            // Disable laser line
            if (laserLine != null)
            {
                laserLine.enabled = false;
            }
            // Disable sweep effect
            if (sweepEffect != null)
            {
                sweepEffect.SetActive(false);
            }

            // Disable zap effect
            foreach (var effect in zapEffects)
            {
                if (effect != null)
                {
                    effect.SetActive(false);
                }
            }

            // Disable rotating laser lines
            foreach (var cross in crossLasers)
            {
                if (cross != null)
                {
                    cross.enabled = false;
                }
            }

            // Disable aim line
            foreach (var l in lineRenderers)
            {
                if (l != null)
                {
                    l.enabled = false;
                }
            }
            rb.gravityScale = 2f;
        }
    }
}
