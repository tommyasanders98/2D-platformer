using UnityEngine;

public class PlayerDamageReceiver : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GetComponentInParent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!playerHealth || playerHealth.IsInvincible()) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>() ?? collision.GetComponentInParent<Enemy>();
            if (enemy)
            {
                playerHealth.TakeDamage(enemy.damage);
            }
        }

        Trap trap = collision.GetComponent<Trap>();
        if (trap && trap.damage > 0)
        {
            playerHealth.TakeDamage(trap.damage);
        }
    }
}
