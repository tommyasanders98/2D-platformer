using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpeedItem : MonoBehaviour, Item    //Item interface tells us that this class must have a collect function so that it can be picke up by the player
{
    public static event Action<float> OnSpeedCollected;
    public float speedMultiplier = 1.5f;    //speed multiplier for increase in characters velocity
    public void Collect()
    {
        OnSpeedCollected.Invoke(speedMultiplier);
        Destroy(gameObject);
    }
}
