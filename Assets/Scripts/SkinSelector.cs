using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SkinSelector : MonoBehaviour
{
    private const string KeyIndex = "SelectedSkinIndex";
    private const string KeyName  = "SelectedSkinName";

    [Header("Prefabs (Gameplay)")]
    public GameObject[] skinPrefabs;

    [Header("Default")]
    [Tooltip("האינדקס של הסקין הדיפולטי שיוצג תמיד (גם אם לא נקנה)")]
    public int defaultIndex = 0;

    [Header("UI Preview (Image in Canvas)")]
    public Image previewImage;

    [Header("Preview Size")]
    public bool autoSizeToCanvas = true;
    [Range(0.1f, 1.5f)] public float sizePercentOfCanvas = 0.7f;
    public Vector2 fixedPreviewSize = new Vector2(700, 700);

    [Header("Swipe")]
    public float swipeThresholdPixels = 80f;
    public bool enableMouseSwipeInEditor = true;

    [Header("Slide Animation")]
    public float slideDuration = 0.18f;
    public bool slideDistanceRelative = true;
    [Range(0.1f, 1f)] public float slideDistanceFactor = 0.35f;
    public float slideDistancePixels = 600f;

    [Header("Confirm")]
    public Button confirmButton; // לא חובה – אבל אם יש, נשתמש בו ל-confirm

    // מצב פנימי
    private List<int> ownedIndices = new List<int>();  // אינדקסים לתוך skinPrefabs
    private int ownedCursor = 0;                       // מצביע בתוך ownedIndices
    private bool isAnimating;
    private Vector2 dragStart;
    private bool tracking;
    private bool sizeDirty;

    void Start()
    {
        if (skinPrefabs == null || skinPrefabs.Length == 0)
        {
            Debug.LogError("[SkinSelector] skinPrefabs ריק.", this);
            enabled = false; return;
        }
        if (!previewImage)
        {
            Debug.LogError("[SkinSelector] previewImage לא מחובר.", this);
            enabled = false; return;
        }

        // הגדרות UI בסיסיות
        var rt = previewImage.rectTransform;
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        previewImage.preserveAspect = true;

        // ודא דיפולטי חוקי וסמן אותו כ-owned
        defaultIndex = Mathf.Clamp(defaultIndex, 0, skinPrefabs.Length - 1);
        var def = skinPrefabs[defaultIndex];
        if (def != null) SkinOwnership.MarkOwnedByName(def.name);

        ApplyPreviewSize();

        // בנה את רשימת הסקינים שבבעלות
        RebuildOwnedList();

        // מיקום התחלתי לפי שמירה מהעבר (שם קודם), ואם לא – על הדיפולטי
        string savedName = PlayerPrefs.GetString(KeyName, "");
        if (!string.IsNullOrEmpty(savedName))
        {
            int i = ownedIndices.FindIndex(idx => skinPrefabs[idx] && skinPrefabs[idx].name == savedName);
            ownedCursor = (i >= 0) ? i : Mathf.Max(0, ownedIndices.FindIndex(idx => idx == defaultIndex));
        }
        else
        {
            ownedCursor = Mathf.Max(0, ownedIndices.FindIndex(idx => idx == defaultIndex));
        }

        // הצג את הנוכחי
        UpdatePreviewFromCurrent();
        rt.anchoredPosition = Vector2.zero;

        // כפתור Confirm תמיד פעיל כאן (אין סקינים נעולים בתצוגה),
        // אבל אם מחובר – נוודא שהוא interactable
        if (confirmButton) confirmButton.interactable = true;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (enableMouseSwipeInEditor) HandleMouse();
#endif
        HandleTouch();

        if (!isAnimating && sizeDirty)
        {
            ApplyPreviewSize();
            sizeDirty = false;
            previewImage.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    // ניתן לקרוא מבחוץ (למשל אחרי חזרה מהחנות)
    public void RefreshOwnedSkins()
    {
        int currentPrefabIndex = GetCurrentPrefabIndex();
        string currentName = (currentPrefabIndex >= 0 && skinPrefabs[currentPrefabIndex]) ? skinPrefabs[currentPrefabIndex].name : "";

        RebuildOwnedList();

        // נסה להישאר על אותו סקין אם עדיין בבעלות, אחרת חזור לדיפולטי
        int stay = ownedIndices.FindIndex(idx => skinPrefabs[idx] && skinPrefabs[idx].name == currentName);
        if (stay >= 0) ownedCursor = stay;
        else ownedCursor = Mathf.Max(0, ownedIndices.FindIndex(idx => idx == defaultIndex));

        UpdatePreviewFromCurrent();
    }

    private void RebuildOwnedList()
    {
        ownedIndices.Clear();

        for (int i = 0; i < skinPrefabs.Length; i++)
        {
            var p = skinPrefabs[i];
            if (!p) continue;

            if (i == defaultIndex || SkinOwnership.IsOwnedByName(p.name))
                ownedIndices.Add(i);
        }

        if (ownedIndices.Count == 0)
        {
            // ביטחון: אם משום מה אין – דאג שלפחות הדיפולט בפנים
            ownedIndices.Add(defaultIndex);
        }
    }

    public void MarkSizeDirty() => sizeDirty = true;

    // ---------- API ----------
    public void Next()
    {
        if (isAnimating || ownedIndices.Count <= 1) return;
        int next = (ownedCursor + 1) % ownedIndices.Count;
        StartCoroutine(SlideTo(next, directionLeft: true));
    }

    public void Prev()
    {
        if (isAnimating || ownedIndices.Count <= 1) return;
        int prev = (ownedCursor - 1 + ownedIndices.Count) % ownedIndices.Count;
        StartCoroutine(SlideTo(prev, directionLeft: false));
    }

    public void ConfirmSelection()
    {
        if (isAnimating) return;
        int prefabIndex = GetCurrentPrefabIndex();
        if (prefabIndex < 0) return;

        var chosen = skinPrefabs[prefabIndex];
        string name = chosen ? chosen.name : "";

        // אין צורך לבדוק נעילה – כאן מוצגים רק owned + דיפולט
        PlayerPrefs.SetInt(KeyIndex, prefabIndex);
        PlayerPrefs.SetString(KeyName, name);
        PlayerPrefs.Save();

        Debug.Log($"[SkinSelector] Saved index={prefabIndex}, name={name}");
        SceneManager.LoadScene("Main");
    }

    // ---------- Helpers ----------
    private int GetCurrentPrefabIndex()
    {
        if (ownedIndices.Count == 0) return -1;
        int idx = Mathf.Clamp(ownedCursor, 0, ownedIndices.Count - 1);
        return ownedIndices[idx];
    }

    private void UpdatePreviewFromCurrent()
    {
        int prefabIndex = GetCurrentPrefabIndex();
        if (prefabIndex < 0) return;

        var prefab = skinPrefabs[prefabIndex];
        Sprite s = prefab ? prefab.GetComponentInChildren<SpriteRenderer>(true)?.sprite : null;
        previewImage.sprite = s;
        previewImage.rectTransform.anchoredPosition = Vector2.zero;

        // בסיס: קבע גודל רגיל לפי ההגדרות
        ApplyPreviewSize();

        // חריגה ייעודית: Bear(0) גדול יותר
        if (prefab && prefab.name == "Bear(0)")
        {
            previewImage.rectTransform.sizeDelta *= 2.25f; // <<--- שנה את המקדם אם תרצה
        }

        // confirmButton תמיד פעיל (כי אין נעולים בתצוגה)
        if (confirmButton) confirmButton.interactable = true;
    }

    private void ApplyPreviewSize()
    {
        var rt = previewImage.rectTransform;

        if (autoSizeToCanvas)
        {
            var root = rt.root as RectTransform;
            float canvasW = root ? root.rect.width : Screen.width;
            float canvasH = root ? root.rect.height : Screen.height;
            float size = Mathf.Min(canvasW, canvasH) * sizePercentOfCanvas;
            rt.sizeDelta = new Vector2(size, size);
        }
        else
        {
            rt.sizeDelta = fixedPreviewSize;
        }
    }

    private float CurrentSlideDistance()
    {
        if (slideDistanceRelative)
        {
            var root = previewImage.rectTransform.root as RectTransform;
            float canvasW = root ? root.rect.width : Screen.width;
            return canvasW * slideDistanceFactor;
        }
        return slideDistancePixels;
    }

    // ---------- Input ----------
    private void HandleTouch()
    {
        if (isAnimating || Input.touchCount == 0) return;
        var t = Input.GetTouch(0);

        switch (t.phase)
        {
            case TouchPhase.Began:
                dragStart = t.position;
                tracking = true;
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (!tracking) break;
                Vector2 delta = t.position - dragStart;
                tracking = false;
                TryCommitSwipe(delta.x);
                break;
        }
    }

#if UNITY_EDITOR
    private Vector2 mouseStart;
    private void HandleMouse()
    {
        if (isAnimating) return;

        if (Input.GetMouseButtonDown(0))
        {
            mouseStart = Input.mousePosition;
            tracking = true;
        }
        else if (Input.GetMouseButtonUp(0) && tracking)
        {
            Vector2 delta = (Vector2)Input.mousePosition - mouseStart;
            tracking = false;
            TryCommitSwipe(delta.x);
        }
    }
#endif

    private void TryCommitSwipe(float dx)
    {
        if (Mathf.Abs(dx) < swipeThresholdPixels) return;
        if (dx < 0f) Next(); else Prev();
    }

    // ---------- Animation ----------
    private IEnumerator SlideTo(int newOwnedCursor, bool directionLeft)
    {
        isAnimating = true;

        RectTransform rt = previewImage.rectTransform;
        Vector2 center = Vector2.zero;
        float dist = CurrentSlideDistance();

        Vector2 offOut = center + new Vector2(directionLeft ? -dist : dist, 0f);
        Vector2 offIn  = center + new Vector2(directionLeft ?  dist : -dist, 0f);

        // א. החוצה
        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / slideDuration;
            rt.anchoredPosition = Vector2.Lerp(center, offOut, k);
            yield return null;
        }

        // מחליפים את הסקין לחדש (owned בלבד)
        ownedCursor = Mathf.Clamp(newOwnedCursor, 0, ownedIndices.Count - 1);
        UpdatePreviewFromCurrent();

        // מציבים בצד הנגדי ומחליקים פנימה
        rt.anchoredPosition = offIn;
        t = 0f;
        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = t / slideDuration;
            rt.anchoredPosition = Vector2.Lerp(offIn, center, k);
            yield return null;
        }

        rt.anchoredPosition = center;
        isAnimating = false;
    }
}
