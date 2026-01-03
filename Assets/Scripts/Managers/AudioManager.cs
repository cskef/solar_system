using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource source;

    private void Awake()
    {
        // Singleton simple
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Si pas assigné, on essaie de le récupérer
        if (source == null)
            source = GetComponent<AudioSource>();

        // Optionnel : garder le manager entre Menu et Scene AR
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClip(AudioClip clip)
    {
        if (source == null) return;

        if (clip == null)
        {
            Stop();
            return;
        }

        source.Stop();
        source.clip = clip;
        source.Play();
    }

    public void Stop()
    {
        if (source == null) return;
        source.Stop();
        source.clip = null;
    }
}
