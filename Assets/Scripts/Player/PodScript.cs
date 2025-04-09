using UnityEngine;

public class PodScript : MonoBehaviour
{
    public GameObject player;
    private PlayerController playerController;

    // Pod movement
    public float followSpeed = 8f;
    public Vector3 offset = new Vector3(-1f, 2.0f, 0f);
    public float swayAmount = 0.2f;
    public float swaySpeed = 3f;

    // Pod behaviour
    public int weaponlevel;
    public int heallevel;
    private float detectionrange = 10f;
    private Vector3 velocity = Vector3.zero;
    public static PodScript Instance;

    // Pod turret
    [SerializeField] Transform turret;
    [SerializeField] Transform turret_firePoint;
    [SerializeField] GameObject turret_bullet;
    [SerializeField] GameObject turret_missle;
    [SerializeField] GameObject cursor;
    private GameObject trackedEnemy;
    private float rotationSpeed = 180f;
    public float fireRate = 1f;
    private float lastFireTime = 0;
    public float missileCooldown = 5f;
    private float lastMissileTime = 0f;
    private AudioManager _audio;

    // Pod heal
    public float healCooldown = 15f;
    private float healAmount = 10f; 
    private float lastHealTime = 0f;

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
        _audio = AudioManager.Instance;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<PlayerController>();
            weaponlevel = 0;
            heallevel = 0;
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        if(weaponlevel >= 1)
        {
            fireRate = (weaponlevel * 3);
        }

        if (heallevel >= 1)
        {
            healAmount = 5 + (heallevel);
            healCooldown = 5;
        }

        if (weaponlevel >= 3)
        {
            missileCooldown =  5f - (0.25f * (weaponlevel - 3));
        }

        FollowPlayer();
        Scout();
        Aim();
        Shoot();
        ShootMissile();
        Heal();
        Shield();
    }

    private void FollowPlayer()
    {
        Vector3 targetPosition = playerController.GetAimPos().position + offset;
        float swayOffset = Mathf.Sin(Time.time * swaySpeed) * swayAmount;
        targetPosition.y += swayOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
    }

    private void Scout()
    {
        trackedEnemy = FindClosestEnemy();
        if (trackedEnemy != null)
        {
            cursor.SetActive(true);
            Enemy_Soldier es = trackedEnemy.GetComponent<Enemy_Soldier>();
            Vector3 directionToEnemy;
            if (es != null)
            {
                directionToEnemy = (es.getAimPos().position - transform.position).normalized;
            }
            else
            {
                directionToEnemy = (trackedEnemy.transform.position - transform.position).normalized;
            }

            float targetAngle = Mathf.Atan2(directionToEnemy.y, directionToEnemy.x) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.LerpAngle(cursor.transform.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
            cursor.transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
            float cursorDistance = 1f; 
            Vector3 cursorOffset = new Vector3(Mathf.Cos(Mathf.Deg2Rad * smoothAngle), Mathf.Sin(Mathf.Deg2Rad * smoothAngle), 0f) * cursorDistance;
            cursor.transform.position = transform.position + cursorOffset;
        }
        else
        {
            cursor.SetActive(false);
        }
    }
    private void Aim()
    {
        if (weaponlevel >= 0 && trackedEnemy != null)
        {
            Enemy_Soldier es = trackedEnemy.GetComponent<Enemy_Soldier>();
            Vector3 direction;
            if (es != null)
            {
                direction = (es.getAimPos().position - turret.position).normalized;
            }
            else
            {
                direction = (trackedEnemy.transform.position - turret.position).normalized;
            }
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(0, 0, angle + 45);
        }
    }

    private void Shoot()
    {
        if (weaponlevel >= 1 && trackedEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, trackedEnemy.transform.position);
            Enemy_Soldier es = trackedEnemy.GetComponent<Enemy_Soldier>();
            bool isPathClear = false;
            if (es != null)
            {
                isPathClear = !Physics2D.Linecast(turret_firePoint.position, es.getAimPos().position, LayerMask.GetMask("Terrain"));
            }
            else
            {
                isPathClear = !Physics2D.Linecast(turret_firePoint.position, trackedEnemy.transform.position, LayerMask.GetMask("Terrain"));
            }
            if (distanceToEnemy <= detectionrange && Time.time > lastFireTime + (1f / fireRate) && isPathClear)
            {
                lastFireTime = Time.time;
                Quaternion fireRotation = Quaternion.Euler(0, 0, turret.rotation.eulerAngles.z - 45);
                Instantiate(turret_bullet, turret_firePoint.position, fireRotation);
                _audio.PlayOneShot(_audio.Laser, transform.position);
            }
        }
    }

    private void ShootMissile()
    {
        if (weaponlevel >= 3 && trackedEnemy != null)
        {
            if(Time.time > lastMissileTime + missileCooldown)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, trackedEnemy.transform.position);
                if (distanceToEnemy <= detectionrange)
                {
                    bool isPathClear = !Physics2D.Linecast(turret_firePoint.position, trackedEnemy.transform.position, LayerMask.GetMask("Terrain"));
                    if (isPathClear)
                    {
                        lastMissileTime = Time.time;
                        _audio.PlayOneShot(_audio.Missile, turret_firePoint.position);
                        Instantiate(turret_missle, turret_firePoint.position, turret_firePoint.rotation);
                        
                    }
                }
            }
        }
    }

    private void Heal()
    {
        if (heallevel >= 1)
        {

            if (Time.time > lastHealTime + healCooldown && playerController.IsAlive())
            {
                lastHealTime = Time.time;
                playerController.Restore(healAmount);
            }
        }
    }

    private void Shield()
    {
        if (heallevel >= 3)
        {

        }
    }

    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestVisibleEnemy = null;
        GameObject closestEnemy = null;
        float minVisibleDistance = Mathf.Infinity;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            EnemyBase eb = enemy.GetComponent<EnemyBase>();

            if (eb == null || !eb.isalive)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (!Physics2D.Linecast(transform.position, enemy.transform.position, LayerMask.GetMask("Terrain")))
            {
                if (distance < minVisibleDistance)
                {
                    minVisibleDistance = distance;
                    closestVisibleEnemy = enemy;
                }
            }

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }
        if (closestVisibleEnemy != null)
        {
            return closestVisibleEnemy;
        }
        return closestEnemy;
    }

}

