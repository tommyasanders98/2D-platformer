using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int bulletDamange = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();

        if(enemy)
        {
            // Determine horizontal direction: +1 = right, -1 = left
            float directionX = Mathf.Sign(enemy.transform.position.x - transform.position.x);

            // Apply knockback only in X direction
            Vector2 knockback = new Vector2(directionX * 5f, 0f); // You can adjust '5f' for strength

            enemy.TakeDamage(bulletDamange, knockback);

            Destroy(gameObject);
        }
    }
}
