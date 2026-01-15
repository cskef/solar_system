using UnityEngine;

public class OrbitKepler : MonoBehaviour
{
    [Header("References")]
    public Transform center;                 // Soleil
    public Vector3 orbitAxis = Vector3.up;   // axe local du Soleil

    [Header("Kepler-like Settings (elliptic)")]
    [Range(0f, 0.9f)] public float eccentricity = 0.0167f;
    public float orbitSpeedDeg = 10f;        // base vitesse (deg/sec)
    public bool lockAFromCurrent = true;
    public float semiMajorAxis = 0f;         // a (world)

    [Header("Stability")]
    public bool lockToPlane = true;

    // Bases dans le plan orbital
    private Vector3 axisWorld;
    private Vector3 uWorld; // axe X de l’ellipse (dans le plan)
    private Vector3 vWorld; // axe Z de l’ellipse (dans le plan)

    private float theta; // paramètre (rad)

    private void OnEnable()
    {
        InitializeFromCurrent();
    }

    public void InitializeFromCurrent()
    {
        if (center == null) return;

        axisWorld = GetAxisWorld();

        // Offset dans le plan orbital
        Vector3 offset = transform.position - center.position;
        Vector3 offsetOnPlane = Vector3.ProjectOnPlane(offset, axisWorld);

        if (offsetOnPlane.sqrMagnitude < 0.000001f)
            offsetOnPlane = Vector3.ProjectOnPlane(Vector3.right, axisWorld);

        // uWorld = direction actuelle (stable)
        uWorld = offsetOnPlane.normalized;
        vWorld = Vector3.Cross(axisWorld, uWorld).normalized;

        // semiMajorAxis = distance actuelle (si demandé)
        float currentR = offsetOnPlane.magnitude;
        if (lockAFromCurrent || semiMajorAxis <= 0.0001f)
            semiMajorAxis = Mathf.Max(0.0001f, currentR);

        // b = a * sqrt(1-e^2)
        float e = Mathf.Clamp(eccentricity, 0f, 0.9f);
        float a = semiMajorAxis;
        float b = a * Mathf.Sqrt(1f - e * e);

        // Calcul theta depuis la position actuelle (ellipse paramétrique)
        // x = a cosθ ; z = b sinθ  => cosθ = x/a ; sinθ = z/b
        float x = Vector3.Dot(offsetOnPlane, uWorld);
        float z = Vector3.Dot(offsetOnPlane, vWorld);

        float cosT = (a > 0.0001f) ? (x / a) : 1f;
        float sinT = (b > 0.0001f) ? (z / b) : 0f;

        theta = Mathf.Atan2(sinT, cosT);
    }

    private void Update()
    {
        if (center == null) return;

        float tScale = (TimeController.Instance != null) ? TimeController.Instance.TimeScale : 1f;
        if (tScale <= 0f) return;

        axisWorld = GetAxisWorld();

        // Recalcule vWorld si l’axe change (AR)
        vWorld = Vector3.Cross(axisWorld, uWorld).normalized;

        float e = Mathf.Clamp(eccentricity, 0f, 0.9f);
        float a = Mathf.Max(0.0001f, semiMajorAxis);
        float b = a * Mathf.Sqrt(1f - e * e);

        // Vitesse variable "Kepler-like" : plus vite quand r est petit (approx 2e loi)
        // r ~ distance au centre dans le plan
        Vector3 posNow = center.position + uWorld * (a * Mathf.Cos(theta)) + vWorld * (b * Mathf.Sin(theta));
        float r = Vector3.ProjectOnPlane(posNow - center.position, axisWorld).magnitude;
        r = Mathf.Max(0.0001f, r);

        float baseOmega = orbitSpeedDeg * Mathf.Deg2Rad;      // rad/s
        float omega = baseOmega * (a * a) / (r * r);          // ~1/r^2
        omega = Mathf.Clamp(omega, baseOmega * 0.2f, baseOmega * 8f);

        theta += omega * tScale * Time.deltaTime;

        // Ellipse paramétrique
        Vector3 newPos = center.position
                       + uWorld * (a * Mathf.Cos(theta))
                       + vWorld * (b * Mathf.Sin(theta));

        if (lockToPlane)
        {
            Vector3 off = newPos - center.position;
            Vector3 offOnPlane = Vector3.ProjectOnPlane(off, axisWorld);
            newPos = center.position + offOnPlane;
        }

        transform.position = newPos;
    }

    private Vector3 GetAxisWorld()
    {
        Vector3 ax = orbitAxis;
        if (center != null)
            ax = center.TransformDirection(ax);
        if (ax.sqrMagnitude < 0.000001f) ax = Vector3.up;
        return ax.normalized;
    }

    // Exposés pour OrbitRingDrawer (orientation identique)
    public Vector3 GetAxisWorldPublic() => GetAxisWorld();
    public Vector3 GetUWorld() => uWorld;
    public float GetSemiMinorAxis()
    {
        float e = Mathf.Clamp(eccentricity, 0f, 0.9f);
        float a = Mathf.Max(0.0001f, semiMajorAxis);
        return a * Mathf.Sqrt(1f - e * e);
    }
}
