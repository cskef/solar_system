using UnityEngine;
using System.Linq;
using System.Collections;

public class OrbitRingDrawer : MonoBehaviour
{
    [Header("Optional (can be empty)")]
    public OrbitAround[] planetOrbits;

    [Header("Render Settings")]
    public int segments = 160;
    public float lineWidth = 0.004f;
    public float yExtraOffset = 0.0f;
    public bool loop = true;

    [Header("Material (optional)")]
    public Material lineMaterial;

    private void Awake()
    {
        // Mat par défaut si pas assignée
        if (lineMaterial == null)
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    public void RebuildNextFrame()
    {
        StartCoroutine(RebuildCoroutine());
    }

    private IEnumerator RebuildCoroutine()
    {
        yield return null; // attendre 1 frame après activation/placement
        RebuildNow();
    }

    public void RebuildNow()
    {
        if (planetOrbits == null || planetOrbits.Length == 0)
            planetOrbits = FindObjectsByType<OrbitAround>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // Nettoie les anciens rings
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        if (planetOrbits == null || planetOrbits.Length == 0)
        {
            Debug.LogWarning("OrbitRingDrawer: No OrbitAround found.");
            return;
        }

        var valid = planetOrbits
            .Where(o => o != null && o.center != null)
            .OrderBy(o => GetWorldRadiusOnOrbitPlane(o))
            .ToArray();

        foreach (var orbit in valid)
        {
            float worldR = GetWorldRadiusOnOrbitPlane(orbit);
            if (worldR <= 0.0001f) continue;

            CreateRing(orbit, worldR);
        }
    }

    // Rayon en WORLD mais calculé sur le plan de l'orbite (cohérent avec OrbitAround)
    private float GetWorldRadiusOnOrbitPlane(OrbitAround orbit)
    {
        Vector3 axisWorld = GetOrbitAxisWorld(orbit);
        Vector3 offset = orbit.transform.position - orbit.center.position;
        Vector3 offsetOnPlane = Vector3.ProjectOnPlane(offset, axisWorld);
        return offsetOnPlane.magnitude;
    }

    private Vector3 GetOrbitAxisWorld(OrbitAround orbit)
    {
        // OrbitAround calcule l'axe via center.TransformDirection(orbitAxis)
        // Ici on reproduit la même logique :
        Vector3 axis = orbit.orbitAxis;
        if (orbit.center != null)
            axis = orbit.center.TransformDirection(axis);
        if (axis.sqrMagnitude < 0.000001f) axis = Vector3.up;
        return axis.normalized;
    }

    private void CreateRing(OrbitAround orbit, float worldRadius)
    {
        GameObject go = new GameObject("Orbit_" + orbit.gameObject.name);
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one;

        // Centre du Soleil en local
        Vector3 centerLocal = transform.InverseTransformPoint(orbit.center.position);
        go.transform.localPosition = centerLocal;

        // Orientation : le ring doit être dans le plan orthogonal à l'axe de l'orbite
        // On aligne "up" local du ring sur l'axe (en local du drawer)
        Vector3 axisWorld = GetOrbitAxisWorld(orbit);
        Vector3 axisLocal = transform.InverseTransformDirection(axisWorld).normalized;
        if (axisLocal.sqrMagnitude < 0.000001f) axisLocal = Vector3.up;

        // La normale du plan = axisLocal, donc on fait une rotation qui met Vector3.up -> axisLocal
        go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, axisLocal);

        // LineRenderer
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = loop;
        lr.positionCount = segments;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = lineMaterial;

        // Convertir le rayon WORLD en rayon LOCAL du drawer
        // On prend un facteur basé sur la moyenne du scale XY du drawer (robuste)
        Vector3 s = transform.lossyScale;
        float scaleAvg = (Mathf.Abs(s.x) + Mathf.Abs(s.z)) * 0.5f;
        if (scaleAvg <= 0.0001f) scaleAvg = 1f;
        float localRadius = worldRadius / scaleAvg;

        // yExtraOffset : dans l’espace local du ring (après rotation)
        float yLocal = yExtraOffset;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments;
            float angle = t * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * localRadius;
            float z = Mathf.Sin(angle) * localRadius;

            // le cercle est dans le plan XZ du ring (puis rotation du ring fait le plan réel)
            lr.SetPosition(i, new Vector3(x, yLocal, z));
        }
    }
}
