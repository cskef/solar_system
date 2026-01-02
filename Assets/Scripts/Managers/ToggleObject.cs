using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public GameObject target;

    public void Toggle()
    {
        if (target == null) return;
        target.SetActive(!target.activeSelf);
    }
}

