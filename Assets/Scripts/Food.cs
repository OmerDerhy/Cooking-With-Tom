using System.Collections;
using UnityEngine;

public class Food : MonoBehaviour
{
    private Face face;

    private bool isDragging = false;
    private int draggingFingerId = -1;
    private Camera mainCamera;
    private bool isOverFace = false;
    private bool isOverBlender = false;
    public bool FinalBlender;
    public bool FinalOven;
    public bool FinalMicroWave;
    
    public AudioClip clickSound;   // הצליל שיושמע
    private AudioSource audioSource;
    private bool isPlaying = false;
    private bool isOverTrash = false;

    // חדש: הגדרות התכווצות
    [Header("Shrink Settings")]
    [SerializeField] private float shrinkDuration = 0.3f;
    [SerializeField] private float endScale = 0.0f; // 0 = נעלם לגמרי
    private bool isShrinking = false;

    private Rigidbody2D rb2d;
    private Collider2D col2d;

    void Awake()
    {
        if (face == null)
            face = FindObjectOfType<Face>();

        rb2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>();
    }

    void Start()
    {
        mainCamera = Camera.main;
        if (rb2d) rb2d.freezeRotation = true;

        // ודא שיש AudioSource באובייקט
        audioSource = gameObject.AddComponent<AudioSource>();
        clickSound = Resources.Load<AudioClip>("FoodClick");
        audioSource.playOnAwake = false;   // לא לנגן אוטומטית
        audioSource.spatialBlend = 0f;     // 2D sound (רצוי ל־UI/קליקים)
        audioSource.loop = false;
    }

    void Update()
    {
        if (isShrinking) return; // בזמן התכווצות לא מגיבים לקלט

        // טאץ'
        foreach (Touch touch in Input.touches)
        {
            int layerMask = ~LayerMask.GetMask("TriggerOnly"); // התעלם מ-TriggerOnly
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMask);
            Vector3 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0f;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        isDragging = true;
                        if (face != null) face.OpenMouth();
                        draggingFingerId = touch.fingerId;

                        // 🔊 נגן צליל לחיצה במובייל
                        if (clickSound != null && audioSource != null)
                            audioSource.PlayOneShot(clickSound);
                    }
                    break;

                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isDragging && touch.fingerId == draggingFingerId)
                        transform.position = touchPosition;
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (touch.fingerId == draggingFingerId)
                    {
                        isDragging = false;
                        if (face != null) face.CloseMouth();
                        draggingFingerId = -1;

                        if (isOverFace)
                        {
                            if (face != null) face.Taste(gameObject.tag);
                            StartCoroutine(ShrinkAndDestroy());
                        }
                        if(isOverTrash)
                        {
                            Destroy(gameObject);
                        }
                    }
                    break;
            }
        }

#if UNITY_EDITOR
        // עכבר
        int layerMaskEditor = ~LayerMask.GetMask("TriggerOnly");
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, layerMaskEditor);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                if (face != null) face.OpenMouth();

                // 🔊 נגן צליל לחיצה בעכבר (Editor)
                if (clickSound != null && audioSource != null)
                    audioSource.PlayOneShot(clickSound);
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
            transform.position = mousePos;

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            if (face != null) face.CloseMouth();

            if (isOverFace)
            {
                if (face != null) face.Taste(gameObject.tag);
                StartCoroutine(ShrinkAndDestroy());
            }
            if(isOverTrash)
            {
                Destroy(gameObject);
            }
        }
#endif
    }

    public bool getIsDragging()
    {
        return isDragging;
    }

    void OnTriggerEnter2D(Collider2D target)
    {
        if (target.CompareTag("Face"))
            isOverFace = true;
        if(target.CompareTag("Trash"))
            isOverTrash = true;
    }

    void OnTriggerExit2D(Collider2D target)
    {
        if (target.CompareTag("Face"))
            isOverFace = false;
        if (target.CompareTag("Trash"))
            isOverTrash = false;
    }

    // ====== חדש: התכווצות לפני השמדה ======
    private IEnumerator ShrinkAndDestroy()
    {
        if (isShrinking) yield break; // הגנה מכפל
        isShrinking = true;

        // נטרל פיזיקה וקוליידר כדי שלא יהיו התנגשויות/גרירות בזמן האפקט
        if (rb2d)
        {
            rb2d.linearVelocity = Vector2.zero;
            rb2d.angularVelocity = 0f;
            rb2d.simulated = false;
        }
        if (col2d) col2d.enabled = false;

        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one * endScale;
        float t = 0f;

        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / shrinkDuration);
            transform.localScale = Vector3.Lerp(startScale, targetScale, k);
            yield return null;
        }

        Destroy(gameObject);
    }
}
