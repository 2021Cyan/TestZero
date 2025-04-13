using UnityEngine;
using System.Collections;
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

    // For corrosive bullet type
    private Coroutine corrosiveCoroutine;
    private float corrosiveTimer = 0f;
    private float corrosiveDamagePerSecond = 0f;
    public GameObject damageTextPrefab;
    private bool isCorroding = false;
    protected int _levelNumber = 0;

    // For Combo bullet 
    private Coroutine comboCoroutine;
    private float comboDamage = 0f;

    public void ZeroResourceAmount()
    {
        resourceAmount = 0;
    }

    public void SetLevelNumber(int levelNumber)
    {
        _levelNumber = levelNumber;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public void Smite()
    {
        resourceAmount = 0;
        TakeDamage(float.MaxValue);
    }

    public virtual void TakeDamage(float damage)
    {
        float bonus = comboDamage;
        float totalDamage = damage + bonus;

        // Subtract damage from current health
        currentHealth -= Mathf.RoundToInt(totalDamage);

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

    public void ApplyCorrosiveEffect(float damagePerSecond, float duration)
    {
        if (!isalive)
        {
            return; 
        }

        if (isCorroding)
        {
            return;
        }
        if (corrosiveCoroutine != null)
        {
            StopCoroutine(corrosiveCoroutine);
        }

        corrosiveDamagePerSecond = damagePerSecond;
        corrosiveTimer = duration;
        corrosiveCoroutine = StartCoroutine(CorrosiveDamageRoutine());
        isCorroding = true;
    }

    private IEnumerator CorrosiveDamageRoutine()
    {
        while (corrosiveTimer > 0)
        {
            if (!isalive)
            {
                yield break;
            }
            TakeDamage(corrosiveDamagePerSecond);
            ShowDamageText((int)corrosiveDamagePerSecond, transform.position + Vector3.up * 1f);
            yield return new WaitForSeconds(1f);
            corrosiveTimer -= 1f;
        }
        isCorroding = false;
    }

    private void ShowDamageText(int damageAmount, Vector3 position)
    {
        if (damageTextPrefab != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab, position, Quaternion.identity);
            DamageText textComponent = damageText.GetComponent<DamageText>();
            if (textComponent != null)
            {
                textComponent.SetDamageText(damageAmount);
            }
        }
    }

    public void ApplyComboEffect(float duration)
    {
        if (!isalive)
            return;

        if (comboCoroutine != null)
        {
            StopCoroutine(comboCoroutine);
        }

        comboCoroutine = StartCoroutine(ComboDamageRoutine(duration));
    }

    private IEnumerator ComboDamageRoutine(float duration)
    {
        comboDamage += 1f; 
        if (comboDamage > 45.0f)
        {
            comboDamage = 45.0f;
        }

        yield return new WaitForSeconds(duration);
        comboDamage = 0f;
    }

    public float GetComboBonus()
    {
        return comboDamage;
    }

    public virtual void OnPaused()
    {
        return;
    }

}