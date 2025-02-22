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
    string reticlePath = $"Sprites/Reticles/Crosshair_17";

    void Start()
    {
        // Get the SpriteRenderer component to toggle visibility
        spriteRenderer = GetComponent<SpriteRenderer>();
        _input = InputManager.Instance;
    }

    void LateUpdate()
    {
        if (!PlayerController.Instance.IsAlive())
        {
            return;
        }

        if (_input.AimInput)
        {
            if (PlayerController.Instance.bulletType == 10)
            {
                spriteRenderer.sprite = Resources.Load<Sprite>(reticlePath);
                transform.localScale = new Vector3(1f, 1f, 1f);

                GameObject target = FindEnemyClosestToMouse();

                if (target != null) {
                    if (trackerInstance == null)
                    {
                        trackerInstance = Instantiate(trackerPrefab, target.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        // Update tracker position if it already exists
                        trackerInstance.transform.position = target.transform.position;
                    }
                }
                else
                {
                    // Remove tracker if there are no enemies nearby
                    if (trackerInstance != null)
                    {
                        Destroy(trackerInstance);
                        trackerInstance = null;
                    }
                }
            }

            // Activate the reticle and update its position
            spriteRenderer.enabled = true;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;

            // Scale the reticle based on the current spread angle
            if (PlayerController.Instance.bulletType != 10)
            {
                float spreadSize = baseSize + (shootingScript.GetCurrentSpread() * sizeMultiplier);
                transform.localScale = new Vector3(spreadSize, spreadSize, 1f);
            }
        }
        else
        {
            if (trackerInstance != null)
            {
                Destroy(trackerInstance);
                trackerInstance = null;
            }
            spriteRenderer.enabled = false;
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
}
