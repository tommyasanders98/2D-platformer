using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = GetComponentInParent<PlayerController>();
        if (!player || !player.isAttacking || player.currentWeapon == null) return;

        IHittable hittable = collision.GetComponent<IHittable>();
        if (hittable != null)
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            float damage = player.currentWeapon.damage;
            hittable.Hit(direction, damage);
        }
    }
}