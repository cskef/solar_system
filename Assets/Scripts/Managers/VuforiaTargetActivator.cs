using UnityEngine;
using Vuforia;

public class VuforiaTargetActivator : MonoBehaviour
{
    public GameObject solarSystemRoot;
    public GameObject vuforiaHintUI;

    public GameObject orbitRingsRoot;
    public GameObject timeControlsGroup;

    public OrbitToggle orbitToggle; 

    private ObserverBehaviour observer;

    private void Awake()
    {
        observer = GetComponent<ObserverBehaviour>();
    }

    private void OnEnable()
    {
        if (!LaunchState.UseVuforia || LaunchState.StartInQuiz)
        {
            ForceOff();
            return;
        }

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
        if (vuforiaHintUI != null)
            vuforiaHintUI.SetActive(!tracked);

        if (solarSystemRoot != null)
            solarSystemRoot.SetActive(tracked);

        if (timeControlsGroup != null)
            timeControlsGroup.SetActive(tracked);

        // Orbites : ne s'affichent que si tracked
        if (orbitToggle != null)
        {
            orbitToggle.SetOrbits(tracked); // ON -> rebuild automatique
        }
        else if (orbitRingsRoot != null)
        {
            orbitRingsRoot.SetActive(tracked);

            if (tracked)
            {
                var drawer = orbitRingsRoot.GetComponentInChildren<OrbitRingDrawer>(true);
                if (drawer != null) drawer.RebuildNextFrame();

                var scaleCtrl = FindFirstObjectByType<ScaleModeController>(FindObjectsInactive.Include);
                if (scaleCtrl != null) scaleCtrl.InitializeIfNeeded();
            }
        }

        if (!tracked) return;

        // Quand tracked : recalculer rayons
        if (solarSystemRoot != null)
        {
            foreach (var orb in solarSystemRoot.GetComponentsInChildren<OrbitAround>(true))
                orb.RecalculateRadius();
        }
    }

    private void ForceOff()
    {
        if (vuforiaHintUI != null) vuforiaHintUI.SetActive(false);
        if (solarSystemRoot != null) solarSystemRoot.SetActive(false);
        if (orbitRingsRoot != null) orbitRingsRoot.SetActive(false);
        if (timeControlsGroup != null) timeControlsGroup.SetActive(false);

        if (orbitToggle != null)
            orbitToggle.SetOrbits(false);
    }
}
