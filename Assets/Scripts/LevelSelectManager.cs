using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Buttons & Icons (for this screen)")]
    public Button[] levelButtons;       // הכפתורים בדף הזה בלבד (לפי סדר)
    public GameObject[] lockIcons;      // איקוני מנעול תואמים לכל כפתור
    public GameObject[] Stars;          // כוכבים מלאים – שלבים שהושלמו
    public GameObject[] UnFillStars;    // כוכבים ריקים – השלב הבא הפתוח

    [Header("Paging")]
    [Tooltip("מספר השלב הראשון שמופיע במסך הזה. לדוגמה: 1 למסך הראשון, 9 למסך השני.")]
    public int firstLevelNumberInThisScreen = 1;

    [Tooltip("כמה שלבים מוצגים במסך הזה (לרוב = מספר הכפתורים).")]
    public int levelsOnThisScreen = 8;

    void Start()
    {
        // HighestLevel = השלב הבא הפתוח (1 כבר פתוח כברירת מחדל)
        int nextOpenLevel = PlayerPrefs.GetInt("HighestLevel", 1);

        // לוודא שלא נחרוג ממספר הכפתורים בפועל
        int count = Mathf.Min(levelButtons != null ? levelButtons.Length : 0, levelsOnThisScreen);

        for (int i = 0; i < count; i++)
        {
            int levelNumber = firstLevelNumberInThisScreen + i;

            // כיבוי ברירת מחדל
            SafeSet(Stars, i, false);
            SafeSet(UnFillStars, i, false);
            SafeSet(lockIcons, i, false);

            // סטטוס לפי HighestLevel
            if (levelNumber < nextOpenLevel)
            {
                // הושלם
                SetInteractable(i, true);
                SafeSet(Stars, i, true);
            }
            else if (levelNumber == nextOpenLevel)
            {
                // השלב הבא הפתוח
                SetInteractable(i, true);
                SafeSet(UnFillStars, i, true);
            }
            else
            {
                // נעול
                SetInteractable(i, false);
                SafeSet(lockIcons, i, true);
            }

            // מאזינים ללחיצה
            int capturedLevel = levelNumber;
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LoadLevel(capturedLevel));
        }

        // במקרה שיש יותר אייקונים/כוכבים ממספר הכפתורים בשימוש – נכבה אותם
        DisableUnusedVisualsBeyond(count, Stars);
        DisableUnusedVisualsBeyond(count, UnFillStars);
        DisableUnusedVisualsBeyond(count, lockIcons);
    }

    private void SetInteractable(int i, bool interactable)
    {
        if (levelButtons != null && i < levelButtons.Length && levelButtons[i] != null)
            levelButtons[i].interactable = interactable;
    }

    private void SafeSet(GameObject[] arr, int i, bool on)
    {
        if (arr != null && i < arr.Length && arr[i] != null)
            arr[i].SetActive(on);
    }

    private void DisableUnusedVisualsBeyond(int fromIndex, GameObject[] arr)
    {
        if (arr == null) return;
        for (int i = fromIndex; i < arr.Length; i++)
            if (arr[i] != null) arr[i].SetActive(false);
    }

    private void LoadLevel(int levelNumber)
    {
        // טוען סצנה בשם "LevelX" (Level1, Level2, ...)
        SceneManager.LoadScene("Level" + levelNumber);
    }
}
