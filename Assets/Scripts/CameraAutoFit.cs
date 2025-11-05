using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAutoFit : MonoBehaviour
{
    public float referenceAspectWidth = 20f;   // יחס בסיסי – אם המשחק בלנדסקייפ
    public float referenceAspectHeight = 9f;

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        // יחס יעד
        float targetAspect = referenceAspectWidth / referenceAspectHeight;
        // יחס מסך בפועל
        float windowAspect = (float)Screen.width / (float)Screen.height;

        if (windowAspect >= targetAspect)
        {
            // המסך רחב יותר – נחתוך בצדדים
            cam.orthographicSize = referenceAspectHeight / 2f;
        }
        else
        {
            // המסך גבוה יותר – נחתוך מלמעלה/למטה
            float differenceInSize = targetAspect / windowAspect;
            cam.orthographicSize = (referenceAspectHeight / 2f) * differenceInSize;
        }
    }
}