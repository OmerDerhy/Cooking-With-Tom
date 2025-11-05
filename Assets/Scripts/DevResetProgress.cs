using UnityEngine;
using UnityEngine.SceneManagement;

public class DevResetProgress : MonoBehaviour
{
    public GameObject[] skinPrefabs;

    [ContextMenu("Reset Progress")]
    public void ResetAll()
    {
        // מחיקה מלאה
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // אופציונלי: הרצה מחדש של הסצנה
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    [ContextMenu("Reset Ownership Only")]
    public void ResetOwnershipOnly()
    {
        foreach (var p in skinPrefabs)
        {
            if (!p) continue;
            PlayerPrefs.DeleteKey("skin_owned_" + p.name);
        }
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}