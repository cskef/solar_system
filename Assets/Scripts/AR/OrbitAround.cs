using UnityEngine;

public class OrbitAround : MonoBehaviour
{
    public Transform center;                 // Soleil
    [Tooltip("Degrees per second")]
    public float orbitSpeed = 10f;           // révolution
    public Vector3 orbitAxis = Vector3.up;   // axe d'orbite

    private void Update()
    {
        if (center == null) return;

        transform.RotateAround(center.position, orbitAxis.normalized, orbitSpeed * Time.deltaTime);
    }
}

