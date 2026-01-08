using UnityEngine;
using Vuforia;

public class VuforiaTargetActivator : MonoBehaviour
{
    public GameObject solarSystemRoot;   // solar_system_elements
    public GameObject vuforiaHintUI;     // UI "Scannez l'image..."

    private ObserverBehaviour observer;

    private void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();
    }

    private void OnEnable()
    {
        // Si on n'est PAS en mode Vuforia, on cache tout et on ne fait rien
        if (!LaunchState.UseVuforia || LaunchState.StartInQuiz)
        {
            ForceOff();
            return;
        }

        // Mode Vuforia : au début on affiche le hint et on cache le système
        SetState(false);

        if (observer != null)
            observer.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    private void OnDisable()
    {
        if (observer != null)
            observer.OnTargetStatusChanged -= OnTargetStatusChanged;
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (!LaunchState.UseVuforia || LaunchState.StartInQuiz)
        {
            ForceOff();
            return;
        }

        bool tracked =
            status.Status == Status.TRACKED ||
            status.Status == Status.EXTENDED_TRACKED;

        SetState(tracked);
    }

    private void SetState(bool tracked)
    {
        if (solarSystemRoot != null)
            solarSystemRoot.SetActive(tracked);

        if (vuforiaHintUI != null)
            vuforiaHintUI.SetActive(!tracked);

        if (solarSystemRoot != null)
        {
            foreach (var orb in solarSystemRoot.GetComponentsInChildren<OrbitAround>(true))
            {
                orb.RecalculateRadius();
            }
        }


        if (tracked)
        {
            var drawer = FindObjectOfType<OrbitRingDrawer>(true);
            if (drawer != null) drawer.RebuildNextFrame();
        }
    }

    private void ForceOff()
    {
        if (vuforiaHintUI != null) vuforiaHintUI.SetActive(false);
        if (solarSystemRoot != null) solarSystemRoot.SetActive(false);
    }
}
