using System;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    public HealthUI healthUI;
    public Collider2D playerHitboxCollider;
    public SpriteRenderer spriteRenderer;
    private int maxHearts;

    [Header("Damage Settings")]
    public float invincibilityDuration = 0.5f;
    public bool isInvincible = false;

    public static event Action OnPlayerDied;

    void Start()
    {
        // Auto-assign healthUI if not manually set
        if (healthUI == null)
        {
            GameObject found = GameObject.FindWithTag("Player Health UI");
            if (found != null)
            {
                healthUI = found.GetComponent<HealthUI>();
                Debug.Log($"[PlayerHealth] healthUI assigned: {found.name}");
            }
            else
            {
                Debug.LogWarning("[PlayerHealth] Could not find object with tag 'Player Health UI'");
            }
        }

        ResetHealth();
        GameController.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInvincible) return;

        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy)
            {
                TakeDamage(enemy.damage);
            }
        }

        Trap trap = collision.GetComponent<Trap>();
        if (trap && trap.damage > 0)
        {
            TakeDamage(trap.damage);
        }
    }

    void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (healthUI != null)
            healthUI.UpdateHearts(currentHealth);
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;

        if (healthUI != null)
            healthUI.SetMaxHearts(maxHealth);
        else
            Debug.LogWarning("[PlayerHealth] healthUI is null on ResetHealth");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(InvincibilityFrames());

        if (healthUI != null)
            healthUI.UpdateHearts(currentHealth);

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    public bool IsInvincible() => isInvincible;
}