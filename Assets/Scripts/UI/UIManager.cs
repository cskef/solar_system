using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statsText;

    private void Start()
    {
        if (panel != null) panel.SetActive(false);
    }

    public void ShowPlanetInfo(string name, string desc, float diameterKm, float distanceMillionKm, float revolutionDays, float rotationDays)
    public void ShowPlanetInfo(string name, string desc, float diameterKm, float distanceMillionKm, float revolutionDays)
    {
        if (panel == null) return;

        panel.SetActive(true);

        if (titleText != null) titleText.text = name;
        if (descriptionText != null) descriptionText.text = desc;

        if (statsText != null)
        {
            statsText.text =
                $"Diamï¿½tre : {diameterKm:N0} km\n" +
                $"Distance au Soleil : {distanceMillionKm:N1} M km\n" +
                $"Rï¿½volution : {revolutionDays:N1} jours";
                $"Rotation : {rotationDays:N1} jours";
                $"Diamètre : {diameterKm:N0} km\n" +
                $"Distance au Soleil : {distanceMillionKm:N1} M km\n" +
                $"Révolution : {revolutionDays:N1} jours";
        }
    }

    public void HidePlanetInfo()
    {
        if (panel != null) panel.SetActive(false);
    }
}
