using UnityEngine;

public class PlanetSpin : MonoBehaviour
{
    [Tooltip("Degrees per second")]
    public float rotationSpeed = 30f;

    private void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
