using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statsText;

    private OrbitAround[] orbits;

    private void Start()
    {
        if (panel != null) panel.SetActive(false);

        // On récupère toutes les orbites une fois
        orbits = FindObjectsOfType<OrbitAround>(true);
    }

    public void ShowPlanetInfo(string name, string desc,
        float diameterKm, float distanceMillionKm,
        float revolutionDays, float rotationHours)
    {
        PauseRevolution(true);

        if (panel == null) return;
        panel.SetActive(true);

        if (titleText != null) titleText.text = name;
        if (descriptionText != null) descriptionText.text = desc;

        if (statsText != null)
        {
            string rotTxt = rotationHours < 0
                ? $"{Mathf.Abs(rotationHours):N1} h (rétrograde)"
                : $"{rotationHours:N1} h";

            statsText.text =
                $"Diamètre : {diameterKm:N0} km\n" +
                $"Distance au Soleil : {distanceMillionKm:N1} M km\n" +
                $"Révolution : {revolutionDays:N1} jours\n" +
                $"Rotation : {rotTxt}";
        }
    }

    public void HidePlanetInfo()
    {
        if (panel != null) panel.SetActive(false);
        PauseRevolution(false);
    }

    private void PauseRevolution(bool pause)
    {
        if (orbits == null) return;

        foreach (var o in orbits)
        {
            if (o != null) o.enabled = !pause;
        }
    }
}
