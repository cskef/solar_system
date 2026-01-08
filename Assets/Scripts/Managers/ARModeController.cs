using UnityEngine;

public class ARModeController : MonoBehaviour
{
    [Header("Mode Groups (assign in Inspector)")]
    public GameObject tapPlacementGroup;   // TapPlacementGroup (PlacementManager + HintText)
    public GameObject vuforiaGroup;        // VuforiaGroup (ImageTarget)
    public GameObject quizGroup;           // QuizGroup (contains QuizPanel)
    public GameObject exploreUI;           // Canvas/ExploreUI

    [Header("Solar System")]
    public Transform solarSystemRoot;      // solar_system_elements (root)
    public Transform vuforiaAnchor;        // VuforiaGroup/ImageTarget

    [Header("Vuforia Placement")]
    [Tooltip("Scale applied to solar_system_elements when attached to ImageTarget. Adjust once, then keep.")]
    public float vuforiaSolarScale = 0.05f;

    private void Start()
    {
        ApplyMode();
    }

    public void ApplyMode()
    {
        // 1) QUIZ MODE
        if (LaunchState.StartInQuiz)
        {
            SetActiveSafe(exploreUI, false);
            SetActiveSafe(tapPlacementGroup, false);
            SetActiveSafe(vuforiaGroup, false);
            SetActiveSafe(quizGroup, true);

            // Système solaire pas nécessaire en quiz
            HideSolarSystem();

            return;
        }

        // 2) EXPLORER MODE 
        SetActiveSafe(quizGroup, false);
        SetActiveSafe(exploreUI, true);

        // Explorer via Vuforia ?
        if (LaunchState.UseVuforia)
        {
            SetActiveSafe(tapPlacementGroup, false);
            SetActiveSafe(vuforiaGroup, true);

            AttachSolarSystemToVuforia();
        }
        else
        {
            // Explorer via Tap
            SetActiveSafe(vuforiaGroup, false);
            SetActiveSafe(tapPlacementGroup, true);

            PrepareSolarSystemForTap();
        }
    }

    // Solar System helpers

    private void AttachSolarSystemToVuforia()
    {
        if (solarSystemRoot == null)
        {
            Debug.LogError("ARModeController: solarSystemRoot is not assigned.");
            return;
        }

        if (vuforiaAnchor == null)
        {
            Debug.LogError("ARModeController: vuforiaAnchor (ImageTarget) is not assigned.");
            return;
        }

        // Attacher le système solaire à l'ancre Vuforia stabilisée
        solarSystemRoot.SetParent(vuforiaAnchor, false);

        // IMPORTANT : ne pas forcer de rotation ici
        solarSystemRoot.localPosition = Vector3.zero;
        solarSystemRoot.localRotation = Quaternion.identity;
        solarSystemRoot.localScale = Vector3.one * vuforiaSolarScale;

        solarSystemRoot.gameObject.SetActive(true);

        // Recalibrer les orbites après attachement
        OrbitRingDrawer drawer =
            Object.FindFirstObjectByType<OrbitRingDrawer>(FindObjectsInactive.Include);

        if (drawer != null)
            drawer.RebuildNextFrame();

        // Recalculer les rayons des planètes
        foreach (var orb in solarSystemRoot.GetComponentsInChildren<OrbitAround>(true))
        {
            orb.RecalculateRadius();
        }
    }


    private void PrepareSolarSystemForTap()
    {
        if (solarSystemRoot == null)
        {
            Debug.LogError("ARModeController: solarSystemRoot is not assigned.");
            return;
        }

        // Detach from any parent (ImageTarget), keep world pos doesn't matter because we hide it
        solarSystemRoot.SetParent(null, true);

        // Hide it: TapToPlaceSolarSystem will enable and place it after tap
        solarSystemRoot.gameObject.SetActive(false);
    }

    private void HideSolarSystem()
    {
        if (solarSystemRoot == null) return;

        solarSystemRoot.SetParent(null, true);
        solarSystemRoot.gameObject.SetActive(false);
    }

    private void SetActiveSafe(GameObject go, bool state)
    {
        if (go != null) go.SetActive(state);
    }
}
