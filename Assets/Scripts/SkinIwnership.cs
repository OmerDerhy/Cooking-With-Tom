using UnityEngine;

public static class SkinOwnership
{
    private const string OwnedPrefix  = "skin_owned_";       // key: skin_owned_<name>
    private const string CoinsKey     = "Coins";

    // מסמן בעלות לפי שם (מועדף – יציב גם אם סדר משתנה)
    public static void MarkOwnedByName(string skinName)
    {
        if (string.IsNullOrEmpty(skinName)) return;
        PlayerPrefs.SetInt(OwnedPrefix + skinName, 1);
        PlayerPrefs.Save();
    }

    public static bool IsOwnedByName(string skinName)
    {
        if (string.IsNullOrEmpty(skinName)) return false;
        return PlayerPrefs.GetInt(OwnedPrefix + skinName, 0) == 1;
    }

    // נוחות: לפי אינדקס עם מערך הפריפאבים
    public static void MarkOwnedByIndex(GameObject[] skinPrefabs, int index)
    {
        if (skinPrefabs == null || index < 0 || index >= skinPrefabs.Length) return;
        var prefab = skinPrefabs[index];
        if (prefab) MarkOwnedByName(prefab.name);
    }

    public static bool IsOwnedByIndex(GameObject[] skinPrefabs, int index)
    {
        if (skinPrefabs == null || index < 0 || index >= skinPrefabs.Length) return false;
        var prefab = skinPrefabs[index];
        return prefab && IsOwnedByName(prefab.name);
    }

    // Coins helpers
    public static int GetCoins() => PlayerPrefs.GetInt(CoinsKey, 0);

    public static bool TrySpendCoins(int amount)
    {
        int c = GetCoins();
        if (c < amount) return false;
        PlayerPrefs.SetInt(CoinsKey, c - amount);
        PlayerPrefs.Save();
        return true;
    }
}