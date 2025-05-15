using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;   //this allows us to call the music manager from other scripts
    private AudioSource audioSource;
    public AudioClip backgroundMusic;
    [SerializeField] private Slider musicSlider;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
      
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackgroundMuisc(false, backgroundMusic);
        }

        // Auto-assign the slider if not set
        if (musicSlider == null)
        {
            GameObject foundSlider = GameObject.FindWithTag("Music Volume Slider");
            if (foundSlider != null)
            {
                musicSlider = foundSlider.GetComponent<Slider>();
                Debug.Log($"[MusicManager] musicSlider assigned: {foundSlider.name}");
            }
            else
            {
                Debug.LogWarning("[MusicManager] No GameObject tagged 'Music Volume Slider' found.");
            }
        }

        // Only add listener if slider is found
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(delegate { SetVolume(musicSlider.value); });
        }
        else
        {
            Debug.LogWarning("[MusicManager] musicSlider is null — volume control will not work.");
        }
    }

    public static void SetVolume(float volume)
    {
        Instance.audioSource.volume = volume;
    }

    public static void PlayBackgroundMuisc(bool resetSong, AudioClip audioClip = null)
    {
        if (audioClip != null)
        {
            Instance.audioSource.clip = audioClip;
        }
        if(Instance.audioSource.clip != null)   //currently playing a song and we haven't passed in a new one
        {
            if(resetSong)
            {
                Instance.audioSource.Stop();
            }
            Instance.audioSource.Play();
        }
    }

    public static void PauseBackgroundMusic()
    {
        Instance.audioSource.Pause();
    }
}
