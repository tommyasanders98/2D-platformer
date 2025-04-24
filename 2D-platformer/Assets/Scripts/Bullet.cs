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
            enemy.TakeDamage(bulletDamange); //calls function in enemy script

            Destroy(gameObject);
        }
    }
}
