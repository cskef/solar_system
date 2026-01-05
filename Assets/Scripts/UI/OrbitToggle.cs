using UnityEngine;

public class OrbitToggle : MonoBehaviour
{
    public GameObject orbitRingsRoot;
    private bool isOn = true;

    public void ToggleOrbits()
    {
        if (orbitRingsRoot == null) return;

        isOn = !isOn;
        orbitRingsRoot.SetActive(isOn);
    }

    public void SetOrbits(bool on)
    {
        if (orbitRingsRoot == null) return;
        isOn = on;
        orbitRingsRoot.SetActive(isOn);
    }
}
