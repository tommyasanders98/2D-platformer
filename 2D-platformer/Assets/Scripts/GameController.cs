using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour 
{
    int progressAmount; //level completion progress
    public Slider progressSlider;   //slider for visual representation

    public GameObject player;
    public GameObject LoadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;
    public Vector3 spawnPosition = Vector3.zero;

    private void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        LoadCanvas.SetActive(false);
    }

    void IncreaseProgressAmount(int amount)
    {
        progressAmount += amount;   //increases progress amount based on gem value
        progressSlider.value = progressAmount;
        if (progressAmount >= 100)
        {
            //Level complete code
            LoadCanvas.SetActive(true); 
            Debug.Log("Level Complete");
        }
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;   //if we are on last level, set the level back to level 1 or go to next level
        LoadCanvas.SetActive(false);    //this will only load one level per hold

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[nextLevelIndex].gameObject.SetActive(true);

        player.transform.position = spawnPosition;  //move the player to the spawn loaction

        currentLevelIndex = nextLevelIndex; //change to next level
        progressAmount = 0; //reset progress
        progressSlider.value = 0;   //reset slider
    }
}
