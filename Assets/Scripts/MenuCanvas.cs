using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public class MenuCanvas : MonoBehaviour
{
    [SerializeField] private Canvas canvas;          // גרור את ה-Canvas אם תרצה (לא חובה)
    [SerializeField] private bool forceOverlayIfNoCamera = true;
    [SerializeField] private int sortingOrder = 100; // לשים אותך למעלה במקרה של קנבסים אחרים

    void Awake()
    {
        // ודא קנבס
        if (!canvas) canvas = GetComponentInChildren<Canvas>(true);
        if (!canvas) return;

        // ודא שיש מצלמה/מצב ציור תקפים
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            if (canvas.worldCamera == null)
                canvas.worldCamera = Camera.main;

            if (!canvas.worldCamera && forceOverlayIfNoCamera)
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        else if (canvas.renderMode == RenderMode.WorldSpace)
        {
            if (canvas.worldCamera == null)
                canvas.worldCamera = Camera.main;
        }

        // סדר ציור גבוה כדי שלא יכוסה
        canvas.sortingOrder = sortingOrder;

        // ודא שקיימת EventSystem אחת
        EnsureEventSystem();

        // בטיחות: לוודא שלא נשארנו בפאוז
        if (Time.timeScale == 0f) Time.timeScale = 1f;

        // בטיחות: CanvasGroup (אם יש) לא שקוף
        var cg = canvas.GetComponent<CanvasGroup>();
        if (cg)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        // ודא שהמצלמה מציירת את שכבת ה-UI
        var cam = canvas.worldCamera ? canvas.worldCamera : Camera.main;
        if (cam)
        {
            var uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer >= 0)
            {
                int uiMask = 1 << uiLayer;
                if ((cam.cullingMask & uiMask) == 0)
                    cam.cullingMask |= uiMask;
            }
        }
    }

    private void EnsureEventSystem()
    {
        var es = FindObjectOfType<EventSystem>();
        if (!es)
        {
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(go); // אופציונלי: אם יש מעבר בין סצנות לעיתים קרובות
        }
        else
        {
            // אם יש כמה (למשל כי טעינתם Additive), השאר רק אחד פעיל
            var all = FindObjectsOfType<EventSystem>();
            for (int i = 1; i < all.Length; i++)
                all[i].gameObject.SetActive(false);
        }
    }
}
