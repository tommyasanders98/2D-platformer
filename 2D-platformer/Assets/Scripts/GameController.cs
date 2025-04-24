using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int survivedLevelsCount;

    public static event Action OnReset;  //tells other scripts when the game has been reset

    private void Start()
    {
        progressAmount = 0;
        progressSlider.value = 0;
        Gem.OnGemCollect += IncreaseProgressAmount;
        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;
        LoadCanvas.SetActive(false);
        gameOverScreen.SetActive(false);    //turns off game over screen on start
    }

    void GameOverScreen()
    {
        gameOverScreen.SetActive(true);
        survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVEL";
        if (survivedLevelsCount != 1) survivedText.text += "S"; //adds plurral for anything other than 1
        Time.timeScale = 0; //pauses game when game over screen pops up
    }

    public void ResetGame()
    {
        gameOverScreen.SetActive(false);
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset.Invoke();   //tells other scripts when this event has happened
        Time.timeScale = 1; //start time once game has reset
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

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        LoadCanvas.SetActive(false);    //this will only load one level per hold

        levels[currentLevelIndex].gameObject.SetActive(false);
        levels[level].gameObject.SetActive(true);

        player.transform.position = spawnPosition;  //move the player to the spawn loaction

        currentLevelIndex = level; //change to next level
        progressAmount = 0; //reset progress
        progressSlider.value = 0;   //reset slider
        if(wantSurvivedIncrease) survivedLevelsCount++;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;   //if we are on last level, set the level back to level 1 or go to next level
        LoadLevel(nextLevelIndex, true);  //allows us to use load level upon first load and restart option
    }
}
