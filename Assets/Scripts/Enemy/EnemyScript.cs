using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Maximum health of the enemy
    public int maxHealth = 100;
    // Current health of the enemy
    private int currentHealth;
    // Player Controller script of the player
    public PlayerController playerController;

    void Start()
    {
        // Set current health of the enemy to the maximum health
        currentHealth = maxHealth;
        // Gets playercontroller component
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Function to deal damage to the enemy
    public void TakeDamage(float damage)
    {
        // Subtract damage from current health
        currentHealth -= Mathf.RoundToInt(damage);

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Function that handles the enemy death
    private void Die()
    {
        // Notify player about the enemy kill for EXP
        if (playerController != null)
        {
            playerController.OnEnemyKilled(160); // Grant EXP to the player
        }
        Destroy(gameObject);  // Destroy the enemy
    }
}
