using UnityEngine;

public class OrbitToggle : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject orbitRingsRoot;          // OrbitRings
    public OrbitRingDrawer orbitDrawer;        // OrbitRingDrawer 

    private bool isOn = true;

    private void Awake()
    {
        if (orbitRingsRoot != null)
            isOn = orbitRingsRoot.activeSelf;

        if (orbitDrawer == null && orbitRingsRoot != null)
            orbitDrawer = orbitRingsRoot.GetComponentInChildren<OrbitRingDrawer>(true);
    }

    public void ToggleOrbits()
    {
        SetOrbits(!isOn);
    }

    public void SetOrbits(bool on)
    {
        if (orbitRingsRoot == null) return;

        isOn = on;
        orbitRingsRoot.SetActive(isOn);

        // Si on active : rebuild pour être sûr que les rings existent
        if (isOn)
        {
            if (orbitDrawer == null)
                orbitDrawer = orbitRingsRoot.GetComponentInChildren<OrbitRingDrawer>(true);

            if (orbitDrawer != null)
                orbitDrawer.RebuildNextFrame();
        }
    }
}
