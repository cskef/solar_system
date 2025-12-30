using UnityEngine;

public class SolarSystemSetup : MonoBehaviour
{
    public Transform sunTransform;

    private void Awake()
    {
        if (sunTransform == null)
        {
            var sunObj = GameObject.Find("Sun");
            if (sunObj != null) sunTransform = sunObj.transform;
        }

        if (sunTransform == null)
        {
            Debug.LogError("SolarSystemSetup: Sun not found. Please name the Sun object 'Sun' or assign it in inspector.");
            return;
        }

        var orbits = FindObjectsOfType<OrbitAround>(true);
        foreach (var orbit in orbits)
        {
            orbit.center = sunTransform;
        }
    }
}
