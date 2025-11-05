using UnityEngine;

public class SetLayer : MonoBehaviour
{
    void Start()
    {
        // קובע את ה-layer ל-4 בהתחלה
        gameObject.layer = 4;
    }

    void Update()
    {
        // מוודא שהוא נשאר ב-layer 4
        if (gameObject.layer != 4)
        {
            gameObject.layer = 4;
        }
    }
}