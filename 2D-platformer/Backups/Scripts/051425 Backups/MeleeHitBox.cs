using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    public int damage = 1;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if player is actively attacking
        PlayerMovement player = GetComponentInParent<PlayerMovement>();
        if (!player || !player.isAttacking || player.currentWeapon == null) return;

        // Check for enemy
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            Vector2 knockback = direction.normalized * player.currentWeapon.knockbackForce;

            enemy.TakeDamage(player.currentWeapon.damage, knockback);
        }
    }
}