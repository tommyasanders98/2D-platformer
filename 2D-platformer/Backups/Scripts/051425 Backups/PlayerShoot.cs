using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;   //projectile speed
    //public Vector3 bulletPosition;
   
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)) //Left Click
        {
            //Shoot();
            GetComponent<Animator>().SetTrigger("attack");
        }
    }

    void Shoot()
    {
        //Get mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Direction for us to use
        Vector3 shootDirection = (mousePosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(shootDirection.x, shootDirection.y) * bulletSpeed;
        Destroy(bullet, 2f);    //destroys bullet after a certain amount of time
    }
}
