using UnityEngine;

public class ReticleController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Get the SpriteRenderer component to toggle visibility
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            // Activate the reticle and update its position
            spriteRenderer.enabled = true;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
}
