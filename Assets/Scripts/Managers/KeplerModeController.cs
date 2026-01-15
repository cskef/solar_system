using UnityEngine;
using TMPro;

public class KeplerModeController : MonoBehaviour
{
    public static KeplerModeController Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI buttonLabel;

    [Header("State")]
    public bool keplerOn = false;

    private OrbitAround[] circularOrbits;
    private OrbitKepler[] keplerOrbits;
    private OrbitRingDrawer ringDrawer;

    private void Awake()
    {
        // Singleton safe (évite les Instances null si 2 objets existent)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Cache();
        UpdateButtonText();
        ApplyState();
    }

    private void Cache()
    {
        circularOrbits = FindObjectsByType<OrbitAround>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        keplerOrbits = FindObjectsByType<OrbitKepler>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ringDrawer = FindFirstObjectByType<OrbitRingDrawer>(FindObjectsInactive.Include);
    }

    public void ToggleKepler()
    {
        Debug.Log("Kepler button clicked ✅");

        // Re-cache à chaque clic (robuste si objets activés/désactivés en AR)
        Cache();

        keplerOn = !keplerOn;

        ApplyState();
        UpdateButtonText();

        if (ringDrawer != null)
            ringDrawer.RebuildNextFrame();
        else
            Debug.LogWarning("KeplerModeController: OrbitRingDrawer not found.");
    }

    public void ApplyState()
    {
        if (circularOrbits == null || keplerOrbits == null) Cache();

        if (!keplerOn)
        {
            // Kepler OFF
            if (keplerOrbits != null)
                foreach (var k in keplerOrbits)
                    if (k != null) k.enabled = false;

            if (circularOrbits != null)
                foreach (var o in circularOrbits)
                    if (o != null) o.enabled = true;
        }
        else
        {
            // Kepler ON : activer + init puis couper cercle
            if (keplerOrbits != null)
                foreach (var k in keplerOrbits)
                {
                    if (k == null) continue;
                    k.enabled = true;
                    k.InitializeFromCurrent();
                }

            if (circularOrbits != null)
                foreach (var o in circularOrbits)
                    if (o != null) o.enabled = false;
        }
    }

    private void UpdateButtonText()
    {
        if (buttonLabel == null) return;
        buttonLabel.text = keplerOn ? "Kepler : ON" : "Kepler : OFF";
    }
}
