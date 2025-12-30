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
        // Souris (PC)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastFromScreen(Input.mousePosition);
        }

        // Tactile (mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            RaycastFromScreen(Input.GetTouch(0).position);
        }
    }

    private void RaycastFromScreen(Vector2 screenPos)
    {
        if (ui == null) return;

        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            PlanetInfo info = hit.collider.GetComponent<PlanetInfo>();
            if (info != null)
            {
                ui.ShowPlanetInfo(info.title, info.description, info.diameterKm, info.distanceToSunMillionKm, info.revolutionDays, info.rotationDays);
            }
        }
    }
}

