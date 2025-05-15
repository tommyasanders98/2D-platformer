using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject player;
    public GameObject LoadCanvas;
    public List<GameObject> levels;
    private int currentLevelIndex = 0;
    public Vector3 spawnPosition = Vector3.zero;

    public GameObject gameOverScreen;
    public TMP_Text survivedText;
    private int survivedLevelsCount;

    public XPManager xpManager;

    public static event Action OnReset;

    private void Start()
    {
        // Auto-assign Player by Tag if not manually set
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer;
                Debug.Log($"[GameController] Player assigned automatically: {player.name}");
            }
            else
            {
                Debug.LogWarning("[GameController] Could not find Player in scene!");
            }
        }

        //  Auto-assign LoadCanvas by tag if missing
        if (LoadCanvas == null)
        {
            GameObject foundCanvas = GameObject.FindWithTag("Load Canvas");
            if (foundCanvas != null)
            {
                LoadCanvas = foundCanvas;
                Debug.Log($"[GameController] LoadCanvas assigned automatically: {LoadCanvas.name}");
            }
            else
            {
                Debug.LogWarning("[GameController] Could not find Load Canvas in scene!");
            }
        }

        HoldToLoadLevel.OnHoldComplete += LoadNextLevel;
        PlayerHealth.OnPlayerDied += GameOverScreen;

        if (LoadCanvas != null) LoadCanvas.SetActive(false);
        if (gameOverScreen != null) gameOverScreen.SetActive(false);

        if (xpManager != null)
            xpManager.ResetXP();
    }

    void GameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
            MusicManager.PauseBackgroundMusic();

            survivedText.text = "YOU SURVIVED " + survivedLevelsCount + " LEVEL";
            if (survivedLevelsCount != 1) survivedText.text += "S";

            Time.timeScale = 0;
        }
    }

    public void ResetGame()
    {
        if (gameOverScreen != null)
            gameOverScreen.SetActive(false);

        MusicManager.PlayBackgroundMuisc(true);
        survivedLevelsCount = 0;
        LoadLevel(0, false);
        OnReset?.Invoke();
        Time.timeScale = 1;

        if (xpManager != null)
            xpManager.ResetXP();
    }

    void LoadLevel(int level, bool wantSurvivedIncrease)
    {
        if (LoadCanvas != null) LoadCanvas.SetActive(false);

        levels[currentLevelIndex].SetActive(false);
        levels[level].SetActive(true);

        if (player != null)
            player.transform.position = spawnPosition;

        currentLevelIndex = level;

        if (wantSurvivedIncrease) survivedLevelsCount++;
    }

    void LoadNextLevel()
    {
        int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
        LoadLevel(nextLevelIndex, true);
    }
}