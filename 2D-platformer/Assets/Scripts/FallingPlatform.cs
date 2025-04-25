using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    public float fallWait = 2f;     //when player jumps on platform, how long before falling
    public float destroyWait = 1f;  //delay before being destroyed after falling

    bool isFalling;
    Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!isFalling && collision.gameObject.CompareTag("Player"))     //if the platform is static and collides with the player tag collision area
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {

        isFalling = true;
        yield return new WaitForSeconds(fallWait);
        rb.bodyType = RigidbodyType2D.Dynamic;          //change to dynamic so that it is affected by gravity after the wait period 
        Destroy(gameObject, destroyWait);               //destory game object after wait
    }
}
