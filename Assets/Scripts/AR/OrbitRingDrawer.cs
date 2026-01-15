using UnityEngine;
using System.Collections;
using System.Linq;

public class OrbitRingDrawer : MonoBehaviour
{
    public OrbitAround[] planetOrbits;
    public OrbitKepler[] keplerOrbits;

    [Header("Render Settings")]
    public int segments = 160;
    public float lineWidth = 0.004f;
    public float yExtraOffset = 0f;
    public bool loop = true;

    [Header("Material (optional)")]
    public Material lineMaterial;

    private void Awake()
    {
        if (lineMaterial == null)
            lineMaterial = new Material(Shader.Find("Sprites/Default"));
    }

    public void RebuildNextFrame() => StartCoroutine(RebuildCoroutine());

    private IEnumerator RebuildCoroutine()
    {
        yield return null;
        RebuildNow();
    }

    private void ClearRings()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
    }

    public void RebuildNow()
    {
        if (!gameObject.activeInHierarchy) return;

        ClearRings();

        bool keplerOn = (KeplerModeController.Instance != null && KeplerModeController.Instance.keplerOn);

        if (keplerOn)
        {
            if (keplerOrbits == null || keplerOrbits.Length == 0)
                keplerOrbits = FindObjectsByType<OrbitKepler>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            var valid = keplerOrbits.Where(k => k != null && k.center != null).ToArray();
            foreach (var k in valid) CreateKeplerEllipseRing(k);
            return;
        }

        if (planetOrbits == null || planetOrbits.Length == 0)
            planetOrbits = FindObjectsByType<OrbitAround>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        var validC = planetOrbits
            .Where(o => o != null && o.center != null)
            .OrderBy(o => GetWorldRadiusOnOrbitPlane(o))
            .ToArray();

        foreach (var o in validC)
        {
            float r = GetWorldRadiusOnOrbitPlane(o);
            if (r <= 0.0001f) continue;
            CreateCircleRing(o, r);
        }
    }

    // ----- CIRCLE -----
    private float GetWorldRadiusOnOrbitPlane(OrbitAround orbit)
    {
        Vector3 axisWorld = GetOrbitAxisWorld(orbit.center, orbit.orbitAxis);
        Vector3 offset = orbit.transform.position - orbit.center.position;
        Vector3 offsetOnPlane = Vector3.ProjectOnPlane(offset, axisWorld);
        return offsetOnPlane.magnitude;
    }

    private Vector3 GetOrbitAxisWorld(Transform center, Vector3 axisLocal)
    {
        Vector3 axis = axisLocal;
        if (center != null) axis = center.TransformDirection(axis);
        if (axis.sqrMagnitude < 0.000001f) axis = Vector3.up;
        return axis.normalized;
    }

    private LineRenderer SetupLine(GameObject go)
    {
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = loop;
        lr.positionCount = segments;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = lineMaterial;
        return lr;
    }

    private float WorldToLocal(float worldLen)
    {
        Vector3 s = transform.lossyScale;
        float scaleAvg = (Mathf.Abs(s.x) + Mathf.Abs(s.z)) * 0.5f;
        if (scaleAvg <= 0.0001f) scaleAvg = 1f;
        return worldLen / scaleAvg;
    }

    private void CreateCircleRing(OrbitAround orbit, float worldRadius)
    {
        GameObject go = new GameObject("Orbit_" + orbit.gameObject.name);
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one;

        Vector3 centerLocal = transform.InverseTransformPoint(orbit.center.position);
        go.transform.localPosition = centerLocal;

        Vector3 axisWorld = GetOrbitAxisWorld(orbit.center, orbit.orbitAxis);
        Vector3 axisLocal = transform.InverseTransformDirection(axisWorld).normalized;
        if (axisLocal.sqrMagnitude < 0.000001f) axisLocal = Vector3.up;
        go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, axisLocal);

        LineRenderer lr = SetupLine(go);

        float rLocal = WorldToLocal(worldRadius);
        float yLocal = yExtraOffset;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments;
            float a = t * Mathf.PI * 2f;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * rLocal, yLocal, Mathf.Sin(a) * rLocal));
        }
    }

    // ----- KEPLER ELLIPSE -----
    private void CreateKeplerEllipseRing(OrbitKepler k)
    {
        GameObject go = new GameObject("KeplerOrbit_" + k.gameObject.name);
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one;

        Vector3 centerLocal = transform.InverseTransformPoint(k.center.position);
        go.transform.localPosition = centerLocal;

        // 1) aligne le plan de l'orbite (normal)
        Vector3 axisWorld = k.GetAxisWorldPublic();
        Vector3 axisLocal = transform.InverseTransformDirection(axisWorld).normalized;
        if (axisLocal.sqrMagnitude < 0.000001f) axisLocal = Vector3.up;
        go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, axisLocal);

        // 2) aligne l’axe X de l’ellipse sur uWorld (même repère que la planète)
        Vector3 uWorld = k.GetUWorld();
        Vector3 uLocal = go.transform.InverseTransformDirection(uWorld);
        uLocal.y = 0f;
        if (uLocal.sqrMagnitude < 0.000001f) uLocal = Vector3.right;

        float yaw = Mathf.Atan2(uLocal.z, uLocal.x) * Mathf.Rad2Deg;
        go.transform.localRotation = go.transform.localRotation * Quaternion.Euler(0f, yaw, 0f);

        LineRenderer lr = SetupLine(go);

        float aWorld = Mathf.Max(0.0001f, k.semiMajorAxis);
        float bWorld = Mathf.Max(0.0001f, k.GetSemiMinorAxis());

        float aLocal = WorldToLocal(aWorld);
        float bLocal = WorldToLocal(bWorld);

        float yLocal = yExtraOffset;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / segments;
            float ang = t * Mathf.PI * 2f;

            float x = aLocal * Mathf.Cos(ang);
            float z = bLocal * Mathf.Sin(ang);

            lr.SetPosition(i, new Vector3(x, yLocal, z));
        }
    }
}
