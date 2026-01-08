using UnityEngine;

public class KeepSolarSystemHorizontal : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public Transform imageTarget;

    [Header("Rotation")]
    public bool keepYaw = true; // garde la rotation Y (tourner autour du marker)

    private void LateUpdate()
    {
        if (imageTarget == null) return;

        // Suivre la position du marker
        transform.position = imageTarget.position;

        if (keepYaw)
        {
            // Garde uniquement la rotation autour de Y (yaw), annule inclinaisons (pitch/roll)
            float yaw = imageTarget.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
        else
        {
            // Toujours horizontal, orientation fixe
            transform.rotation = Quaternion.identity;
        }
    }
}
