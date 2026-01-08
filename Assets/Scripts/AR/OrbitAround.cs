using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    public Transform center;                 // Soleil
    public float orbitSpeed = 10f;           // degrés/seconde
    public Vector3 orbitAxis = Vector3.up;   // axe de rotation (local)

    [Header("Radius")]
    public bool lockRadius = true;
    public float radius = 0f;                // si 0 → calcul auto

    private Vector3 AxisWorld => (center != null)
        ? center.TransformDirection(orbitAxis).normalized
        : orbitAxis.normalized;

    private void OnEnable()
    {
        // recalcul à chaque activation (utile après placement / vuforia)
        RecalculateRadius();
        SnapToRadius();
    }

    private void Update()
    {
        if (center == null) return;

        // rotation autour du soleil selon l’axe du soleil (pas world up)
        transform.RotateAround(center.position, AxisWorld, orbitSpeed * Time.deltaTime);

        if (lockRadius)
            SnapToRadius();
    }

    public void RecalculateRadius()
    {
        if (center == null) return;

        if (radius <= 0f)
        {
            Vector3 offset = transform.position - center.position;

            // On projette l’offset sur le plan orthogonal à l’axe (Kepler simple)
            Vector3 offsetOnPlane = Vector3.ProjectOnPlane(offset, AxisWorld);

            radius = offsetOnPlane.magnitude;
            if (radius < 0.0001f) radius = 0.1f;
        }
    }

    private void SnapToRadius()
    {
        if (center == null) return;

        Vector3 offset = transform.position - center.position;
        Vector3 offsetOnPlane = Vector3.ProjectOnPlane(offset, AxisWorld);

        if (offsetOnPlane.sqrMagnitude < 0.000001f)
            offsetOnPlane = Vector3.ProjectOnPlane(Vector3.right, AxisWorld);

        Vector3 dir = offsetOnPlane.normalized;

        // Position EXACTE sur le plan + rayon
        transform.position = center.position + dir * radius;
    }
}
