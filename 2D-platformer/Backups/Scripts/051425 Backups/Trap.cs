using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Trap : MonoBehaviour
{
    public float bounceForce = 10f;
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerBounce(collision.gameObject);
        }
    }

    private void HandlePlayerBounce(GameObject player)
    {
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if(rb)
        {
            //reset player velocity
            rb.linearVelocity = new Vector2(rb.linearVelocityX, 0);

            //apply bounch force
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);     //the forcemode2d.impulse uses the rigid body's mass to generator a force
        }
    }
}
