using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlanetAlignmentController : MonoBehaviour
{
    [Header("Assigner")]
    public Transform sun;                  
    public Transform solarSystemRoot;      
    public Vector3 alignDirection = Vector3.forward;

    [Header("UI")]
    public TextMeshProUGUI buttonLabel;    

    [Header("Behaviour")]
    public bool pauseOrbitsDuringAlignment = true;

    private OrbitAround[] orbits;
    private Dictionary<Transform, Pose> savedPoses = new Dictionary<Transform, Pose>();
    private bool aligned = false;

    private void Awake()
    {
        orbits = FindObjectsByType<OrbitAround>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        UpdateButtonText();
    }

    public void ToggleAlignment()
    {
        if (!aligned)
            AlignNow();
        else
            RestoreNow();

        UpdateButtonText();
    }

    private void AlignNow()
    {
        if (sun == null) return;

        if (orbits == null || orbits.Length == 0)
            orbits = FindObjectsByType<OrbitAround>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        savedPoses.Clear();

        Transform refTr = (solarSystemRoot != null) ? solarSystemRoot : sun;
        Vector3 dirWorld = refTr.TransformDirection(alignDirection).normalized;
        if (dirWorld.sqrMagnitude < 0.000001f) dirWorld = Vector3.forward;

        foreach (var o in orbits)
        {
            if (o == null || o.center == null) continue;

            Transform planet = o.transform;
            savedPoses[planet] = new Pose(planet.position, planet.rotation);

            Vector3 axisWorld = o.center.TransformDirection(o.orbitAxis).normalized;
            Vector3 offset = planet.position - o.center.position;
            Vector3 offsetOnPlane = Vector3.ProjectOnPlane(offset, axisWorld);
            float r = offsetOnPlane.magnitude;

            Vector3 dirOnPlane = Vector3.ProjectOnPlane(dirWorld, axisWorld).normalized;
            if (dirOnPlane.sqrMagnitude < 0.000001f)
                dirOnPlane = Vector3.ProjectOnPlane(Vector3.forward, axisWorld).normalized;

            planet.position = o.center.position + dirOnPlane * r;
        }

        if (pauseOrbitsDuringAlignment)
        {
            foreach (var o in orbits)
                if (o != null) o.enabled = false;
        }

        aligned = true;
    }

    private void RestoreNow()
    {
        foreach (var kv in savedPoses)
        {
            if (kv.Key == null) continue;
            kv.Key.SetPositionAndRotation(kv.Value.position, kv.Value.rotation);
        }

        if (orbits != null)
        {
            foreach (var o in orbits)
                if (o != null) o.enabled = true;
        }

        aligned = false;
    }

    private void UpdateButtonText()
    {
        if (buttonLabel == null) return;

        buttonLabel.text = aligned
            ? "Retour état"
            : "Alignement";
    }
}
