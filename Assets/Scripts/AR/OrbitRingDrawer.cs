using UnityEngine;

public class OrbitRingDrawer : MonoBehaviour
{
    [System.Serializable]
    public class Ring
    {
        public string name;
        public float radius = 0.5f;  // en Unity units (mètres)
    }

    [Header("Orbit Center")]
    public Transform center;                 // Soleil

    [Header("Rings")]
    public Ring[] rings;

    [Header("Render Settings")]
    public int segments = 128;               // plus = plus rond
    public float lineWidth = 0.005f;         // épaisseur
    public bool loop = true;

    private void Start()
    {
        if (center == null)
        {
            var sunObj = GameObject.Find("Sun");
            if (sunObj != null) center = sunObj.transform;
        }

        if (center == null)
        {
            Debug.LogError("OrbitRingDrawer: Sun not found. Name it 'Sun' or assign center.");
            return;
        }

        DrawAllRings();
    }

    public void DrawAllRings()
    {
        // Nettoyage si relancé
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (var ring in rings)
        {
            CreateRing(ring.name, ring.radius);
        }
    }

    private void CreateRing(string ringName, float radius)
    {
        GameObject go = new GameObject(ringName);
        go.transform.SetParent(transform);
        go.transform.position = center.position;

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = loop;
        lr.positionCount = segments;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        // Matériau simple (Unity built-in)
        lr.material = new Material(Shader.Find("Sprites/Default"));

        for (int i = 0; i < segments; i++)
        {
            float angle = (float)i / segments * Mathf.PI * 2f;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            Vector3 pos = new Vector3(center.position.x + x, center.position.y, center.position.z + z);
            lr.SetPosition(i, pos);
        }
    }
}
