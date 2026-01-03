using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioClip mercure;
    public AudioClip venus;
    public AudioClip terre;
    public AudioClip mars;
    public AudioClip jupiter;
    public AudioClip uranus;
    public AudioClip saturne;
    public AudioClip neptune;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                string planetTag = hit.transform.tag;
                PlayPlanetSound(planetTag);
            }
        }
    }

    private void PlayPlanetSound(string tag)
    {
        AudioClip clip = null;
        switch (tag)
        {
            case "mercure":
                clip = mercure;
                break;
            case "venus":
                clip = venus;
                break;
            case "terre":
                clip = terre;
                break;
            case "mars":
                clip = mars;
                break;
            case "jupiter":
                clip = jupiter;
                break;
            case "uranus":
                clip = uranus;
                break;
            case "saturne":
                clip = saturne;
                break;
            case "neptune":
                clip = neptune;
                break;
            case "UIManagerObject":
                audioSource.Stop();
                break;
        }
        if (clip != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(clip, 1f);
        }
    }
}

