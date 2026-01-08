using UnityEngine;
using TMPro;

public class TapToPlaceSolarSystem : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform solarSystemRoot;
    public TextMeshProUGUI hintText; 

    [Header("Placement")]
    public float distanceInFrontOfCamera = 0.8f;
    public float uniformScale = 0.7f;
    public Vector3 extraRotation = Vector3.zero;

    private bool placed = false;
    private bool placementEnabled = true;

    private void OnEnable()
    {
        // À chaque activation du groupe Tap, on remet l’état correct
        placed = false;
        placementEnabled = !LaunchState.StartInQuiz && !LaunchState.UseVuforia;

        if (solarSystemRoot != null)
            solarSystemRoot.gameObject.SetActive(false);

        UpdateHint();
    }

    private void Update()
    {
        if (!placementEnabled) return;
        if (placed) return;

        // Touch
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Place();
            return;
        }

        // Mouse (test PC)
        if (Input.GetMouseButtonDown(0))
        {
            Place();
        }
    }

    private void Place()
    {
        if (solarSystemRoot == null)
        {
            Debug.LogError("TapToPlaceSolarSystem: solarSystemRoot not assigned.");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("TapToPlaceSolarSystem: MainCamera not found. Tag ARCamera as MainCamera.");
            return;
        }

        Vector3 pos = cam.transform.position + cam.transform.forward * distanceInFrontOfCamera;

        // rotation seulement sur Y
        Vector3 forwardFlat = cam.transform.forward;
        forwardFlat.y = 0f;
        if (forwardFlat.sqrMagnitude < 0.0001f)
            forwardFlat = Vector3.forward;

        solarSystemRoot.position = pos;
        solarSystemRoot.rotation =
            Quaternion.LookRotation(forwardFlat.normalized, Vector3.up) * Quaternion.Euler(extraRotation);

        solarSystemRoot.localScale = Vector3.one * uniformScale;

        solarSystemRoot.gameObject.SetActive(true);
        placed = true;
        foreach (var orb in solarSystemRoot.GetComponentsInChildren<OrbitAround>(true))
        {
            orb.RecalculateRadius();
        }


        // Rebuild orbites
        OrbitRingDrawer drawer = FindObjectOfType<OrbitRingDrawer>(true);
        if (drawer != null) drawer.RebuildNextFrame();

        UpdateHint();
    }

    private void UpdateHint()
    {
        if (hintText == null) return;

        bool shouldShow = placementEnabled && !placed;

        hintText.gameObject.SetActive(shouldShow);
        if (shouldShow)
            hintText.text = "Tapez sur l’écran pour placer le système solaire";
    }

    public bool IsPlaced() => placed;

    // Si tu veux réactiver manuellement le placement
    public void ResetPlacement()
    {
        placed = false;
        placementEnabled = !LaunchState.StartInQuiz && !LaunchState.UseVuforia;

        if (solarSystemRoot != null)
            solarSystemRoot.gameObject.SetActive(false);

        UpdateHint();
    }
}
