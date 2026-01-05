using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject panel;

    [Header("Texts")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statsText;

    private OrbitAround[] orbits;

    private void Awake()
    {
        // Cache le panel au lancement
        if (panel != null)
            panel.SetActive(false);

        // Récupère toutes les révolutions (pour pause/reprise)
        orbits = FindObjectsOfType<OrbitAround>(true);
    }


    /// Affiche les informations d'une planète
    public void ShowPlanetInfo(
        string title,
        string description,
        float diameterKm,
        float distanceMillionKm,
        float revolutionDays,
        float rotationHours
    )
    {
        PauseRevolution(true);

        if (panel != null)
            panel.SetActive(true);

        if (titleText != null)
            titleText.text = title;

        if (descriptionText != null)
            descriptionText.text = description;

        if (statsText != null)
        {
            string rotationText = rotationHours < 0
                ? $"{Mathf.Abs(rotationHours):N1} h (rétrograde)"
                : $"{rotationHours:N1} jours";

            statsText.text =
                $"Diamètre : {diameterKm:N0} km\n" +
                $"Distance au Soleil : {distanceMillionKm:N1} M km\n" +
                $"Révolution : {revolutionDays:N1} jours\n" +
                $"Rotation : {rotationText}";
        }
    }


    /// Ferme le panneau d'information
    public void HidePlanetInfo()
    {
        if (panel != null)
            panel.SetActive(false);

        PauseRevolution(false);

        // Stop audio planète
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopPlanetAudio();
    }


    /// Pause ou reprend la révolution des planètes
    private void PauseRevolution(bool pause)
    {
        if (orbits == null) return;

        foreach (OrbitAround orbit in orbits)
        {
            if (orbit != null)
                orbit.enabled = !pause;
        }
    }
}
