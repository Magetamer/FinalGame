using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;
    private AudioSource audioSource;

    public AudioClip backgroundMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);

            // Load saved volume
            audioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }
    }

    // Called by scene UI sliders, syncs audio control between scenes
    public void BindSlider(UnityEngine.UI.Slider slider)
    {
        if (slider == null) return;

        slider.value = audioSource.volume;
        slider.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.AddListener(SetVolume);
    }

    public static void SetVolume(float volume)
    {
        if (Instance == null) return;

        Instance.audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public static void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        if (Instance == null) return;

        if (audioClip != null)
            Instance.audioSource.clip = audioClip;

        if (Instance.audioSource.clip != null)
        {
            if (resetSong)
                Instance.audioSource.Stop();

            Instance.audioSource.Play();
        }
    }

    public static void PauseBackgroundMusic()
    {
        if (Instance == null) return;
        Instance.audioSource.Pause();
    }
}
