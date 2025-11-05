using UnityEngine;

public class CookieRain : MonoBehaviour
{
    [Header("🍪 Cookie Settings")]
    public GameObject cookiePrefab;      // פריפאב של העוגייה (UI או World)
    public Transform parentTransform;    // Canvas או Panel
    public float spawnInterval = 0.5f;   // כל כמה שניות נוצרת עוגייה
    public float spawnXMin = -500f;      // גבול שמאלי
    public float spawnXMax = 500f;       // גבול ימני
    public float spawnY = 600f;          // גובה ההתחלה של העוגיות
    public float fallSpeed = 50f;        // מהירות הנפילה
    public float rotateSpeed = 100f;     // מהירות הסיבוב (degrees/sec)

    void Start()
    {
        InvokeRepeating(nameof(SpawnCookie), 0f, spawnInterval);
    }

    void SpawnCookie()
    {
        if (cookiePrefab == null) return;

        float randomX = Random.Range(spawnXMin, spawnXMax);

        GameObject cookie = Instantiate(cookiePrefab, new Vector3(randomX, spawnY, 0f), Quaternion.identity);

        if (parentTransform != null)
            cookie.transform.SetParent(parentTransform, false);

        // הוספת קומפוננט לנפילה וסיבוב
        CookieBehavior behavior = cookie.AddComponent<CookieBehavior>();
        behavior.fallSpeed = fallSpeed;
        behavior.rotateSpeed = rotateSpeed;
    }
}



public class CookieBehavior : MonoBehaviour
{
    public float fallSpeed = 100f;    // מהירות נפילה ב-pixels/sec
    public float rotateSpeed = 100f;  // מהירות סיבוב ב-degrees/sec
    private RectTransform rt;
    public float bottomLimit = -600f; // הגבול התחתון (כאשר העוגייה נמוכה יותר מזה - נהרסת)

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        if (rt == null)
            rt = gameObject.AddComponent<RectTransform>();
    }

    void Update()
    {
        // נפילה איטית יותר (UI coordinates)
        rt.anchoredPosition += Vector2.down * fallSpeed * Time.deltaTime;

        // סיבוב סביב עצמו
        rt.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);

        // בדיקה אם העוגייה יצאה מתחת לגבול
        if (rt.anchoredPosition.y < bottomLimit)
        {
            Destroy(gameObject);
        }
    }
}

