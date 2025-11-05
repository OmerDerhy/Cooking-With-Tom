using UnityEngine;

public class KitchenTool : MonoBehaviour
{
    private bool isDragging = false;
    protected bool canDrag = true; // ✅ מותר לגרור בהתחלה
    private int draggingFingerId = -1;
    private Camera mainCamera;
    private Rigidbody2D rb;

    public AudioClip clickSound;   // הצליל שיושמע
    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.bodyType = RigidbodyType2D.Dynamic; // ברירת מחדל - דינמי
        }

        audioSource = gameObject.AddComponent<AudioSource>();
        clickSound = Resources.Load<AudioClip>("FoodClick"); // קובץ תחת Assets/Resources/FoodClick.wav למשל
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // דו-ממדי
    }

    void Update()
    {
        if (!canDrag) return; 

        // טאץ'
        foreach (Touch touch in Input.touches)
        {
            Vector3 touchPosition = mainCamera.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0f;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

                    if (hit.collider != null && hit.collider.gameObject == gameObject)
                    {
                        isDragging = true;
                        draggingFingerId = touch.fingerId;

                        // 🔊 נגן צליל לחיצה
                        PlayClickSound();
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
                        draggingFingerId = -1;
                    }
                    break;
            }
        }

#if UNITY_EDITOR
        // עכבר
        if (!canDrag) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isDragging = true;

                // 🔊 נגן צליל לחיצה
                PlayClickSound();
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
            transform.position = mousePos;

        if (Input.GetMouseButtonUp(0))
            isDragging = false;
#endif
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cabinet"))
        {
            canDrag = false;
            isDragging = false;

            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Static;
            }
        }
    }

    public bool getIsDragging()
    {
        return isDragging;
    }
}
