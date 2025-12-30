using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    public Transform center;                 // Soleil
    public float orbitSpeed = 10f;           // degrés/seconde
    public Vector3 orbitAxis = Vector3.up;   // axe Y

    [Header("Radius")]
    public bool lockRadius = true;
    public float radius = 0f;                // si 0 → calcul auto au Start

    private void Start()
    {
        if (center == null) return;

        // Si radius pas défini, on calcule à partir de la position actuelle
        if (radius <= 0f)
        {
            Vector3 offset = transform.position - center.position;
            offset.y = 0f; // on travaille dans le plan XZ
            radius = offset.magnitude;
        }

        // On snap la planète exactement sur le cercle au démarrage
        SnapToRadius();
    }

    private void Update()
    {
        if (center == null) return;

        transform.RotateAround(center.position, orbitAxis.normalized, orbitSpeed * Time.deltaTime);

        if (lockRadius)
            SnapToRadius();
    }

    private void SnapToRadius()
    {
        Vector3 offset = transform.position - center.position;
        offset.y = 0f;

        if (offset == Vector3.zero)
            offset = Vector3.right;

        Vector3 dir = offset.normalized;
        transform.position = new Vector3(
            center.position.x + dir.x * radius,
            transform.position.y,
            center.position.z + dir.z * radius
        );
    }
}
