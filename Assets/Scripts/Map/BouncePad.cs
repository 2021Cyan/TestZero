using UnityEngine;

public class BouncePad : MonoBehaviour
{
    public float bounceForce = 10f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0); // Reset Y velocity to prevent stacking
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse); // Apply an upward force
        }
    }
}
