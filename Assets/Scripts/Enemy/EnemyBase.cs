using UnityEngine;
public abstract class EnemyBase : MonoBehaviour
{
    // Maximum health of the enemy
    public int maxHealth;
    // Current health of the enemy
    public int currentHealth;
    // Resource rewarded when enemy is killed
    public int resourceAmount;

    // Player Controller script of the player
    public PlayerController playerController;
    public bool isalive;

    public virtual void TakeDamage(float damage)
    {
        // Subtract damage from current health
        currentHealth -= Mathf.RoundToInt(damage);

        // Check if the enemy is dead
        if (currentHealth <= 0)
        {
            isalive = false;
            Die(resourceAmount);
        }
    }

    protected virtual void Die(int amount)
    {
        // Notify player about the enemy kill for EXP
        if (playerController != null)
        {
            if (amount > 0) { 
                playerController.OnEnemyKilled(amount); 
            }
        }
        Destroy(gameObject);  // Destroy the enemy
    }
}