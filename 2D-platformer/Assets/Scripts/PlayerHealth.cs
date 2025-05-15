using System;
using System.Collections;
using Unity.VisualScripting;
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

    public static event Action OnPlayerDied;    //lets the UI know when the player is dead
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        ResetHealth();
        GameController.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collision detected: {collision.name}");
        // Only respond if this trigger event is for the player damage hitbox
        //if (collision != playerHitboxCollider) return;

        if (isInvincible) return;

        // If this is NOT the assigned damage-receiving collider, ignore it
        //if (collision != playerHitboxCollider)
            //return;

        Debug.Log($"[Damage Check] Triggered by: {collision.name}");

        // Only take damage from objects tagged as \"Enemy\"
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
        if(currentHealth > maxHealth)                       //can't go over max health
        {
            currentHealth = maxHealth;
        }

        healthUI.UpdateHearts(currentHealth);   //update health UI to show correct health
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(InvincibilityFrames());      //invinccibiilty timer 
        healthUI.UpdateHearts(currentHealth);

        //Flash red
        StartCoroutine(FlashRed());

        if(currentHealth <= 0)
        {
            //player dead
            OnPlayerDied.Invoke();
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
        yield return new WaitForSeconds(0.2f); //delay so character stays red for a set amount of time
        spriteRenderer.color = Color.white;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}
