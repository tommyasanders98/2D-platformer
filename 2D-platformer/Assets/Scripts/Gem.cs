using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour, Item 
{
    public static event Action<int> OnGemCollect;
    public int worth = 5; //we can add different gems that are worth different amounts
    public void Collect()
    {
        OnGemCollect.Invoke(worth); //sets up an event that other scripts can access
        SoundEffectManager.Play("Gem"); //play the sound for picking up a gem
        Destroy(gameObject);    //when we pick up gem, it will disappear
    }

}
