using UnityEngine;

public class PandaIAPReward : MonoBehaviour
{
    [Tooltip("אופציונלי: גרור לכאן את ה-SkinSelector אם הסצנה של הבחירה פתוחה כעת")]
    public SkinSelector skinSelector;

    // חבר את הפונקציה הזו לשדה On Purchase Complete של IAP Button
    public void OnPandaPurchased()
    {
        // 1) סימון בעלות קבועה בזיכרון
        SkinOwnership.MarkOwnedByName("Panda(0)");

        // 2) אם מסך הבחירה פתוח כרגע – רענון הרשימה כדי שהפנדה תופיע מיד
        if (skinSelector != null)
        {
            skinSelector.RefreshOwnedSkins();
        }

        Debug.Log("✅ Panda(0) unlocked via IAP.");
    }
    public void OnPenguinPurchased()
    {
        // 1) סימון בעלות קבועה בזיכרון
        SkinOwnership.MarkOwnedByName("Penguin(0)");

        // 2) אם מסך הבחירה פתוח כרגע – רענון הרשימה כדי שהפנדה תופיע מיד
        if (skinSelector != null)
        {
            skinSelector.RefreshOwnedSkins();
        }

        Debug.Log("✅ Penguin(0) unlocked via IAP.");
    }
    
}