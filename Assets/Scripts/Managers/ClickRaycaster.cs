using UnityEngine;
using UnityEngine.EventSystems;

public class ClickRaycaster : MonoBehaviour
{
    [Header("References")]
    public UIManager uiManager;
    public TapToPlaceSolarSystem placementManager;

    [Header("Raycast")]
    public LayerMask planetsLayer; 
    public float maxDistance = 10f;

    private void Awake()
    {
        // Auto-find si non assigné
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();

        if (placementManager == null)
            placementManager = FindObjectOfType<TapToPlaceSolarSystem>();
    }

    private void Update()
    {
        //Touch mobile 
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // 1) Si pas encore placé : on laisse TapToPlace gérer le premier tap
            if (placementManager != null && !placementManager.IsPlaced())
                return;

            // 2) Ignore si le doigt touche l'UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                return;

            RaycastFromScreen(Input.GetTouch(0).position);
        }

        // Souris (Editor/PC) 
        if (Input.GetMouseButtonDown(0))
        {
            // 1) Si pas encore placé : on laisse TapToPlace gérer
            if (placementManager != null && !placementManager.IsPlaced())
                return;

            // 2) Ignore si la souris clique sur l'UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

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

        Ray ray = cam.ScreenPointToRay(screenPos);

        // Raycast uniquement sur le layer Planets
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, planetsLayer))
        {
            PlanetInfo info = hit.collider.GetComponent<PlanetInfo>();
            if (info == null)
                info = hit.collider.GetComponentInParent<PlanetInfo>();

            if (info == null) return;

            // UI
            uiManager.ShowPlanetInfo(
                info.title,
                info.description,
                info.diameterKm,
                info.distanceToSunMillionKm,
                info.revolutionPeriod,
                info.rotationPeriod
            );

            // Audio planète
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayClip(info.audioClip);
        }
    }
}
