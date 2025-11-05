using UnityEngine;
using UnityEngine.UI; // או TMPro אם אתה משתמש ב-TextMeshPro

public class CoinsUIManager : MonoBehaviour
{
    public Text coinsText; // חבר כאן את Text מ-Inspector

    void Start()
    {
        UpdateCoinsUI();
    }

    public void UpdateCoinsUI()
    {
        int currentCoins = PlayerPrefs.GetInt("Coins", 0);
        coinsText.text = " "+ currentCoins;
    }
}