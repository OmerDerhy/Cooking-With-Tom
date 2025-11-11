using UnityEngine;
using UnityEngine.UI;

public class Wallet : MonoBehaviour
{
    private const string KeyCoins = "Coins";
    private const string KeyIndex = "SelectedSkinIndex";
    private const string KeyName  = "SelectedSkinName";

    [Header("Skin display (assign in Inspector)")]
    [Tooltip("אותם Prefabs של הסקינים, לפי סדר")]
    public GameObject[] skinPrefabs;

    [Tooltip("UI -> Image בקנבס שבו תוצג תמונת הסקין. אתה ממקם אותו ידנית!")]
    public Image skinPreviewImage;

    [Header("Preview options (optional)")]
    public bool preserveAspect = true;
    public bool autoSizeToCanvas = false;
    [Range(0.1f, 1f)] public float sizePercentOfCanvas = 0.5f;
    public bool ignoreParentLayout = false;

    [Header("Manual size control")]
    [Tooltip("כפול גודל התמונה. 1 = גודל רגיל, 2 = פי 2, 0.5 = חצי")]
    public float scaleFactor = 1f;

    [Header("Special case override")]
    [Tooltip("שם הסקין שיקבל גודל מיוחד (למשל Bear(0))")]
    public string specialSkinName = "Bear(0)";
    [Tooltip("מכפיל גודל נוסף לסקין המיוחד (מוכפל ב-scaleFactor)")]
    public float specialScaleMultiplier = 2.25f;

    void Awake()
    {
        // סקין דיפולטי בפעם הראשונה
        if (!PlayerPrefs.HasKey(KeyIndex))
        {
            PlayerPrefs.SetInt(KeyIndex, 0);
            PlayerPrefs.SetString(KeyName, (skinPrefabs != null && skinPrefabs.Length > 0 && skinPrefabs[0]) ? skinPrefabs[0].name : "");
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        // אתחול מטבעות בפעם הראשונה
        if (!PlayerPrefs.HasKey(KeyCoins))
        {
            PlayerPrefs.SetInt(KeyCoins, 1000);
            PlayerPrefs.Save();
        }

        UpdateSkinPreview();
    }

    /// <summary>
    /// מעדכן את תמונת הסקין על פי הבחירה השמורה (עדיפות לשם, אח"כ אינדקס).
    /// </summary>
    public void UpdateSkinPreview()
    {
        if (skinPreviewImage == null)
        {
            Debug.LogWarning("[Wallet] skinPreviewImage לא מחובר באינספקטור.");
            return;
        }
        if (skinPrefabs == null || skinPrefabs.Length == 0)
        {
            Debug.LogWarning("[Wallet] skinPrefabs ריק/לא מולא.");
            return;
        }

        string savedName = PlayerPrefs.GetString(KeyName, "");
        int idx = Mathf.Clamp(PlayerPrefs.GetInt(KeyIndex, 0), 0, skinPrefabs.Length - 1);

        GameObject prefab = null;

        // קודם ניסיון לפי שם
        if (!string.IsNullOrEmpty(savedName))
        {
            prefab = System.Array.Find(skinPrefabs, p => p && p.name == savedName);
            if (prefab != null)
                Debug.Log($"[Wallet] Found skin by name: '{savedName}'");
            else
                Debug.LogWarning($"[Wallet] Not found by name: '{savedName}', trying index {idx}");
        }

        // אם לא נמצא לפי שם — לפי אינדקס
        if (prefab == null)
        {
            if (idx >= 0 && idx < skinPrefabs.Length)
            {
                prefab = skinPrefabs[idx];
                Debug.Log($"[Wallet] Using skin by index: {idx} ({(prefab ? prefab.name : "NULL")})");
            }
        }

        // נפילה לראשון אם עדיין אין
        if (!prefab)
        {
            Debug.LogWarning("[Wallet] Selected skin not found. Falling back to first skin.");
            prefab = skinPrefabs[0];
        }

        // שליפת ספרייט
        Sprite s = prefab.GetComponentInChildren<SpriteRenderer>(true)?.sprite;
        if (!s)
        {
            Debug.LogWarning($"[Wallet] Prefab '{prefab.name}' has no SpriteRenderer/sprite.");
            return;
        }

        skinPreviewImage.preserveAspect = preserveAspect;
        skinPreviewImage.sprite = s;

        // בסיס: קביעת גודל
        var rt = skinPreviewImage.rectTransform;
        if (autoSizeToCanvas)
        {
            var root = rt.root as RectTransform;
            float canvasW = root ? root.rect.width : Screen.width;
            float canvasH = root ? root.rect.height : Screen.height;
            float size = Mathf.Min(canvasW, canvasH) * 1.2f*sizePercentOfCanvas;
            rt.sizeDelta = new Vector2(size, size);
        }

        // סקייל ברירת מחדל
        float finalScale = scaleFactor;

        // חריגה ייעודית: אם זה הסקין המיוחד (למשל Bear(0)) – תן מכפיל נוסף
        if (prefab && !string.IsNullOrEmpty(specialSkinName) && prefab.name == specialSkinName)
        {
            finalScale *= specialScaleMultiplier; // ← שנה את המכפיל באינספקטור אם צריך
        }

        rt.localScale = Vector3.one * finalScale;

        if (ignoreParentLayout)
        {
            var le = rt.GetComponent<LayoutElement>();
            if (!le) le = rt.gameObject.AddComponent<LayoutElement>();
            le.ignoreLayout = true;

            var fitter = rt.GetComponent<ContentSizeFitter>(); if (fitter) fitter.enabled = false;
            var arf    = rt.GetComponent<AspectRatioFitter>(); if (arf) arf.enabled = false;
        }

        Debug.Log($"[Wallet] Preview set: {prefab.name} (idx={idx}, savedName='{savedName}', scale={finalScale})");
    }
}
