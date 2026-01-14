using UnityEngine;
using TMPro;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }

    [Header("Current Time Scale")]
    [SerializeField] private float timeScale = 1f;
    public float TimeScale => timeScale;

    [Header("Optional UI")]
    public TextMeshProUGUI timeLabel; //  "x1", "x5", "Pause" 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetSpeed(float speed)
    {
        timeScale = Mathf.Max(0f, speed);
        UpdateLabel();
    }

    public void Pause()
    {
        timeScale = 0f;
        UpdateLabel();
    }

    public void ResumeNormal()
    {
        timeScale = 1f;
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (timeLabel == null) return;

        if (timeScale <= 0f) timeLabel.text = "Pause";
        else timeLabel.text = $"x{timeScale:0}";
    }
}
