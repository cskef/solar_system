using UnityEngine;

public class KeepHorizontal : MonoBehaviour
{
    public Transform target; // ImageTarget

    void LateUpdate()
    {
        if (target == null) return;

        // garde seulement le X, annule Y/Z (pitch/roll)
        Vector3 e = target.eulerAngles;
        transform.rotation = Quaternion.Euler(e.y, 0f, 0f);
        transform.position = target.position;
    }
}
