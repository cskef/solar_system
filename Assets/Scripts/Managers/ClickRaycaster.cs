using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class ClickRaycaster : MonoBehaviour
{
    [Header("References")]
    public UIManager uiManager;
    public TapToPlaceSolarSystem placementManager;

    [Header("Raycast")]
    public LayerMask planetsLayer;
    public float maxDistanceTap = 10f;
    public float maxDistanceVuforia = 50f;

    private void Awake()
    {
        if (uiManager == null)
            uiManager = FindFirstObjectByType<UIManager>(FindObjectsInactive.Include);

        // placementManager peut être null en Vuforia (OK)
        if (placementManager == null)
            placementManager = FindFirstObjectByType<TapToPlaceSolarSystem>(FindObjectsInactive.Include);
    }

    private void Update()
    {
        // Touch mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Ignore UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            // En mode Tap : si pas placé, on laisse TapToPlace gérer le premier tap
            if (!LaunchState.UseVuforia)
            {
                if (placementManager != null && !placementManager.IsPlaced())
                    return;
            }

            RaycastFromScreen(Input.GetTouch(0).position);
        }

        // Souris editor
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (!LaunchState.UseVuforia)
            {
                if (placementManager != null && !placementManager.IsPlaced())
                    return;
            }

            RaycastFromScreen(Input.mousePosition);
        }
    }

    private void RaycastFromScreen(Vector2 screenPos)
    {
        if (uiManager == null) return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("ClickRaycaster: No MainCamera found. Tag ARCamera as MainCamera.");
            return;
        }

        float maxDist = LaunchState.UseVuforia ? maxDistanceVuforia : maxDistanceTap;

        Ray ray = cam.ScreenPointToRay(screenPos);

        // RaycastAll + tri par distance = plus fiable en AR
        RaycastHit[] hits = Physics.RaycastAll(ray, maxDist, planetsLayer);
        if (hits == null || hits.Length == 0) return;

        RaycastHit hit = hits.OrderBy(h => h.distance).First();

        PlanetInfo info = hit.collider.GetComponent<PlanetInfo>();
        if (info == null)
            info = hit.collider.GetComponentInParent<PlanetInfo>();

        if (info == null) return;

        uiManager.ShowPlanetInfo(
            info.title,
            info.description,
            info.diameterKm,
            info.distanceToSunMillionKm,
            info.revolutionPeriod,
            info.rotationPeriod
        );

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayClip(info.audioClip);
    }
}
