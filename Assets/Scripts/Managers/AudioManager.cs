using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Planet / SFX Source (click planets)")]
    public AudioSource sfxSource;

    [Header("Ambience Source (background loop)")]
    public AudioSource ambienceSource;

    [Header("Ambience Clip")]
    public AudioClip ambienceClip;

    private void Awake()
    {
        // Singleton simple
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Sécurité si non assigné
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartAmbience();
    }

    // Ambience
    public void StartAmbience()
    {
        if (ambienceSource == null || ambienceClip == null) return;

        ambienceSource.loop = true;
        ambienceSource.clip = ambienceClip;

        if (!ambienceSource.isPlaying)
            ambienceSource.Play();
    }

    public void StopAmbience()
    {
        if (ambienceSource == null) return;
        ambienceSource.Stop();
        ambienceSource.clip = null;
    }

    //Planet SFX 
    public void PlayClip(AudioClip clip)
    {
        if (sfxSource == null) return;

        if (clip == null)
        {
            StopPlanetAudio();
            return;
        }

        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void StopPlanetAudio()
    {
        if (sfxSource == null) return;
        sfxSource.Stop();
        sfxSource.clip = null;
    }
}
