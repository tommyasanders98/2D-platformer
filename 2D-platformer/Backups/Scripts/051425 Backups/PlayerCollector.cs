using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collison)
    {
        Item item = collison.GetComponent<Item>();
        if (item != null )
        {
            item.Collect();
        }
    }

}