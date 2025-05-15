using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Slider musicSlider;
    public Slider fxSlider;

    [Header("Input")]
    public InputActionReference pauseAction; // Assign this in the inspector

    private bool isPaused = false;

    private void OnEnable()
    {
        pauseAction.action.Enable();
        pauseAction.action.performed += OnPausePressed;
        Debug.Log("Pause input enabled: " + pauseAction.action.enabled);
    }

    private void OnDisable()
    {
        pauseAction.action.performed -= OnPausePressed;
        pauseAction.action.Disable();
    }

    private void Start()
    {
        pauseMenuPanel.SetActive(false);
        resumeButton.onClick.AddListener(ResumeGame);

        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicSlider.value = savedMusic;
        fxSlider.value = savedSFX;

        musicSlider.onValueChanged.AddListener(val => {
            MusicManager.SetVolume(val);
            PlayerPrefs.SetFloat("MusicVolume", val);
        });

        fxSlider.onValueChanged.AddListener(val => {
            SoundEffectManager.SetVolume(val);
            PlayerPrefs.SetFloat("SFXVolume", val);
        });

        MusicManager.SetVolume(savedMusic);
        SoundEffectManager.SetVolume(savedSFX);
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenuPanel.SetActive(false);
    }
}