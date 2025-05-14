using System;
using UnityEngine;

public class HealthItem : MonoBehaviour, Item
{
    public int healthAmount = 1;
    public static event Action<int> OnHealthCollect;

    public void Collect()
    {
        OnHealthCollect.Invoke(healthAmount);
        Destroy(gameObject);
    }

  
}

