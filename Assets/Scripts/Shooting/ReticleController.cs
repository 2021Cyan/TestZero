using UnityEngine;

public class ReticleController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Shooting shootingScript;           // Reference to the Shooting script
    [SerializeField] private float baseSize = 0.25f;            // Base size of the reticle
    [SerializeField] private float sizeMultiplier = 0.125f;     // Multiplier for spread
    private InputManager _input;

    // Tracker references
    public GameObject trackerPrefab;
    private GameObject trackerInstance;
    public GameObject reload;
    public GameObject spin;
    string reticlePath_original = $"Sprites/Reticles/Crosshair_08";
    string reticlePath = $"Sprites/Reticles/Crosshair_17";

    void Start()
    {
        // Get the SpriteRenderer component to toggle visibility
        if(reload != null && spin != null)
        {
            reload.SetActive(false);
            spin.SetActive(false);
        }
        shootingScript = PlayerController.Instance.GetComponent<Shooting>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _input = InputManager.Instance;
    }

    void LateUpdate()
    {
        if (!PlayerController.Instance || !PlayerController.Instance.IsAlive() || shootingScript == null)
        {
            DestroyTracker();
            spriteRenderer.enabled = false;
            return;
        }

        spriteRenderer.enabled = true;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePos;

        if (PlayerController.Instance.bulletType == 10)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(reticlePath);
            transform.localScale = new Vector3(1f, 1f, 1f);
            GameObject target = FindEnemyClosestToMouse();

            if (target != null)
            {
                Enemy_Soldier es = target.GetComponent<Enemy_Soldier>();

                if(es != null)
                {
                    if (trackerInstance == null)
                    {
                        trackerInstance = Instantiate(trackerPrefab, es.getAimPos().position, Quaternion.identity);
                    }
                    else
                    {
                        trackerInstance.transform.position = es.getAimPos().position;
                    }
                }
                else
                {
                    if (trackerInstance == null)
                    {
                        trackerInstance = Instantiate(trackerPrefab, target.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        trackerInstance.transform.position = target.transform.position;
                    }
                }
            }
            else
            {
                DestroyTracker();
            }
        }
        else
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(reticlePath_original);
            DestroyTracker();
        }

        if (PlayerController.Instance.bulletType != 10)
        {
            float spreadSize = baseSize + (shootingScript.GetCurrentSpread() * sizeMultiplier);
            transform.localScale = new Vector3(spreadSize, spreadSize, 1f);
        }

        if (reload != null && spin != null)
        {
            float spreadSize = baseSize + (shootingScript.GetCurrentSpread() * sizeMultiplier);
            if (PlayerController.Instance.bulletType != 10)
            {
                reload.transform.localScale = new Vector3(1 / spreadSize, 1 / spreadSize, 1);
                spin.transform.localScale = new Vector3(1 / spreadSize, 1 / spreadSize, 1);
            }
            else
            {
                reload.transform.localScale = new Vector3(1f, 1f, 1f);
                spin.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            spin.transform.Rotate(0, 0, 300f * Time.deltaTime);
        }
    }

    private GameObject FindEnemyClosestToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(_input.MouseInput);
        mousePos.z = 0f;

        float searchWidth = 5f;   
        float searchHeight = 5f;  

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            EnemyBase eb = enemy.GetComponent<EnemyBase>();

            if (eb == null || !eb.isalive)
            {
                continue;
            }

            Vector3 enemyPos = enemy.transform.position;
            if (enemyPos.x >= mousePos.x - searchWidth / 2 && enemyPos.x <= mousePos.x + searchWidth / 2 &&
                enemyPos.y >= mousePos.y - searchHeight / 2 && enemyPos.y <= mousePos.y + searchHeight / 2)
            {
                float distance = Vector3.Distance(enemyPos, mousePos);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private void DestroyTracker()
    {
        if (trackerInstance != null)
        {
            Destroy(trackerInstance);
            trackerInstance = null;
        }
    }

    public void SetReloadUI(bool state)
    {
        if (reload != null && spin != null)
        {
            reload.SetActive(state);
            spin.SetActive(state);
        }
    }
}
