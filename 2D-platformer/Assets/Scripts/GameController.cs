using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
    int progressAmount; //level completion progress
    public Slider progressSlider;   //slider for visual representation

    private void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;   //increases progress amount based on gem value
        progressSlider.value = progressAmount;
        if (progressAmount >= 100)
        {
            //Level complete code
            Debug.Log("Level Complete");
        }
    }
}
