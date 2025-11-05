using UnityEngine;

[DisallowMultipleComponent]
public class Logo : MonoBehaviour
{
    [Header("Target (optional)")]
    [SerializeField] private Transform target;           // אם ריק - נשתמש ב-transform של האובייקט

    [Header("Breathing Settings")]
    [SerializeField, Min(0.01f)] private float periodSeconds = 3f; // זמן מחזור מלא: התרחבות+התכווצות
    [SerializeField] private float minScale = 0.95f;     // סקייל מינימלי
    [SerializeField] private float maxScale = 1.05f;     // סקייל מקסימלי
    [SerializeField] private bool useUnscaledTime = true;// ימשיך לנשום גם כשהמשחק בפאוז (Time.timeScale=0)
    [SerializeField] private bool randomizeStartPhase = true;
    [SerializeField] private bool resetOnDisable = true; // להחזיר סקייל מקורי כשמכבים את האובייקט

    private Vector3 baseScale;
    private float phase;

    void Awake()
    {
        if (!target) target = transform;
        baseScale = target.localScale;

        // לוודא ש-min <= max
        if (minScale > maxScale)
        {
            float tmp = minScale; minScale = maxScale; maxScale = tmp;
        }

        if (randomizeStartPhase)
            phase = Random.value * Mathf.PI * 2f;
    }

    void OnEnable()
    {
        if (randomizeStartPhase)
            phase = Random.value * Mathf.PI * 2f;
    }

    void Update()
    {
        float t = useUnscaledTime ? Time.unscaledTime : Time.time;
        float omega = (Mathf.PI * 2f) / Mathf.Max(0.01f, periodSeconds);

        // ערך חלק 0..1 באמצעות סינוס
        float s01 = 0.5f + 0.5f * Mathf.Sin(omega * t + phase);

        // גוזרים פקטור סקייל בין מינ' למקס'
        float k = Mathf.Lerp(minScale, maxScale, s01);

        target.localScale = baseScale * k; // סקייל אחיד משלושת הצירים
    }

    void OnDisable()
    {
        if (resetOnDisable && target)
            target.localScale = baseScale;
    }

    // אופציונלי: שינוי פרמטרים בזמן ריצה
    public void SetBreath(float min, float max, float period)
    {
        minScale = min;
        maxScale = max;
        periodSeconds = period;
    }
}
