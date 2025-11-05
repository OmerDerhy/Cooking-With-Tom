using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;

    void Awake()
    {
        // אם כבר קיים MusicPlayer, תהרוס את זה (כדי שלא יהיה כפול)
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // שומר את האובייקט גם במעבר סצנות
    }
}