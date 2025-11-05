using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MarketManager : MonoBehaviour
{
    [Header("Skins & Prices (same order)")]
    public GameObject[] skinPrefabs;
    public int[] coinPrices;                  // למחירי מטבעות (אם IAP=false)
    [Tooltip("סמן true עבור סקינים שנקנים בכסף אמיתי (IAP). אותו סדר כמו skinPrefabs.")]
    public bool[] isIAP;                      // <- חדש

    [Header("UI")]
    public Button[] buyButtons;               // אותו סדר כמו skinPrefabs
    public Text[] priceLabels;                // אופציונלי
    public Text coinsLabel;                   // אופציונלי
    public int defaultPrice = 2000;

    [Header("After Purchase")]
    public bool returnToSelectorOnBuy = false;
    public string skinSelectorSceneName = "SkinSelectorScene";

    [Header("Defaults")]
    public bool ownFirstSkinByDefault = false;
    public int startingCoinsIfMissing = 5000;

    private const string CoinsKey = "Coins";

    private void Start()
    {
        if (skinPrefabs == null || skinPrefabs.Length == 0)
        {
            Debug.LogError("[Market] skinPrefabs empty.");
            return;
        }

        EnsureCoinsInitialized();

        if (ownFirstSkinByDefault && skinPrefabs[0] != null && GetPriceForIndex(0) <= 0)
            MarkOwned(skinPrefabs[0].name);

        // חיבור מאזינים רק לפריטי Coins; פריטי IAP מנוהלים ע"י IAP Button בנפרד
        for (int i = 0; i < skinPrefabs.Length && i < buyButtons.Length; i++)
        {
            int idx = i;
            if (buyButtons[i] == null) continue;

            buyButtons[i].onClick.RemoveAllListeners();

            bool useIAP = (isIAP != null && i < isIAP.Length && isIAP[i]);
            if (!useIAP)
            {
                buyButtons[i].onClick.AddListener(() => BuyWithCoins(idx));
            }
            // אם זה IAP – אל תחבר מאזין כאן; ה־IAP Button על אותו כפתור יבצע את הרכישה
        }

        RefreshUI();
    }

    private void EnsureCoinsInitialized()
    {
        if (!PlayerPrefs.HasKey(CoinsKey))
        {
            PlayerPrefs.SetInt(CoinsKey, Mathf.Max(0, startingCoinsIfMissing));
            PlayerPrefs.Save();
        }
    }

    // --- Coins ---
    private int GetCoins() => PlayerPrefs.GetInt(CoinsKey, Mathf.Max(0, startingCoinsIfMissing));
    private bool TrySpendCoins(int amount)
    {
        amount = Mathf.Max(0, amount);
        int c = GetCoins();
        if (c < amount) return false;
        PlayerPrefs.SetInt(CoinsKey, c - amount);
        PlayerPrefs.Save();
        return true;
    }

    // --- Ownership (לפי הסכמה שלך) ---
    private static string OwnedKey(string skinName) => "skin_owned_" + skinName;
    private bool IsOwned(string skinName) => PlayerPrefs.GetInt(OwnedKey(skinName), 0) == 1;
    private void MarkOwned(string skinName)
    {
        PlayerPrefs.SetInt(OwnedKey(skinName), 1);
        PlayerPrefs.Save();
    }

    private int GetPriceForIndex(int index)
    {
        if (coinPrices != null && index >= 0 && index < coinPrices.Length && coinPrices[index] > 0)
            return coinPrices[index];
        return Mathf.Max(0, defaultPrice);
    }

    private void SetButtonText(Button btn, string text)
    {
        if (!btn) return;
        var t = btn.GetComponentInChildren<Text>(true);
        if (t) t.text = text;
    }

    private void RefreshUI()
    {
        if (coinsLabel != null) coinsLabel.text = GetCoins().ToString();

        for (int i = 0; i < skinPrefabs.Length; i++)
        {
            var prefab = skinPrefabs[i];
            string name = prefab ? prefab.name : $"Skin_{i}";
            bool owned = IsOwned(name);
            bool useIAP = (isIAP != null && i < isIAP.Length && isIAP[i]);
            int coinPrice = GetPriceForIndex(i);

            // price label
            if (priceLabels != null && i < priceLabels.Length && priceLabels[i] != null)
            {
                if (owned)
                    priceLabels[i].text = useIAP ? "Purchased" : "Owned";
                else
                    priceLabels[i].text = useIAP ? "Buy" : coinPrice.ToString();
            }

            // buy button state & text
            if (buyButtons != null && i < buyButtons.Length && buyButtons[i] != null)
            {
                var btn = buyButtons[i];

                if (owned)
                {
                    btn.interactable = false;
                    SetButtonText(btn, useIAP ? "PURCHASED" : "Owned");
                }
                else
                {
                    if (useIAP)
                    {
                        // IAP item: ה־IAP Button ינהל את הרכישה; רק טקסט נוח
                        btn.interactable = true; // ניתן ללחוץ לרכישה
                        SetButtonText(btn, "BUY");
                    }
                    else
                    {
                        // Coins item
                        bool canAfford = GetCoins() >= coinPrice;
                        btn.interactable = canAfford;
                        SetButtonText(btn, coinPrice + "C");
                    }
                }
            }
        }
    }

    // --- Coins purchase flow ---
    public void BuyWithCoins(int index)
    {
        if (skinPrefabs == null || index < 0 || index >= skinPrefabs.Length) return;
        var prefab = skinPrefabs[index];
        if (!prefab) return;

        string name = prefab.name;
        if (IsOwned(name))
        {
            RefreshUI();
            return;
        }

        // לא לאפשר קנייה במטבעות לפריט IAP בטעות
        bool useIAP = (isIAP != null && index < isIAP.Length && isIAP[index]);
        if (useIAP)
        {
            Debug.LogWarning($"[Market] '{name}' מוגדר כ-IAP. הרכישה מתבצעת דרך IAP Button, לא במטבעות.");
            RefreshUI();
            return;
        }

        int price = GetPriceForIndex(index);
        if (price <= 0)
        {
            MarkOwned(name);
            RefreshUI();
            if (returnToSelectorOnBuy && !string.IsNullOrEmpty(skinSelectorSceneName))
                SceneManager.LoadScene(skinSelectorSceneName);
            return;
        }

        if (!TrySpendCoins(price))
        {
            Debug.Log("[Market] Not enough coins.");
            RefreshUI();
            return;
        }

        MarkOwned(name);
        RefreshUI();

        if (returnToSelectorOnBuy && !string.IsNullOrEmpty(skinSelectorSceneName))
            SceneManager.LoadScene(skinSelectorSceneName);
    }

    // --- IAP purchase completion hooks (לקרוא מה-IAP Button) ---
    /// <summary>
    /// לקרוא מ-On Purchase Complete של IAP Button (ע"י Name).
    /// </summary>
    public void OnIAPPurchaseCompleteByName(string purchasedSkinName)
    {
        if (string.IsNullOrEmpty(purchasedSkinName)) return;
        MarkOwned(purchasedSkinName);
        RefreshUI();
    }

    /// <summary>
    /// לקרוא מ-On Purchase Complete של IAP Button (ע"י אינדקס).
    /// </summary>
    public void OnIAPPurchaseCompleteByIndex(int index)
    {
        if (skinPrefabs == null || index < 0 || index >= skinPrefabs.Length) return;
        var prefab = skinPrefabs[index];
        if (!prefab) return;
        MarkOwned(prefab.name);
        RefreshUI();
    }
}
