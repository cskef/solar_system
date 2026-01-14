using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ScaleModeController : MonoBehaviour
{
    public enum ScaleMode { Pedagogical, RealCompressedLog }

    [Header("Mode")]
    public ScaleMode currentMode = ScaleMode.Pedagogical;

    [Header("References (optional)")]
    public Transform solarSystemRoot; 
    public TextMeshProUGUI modeLabel; // affiche "Pédago" / "Réel compressé"

    [Header("Earth reference")]
    public string earthNameFR = "terre";
    public string earthNameEN = "earth";

    // Vraies valeurs de référence
    private const float EarthDiameterKm = 12742f;
    private const float EarthDistanceMillionKm = 149.6f;

    // Sauvegarde "mode pédago" (état initial)
    private readonly Dictionary<Transform, Vector3> savedLocalScales = new();
    private readonly Dictionary<OrbitAround, float> savedOrbitRadii = new();

    private OrbitAround[] orbits;
    private PlanetInfo[] planets;
    private OrbitRingDrawer ringDrawer;

    private bool initialized = false;

    private void Awake()
    {
        CacheReferences();
    }

    private void OnEnable()
    {
        // Si on revient d’un mode/scene, on recache
        CacheReferences();
    }

    private void CacheReferences()
    {
        // Récupère orbits/planets même inactifs
        orbits = FindObjectsByType<OrbitAround>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        planets = FindObjectsByType<PlanetInfo>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        ringDrawer = FindFirstObjectByType<OrbitRingDrawer>(FindObjectsInactive.Include);
    }

    /// Appel à faire quand le système solaire devient visible (Tap placé ou Vuforia track)
    public void InitializeIfNeeded()
    {
        if (initialized) return;

        if (orbits == null || orbits.Length == 0) CacheReferences();

        // Sauvegarde scales
        foreach (var p in planets)
        {
            if (p == null) continue;
            savedLocalScales[p.transform] = p.transform.localScale;
        }

        // Sauvegarde rayons "pédago"
        foreach (var o in orbits)
        {
            if (o == null || o.center == null) continue;

            // Si radius déjà défini, on le prend. Sinon on le calcule depuis la position
            float r = o.radius;
            if (r <= 0.0001f)
            {
                Vector3 axisWorld = GetAxisWorld(o);
                Vector3 offsetOnPlane = Vector3.ProjectOnPlane(o.transform.position - o.center.position, axisWorld);
                r = offsetOnPlane.magnitude;
            }
            savedOrbitRadii[o] = r;
        }

        initialized = true;
        UpdateLabel();
    }

    public void ToggleScaleMode()
    {
        if (!initialized) InitializeIfNeeded();

        currentMode = (currentMode == ScaleMode.Pedagogical)
            ? ScaleMode.RealCompressedLog
            : ScaleMode.Pedagogical;

        ApplyCurrentMode();
    }

    public void ApplyCurrentMode()
    {
        if (!initialized) InitializeIfNeeded();

        if (currentMode == ScaleMode.Pedagogical)
            ApplyPedagogical();
        else
            ApplyRealCompressedLog();

        // rebuild orbites
        if (ringDrawer != null)
            ringDrawer.RebuildNextFrame();

        UpdateLabel();
    }

    private void ApplyPedagogical()
    {
        // Restore radii
        foreach (var kv in savedOrbitRadii)
        {
            if (kv.Key == null || kv.Key.center == null) continue;

            kv.Key.radius = kv.Value;
            SnapPlanetToRadius(kv.Key, kv.Value);
        }

        // Restore scales
        foreach (var kv in savedLocalScales)
        {
            if (kv.Key == null) continue;
            kv.Key.localScale = kv.Value;
        }

        // Relancer recalcul (optionnel)
        foreach (var o in orbits)
            if (o != null) o.RecalculateRadius();
    }

    private void ApplyRealCompressedLog()
    {
        // Trouver Earth dans PlanetInfo
        PlanetInfo earth = FindEarthPlanet();
        if (earth == null)
        {
            Debug.LogWarning("ScaleModeController: Earth (Terre/Earth) not found in PlanetInfo titles. Using saved radii/scales only.");
            return;
        }

        // Base visuelle depuis le mode pédago
        // -> earthOrbitBase = rayon pédago de la Terre
        // -> earthScaleBase = scale pédago de la Terre
        OrbitAround earthOrbit = earth.GetComponent<OrbitAround>();
        if (earthOrbit == null || !savedOrbitRadii.ContainsKey(earthOrbit))
        {
            Debug.LogWarning("ScaleModeController: Earth OrbitAround not found or not saved. Using fallback values.");
        }

        float earthOrbitBase = (earthOrbit != null && savedOrbitRadii.TryGetValue(earthOrbit, out float rE)) ? rE : 1.0f;
        Vector3 earthScaleBase = savedLocalScales.TryGetValue(earth.transform, out Vector3 sE) ? sE : earth.transform.localScale;

        // 1) Distances -> affecte OrbitAround.radius + position
        foreach (var o in orbits)
        {
            if (o == null || o.center == null) continue;

            PlanetInfo info = o.GetComponent<PlanetInfo>();
            if (info == null) continue;

            float d = info.distanceToSunMillionKm; // millions km
            if (d <= 0f) continue;

            float relative = d / EarthDistanceMillionKm; // Terre=1
            float compressed = LogCompressRelative(relative); // Terre=1

            float newRadius = earthOrbitBase * compressed;
            o.radius = newRadius;
            SnapPlanetToRadius(o, newRadius);
        }

        // 2) Tailles -> affecte localScale (comparée à Terre)
        foreach (var p in planets)
        {
            if (p == null) continue;

            float dia = p.diameterKm;
            if (dia <= 0f) continue;

            float relativeSize = dia / EarthDiameterKm; // Terre=1
            float compressedSize = LogCompressRelative(relativeSize); // Terre=1

            // Terre garde son scale de base, autres = baseTerre * facteur
            p.transform.localScale = earthScaleBase * compressedSize;
        }

        // Recalibrer les rayons (pour cohérence interne si besoin)
        foreach (var o in orbits)
            if (o != null) o.RecalculateRadius();
    }

    // Compression log qui mappe 1 -> 1 :
    // f(x) = log(1+x)/log(2)
    private float LogCompressRelative(float x)
    {
        x = Mathf.Max(0f, x);
        return Mathf.Log(1f + x) / Mathf.Log(2f);
    }

    private PlanetInfo FindEarthPlanet()
    {
        // On cherche par title : "Terre" ou "Earth"
        foreach (var p in planets)
        {
            if (p == null) continue;
            string t = (p.title ?? "").Trim().ToLowerInvariant();
            if (t == earthNameFR || t == earthNameEN) return p;
        }

        // Fallback : contient le mot
        foreach (var p in planets)
        {
            if (p == null) continue;
            string t = (p.title ?? "").Trim().ToLowerInvariant();
            if (t.Contains(earthNameFR) || t.Contains(earthNameEN)) return p;
        }

        return null;
    }

    private Vector3 GetAxisWorld(OrbitAround o)
    {
        Vector3 axis = o.orbitAxis;
        if (o.center != null)
            axis = o.center.TransformDirection(axis);
        if (axis.sqrMagnitude < 0.000001f) axis = Vector3.up;
        return axis.normalized;
    }

    private void SnapPlanetToRadius(OrbitAround o, float radius)
    {
        // Même logique que OrbitAround : on place sur le plan orthogonal à l’axe
        Vector3 axisWorld = GetAxisWorld(o);

        Vector3 offset = o.transform.position - o.center.position;
        Vector3 offsetOnPlane = Vector3.ProjectOnPlane(offset, axisWorld);

        if (offsetOnPlane.sqrMagnitude < 0.000001f)
            offsetOnPlane = Vector3.ProjectOnPlane(Vector3.right, axisWorld);

        Vector3 dir = offsetOnPlane.normalized;
        o.transform.position = o.center.position + dir * radius;
    }

    private void UpdateLabel()
    {
        if (modeLabel == null) return;

        modeLabel.text = (currentMode == ScaleMode.Pedagogical)
            ? "Pédagogique"
            : "Réelle (compressée)";
    }
}
