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

    public void RebuildNextFrame()
    {
        StartCoroutine(RebuildCoroutine());
    }

    private IEnumerator RebuildCoroutine()
    {
        yield return null; // attendre 1 frame après placement/activation
        RebuildNow();
    }

    public void RebuildNow()
    {
        if (planetOrbits == null || planetOrbits.Length == 0)
            planetOrbits = FindObjectsOfType<OrbitAround>(true);

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
            .OrderBy(o => GetWorldRadius(o))
            .ToArray();

        foreach (var orbit in valid)
        {
            float worldR = GetWorldRadius(orbit);
            if (worldR <= 0.0001f) continue;

            CreateRing(orbit, worldR);
        }
    }

    // Rayon réel en WORLD (plan XZ)
    private float GetWorldRadius(OrbitAround orbit)
    {
        Vector3 offset = orbit.transform.position - orbit.center.position;
        offset.y = 0f;
        return offset.magnitude;
    }

    private void CreateRing(OrbitAround orbit, float worldRadius)
    {
        GameObject go = new GameObject("Orbit_" + orbit.gameObject.name);
        go.transform.SetParent(transform, false);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        // Centre du Soleil converti en LOCAL
        Vector3 centerLocal = transform.InverseTransformPoint(orbit.center.position);
        go.transform.localPosition = centerLocal;

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = loop;
        lr.positionCount = segments;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        //  Conversion WORLD -> LOCAL (à cause du scale du parent)
        float sx = transform.lossyScale.x;
        if (sx <= 0.0001f) sx = 1f;
        float localRadius = worldRadius / sx;

        // Y en local (différence world -> convertie)
        float yLocal = (transform.InverseTransformPoint(orbit.transform.position).y
                     - transform.InverseTransformPoint(orbit.center.position).y)
                     + yExtraOffset;

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * localRadius;
            float z = Mathf.Sin(angle) * localRadius;
            lr.SetPosition(i, new Vector3(x, yLocal, z));
        }
    }
}
