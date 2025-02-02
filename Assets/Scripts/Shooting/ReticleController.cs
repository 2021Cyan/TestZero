using UnityEngine;

public class ReticleController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Shooting shootingScript;           // Reference to the Shooting script
    [SerializeField] private float baseSize = 0.25f;            // Base size of the reticle
    [SerializeField] private float sizeMultiplier = 0.125f;     // Multiplier for spread
    private InputManager _Input;
    void Start()
    {
        // Get the SpriteRenderer component to toggle visibility
        spriteRenderer = GetComponent<SpriteRenderer>();
        _Input = InputManager.Instance;
    }

    void LateUpdate()
    {
        if (_Input.AimInput)
        {
            // Activate the reticle and update its position
            spriteRenderer.enabled = true;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(_Input.MouseInput);
            transform.position = mousePos;

            // Scale the reticle based on the current spread angle
            float spreadSize = baseSize + (shootingScript.GetCurrentSpread() * sizeMultiplier);
            transform.localScale = new Vector3(spreadSize, spreadSize, 1f);
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
}
