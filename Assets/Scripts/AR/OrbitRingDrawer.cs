using UnityEngine;
using System.Linq;
using System.Collections;

public class OrbitRingDrawer : MonoBehaviour
{
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
        if (lineMaterial == null)
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    public void RebuildNextFrame()
    {
        StartCoroutine(RebuildCoroutine());
    }

    private IEnumerator RebuildCoroutine()
    {
        yield return null;
        RebuildNow();
    }

    public void ClearRings()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    public void RebuildNow()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (planetOrbits == null || planetOrbits.Length == 0)
            planetOrbits = FindObjectsByType<OrbitAround>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        ClearRings();

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

    private float GetWorldRadiusOnOrbitPlane(OrbitAround orbit)
    {
        Vector3 axisWorld = GetOrbitAxisWorld(orbit);
        Vector3 offset = orbit.transform.position - orbit.center.position;
        Vector3 offsetOnPlane = Vector3.ProjectOnPlane(offset, axisWorld);
        return offsetOnPlane.magnitude;
    }

    private Vector3 GetOrbitAxisWorld(OrbitAround orbit)
    {
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

        Vector3 centerLocal = transform.InverseTransformPoint(orbit.center.position);
        go.transform.localPosition = centerLocal;

        Vector3 axisWorld = GetOrbitAxisWorld(orbit);
        Vector3 axisLocal = transform.InverseTransformDirection(axisWorld).normalized;
        if (axisLocal.sqrMagnitude < 0.000001f) axisLocal = Vector3.up;

        go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, axisLocal);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = loop;
        lr.positionCount = segments;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = lineMaterial;

        Vector3 s = transform.lossyScale;
        float scaleAvg = (Mathf.Abs(s.x) + Mathf.Abs(s.z)) * 0.5f;
        if (scaleAvg <= 0.0001f) scaleAvg = 1f;
        float localRadius = worldRadius / scaleAvg;

        float yLocal = yExtraOffset;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments;
            float angle = t * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * localRadius;
            float z = Mathf.Sin(angle) * localRadius;

            lr.SetPosition(i, new Vector3(x, yLocal, z));
        }
    }
}
