using UnityEngine;

public class ClickRaycaster : MonoBehaviour
{
    private UIManager ui;

    private void Start()
    {
        ui = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        // Clic souris (Editor/PC)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastFromScreen(Input.mousePosition);
        }

        // Touch mobile
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastFromScreen(Input.GetTouch(0).position);
        }
    }

    private void RaycastFromScreen(Vector2 screenPos)
    {
        if (ui == null) return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("ClickRaycaster: No MainCamera found. Tag your camera as MainCamera.");
            return;
        }

        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // On récupère PlanetInfo sur l'objet touché (collider)
            PlanetInfo info = hit.collider.GetComponent<PlanetInfo>();

            // Si PlanetInfo est sur un parent (cas prefab), on essaye aussi le parent
            if (info == null)
                info = hit.collider.GetComponentInParent<PlanetInfo>();

            if (info == null) return;

            // 1) Affiche les infos dans l'UI
            ui.ShowPlanetInfo(
                info.title,
                info.description,
                info.diameterKm,
                info.distanceToSunMillionKm,
                info.revolutionPeriod,
                info.rotationPeriod
            );

            // 2) Joue l'audio de la planète (si un clip est assigné)
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayClip(info.audioClip);
            }
        }
    }
}
