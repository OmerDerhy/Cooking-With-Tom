using UnityEngine;
using UnityEngine.UI;

public class IAPBlockIfOwned : MonoBehaviour
{
    [Tooltip("שם הסקין בדיוק כמו prefab.name, למשל Panda(0)")]
    public string skinName;

    [Tooltip("הכפתור (אם ריק ניקח את זה שעל האובייקט)")]
    public Button button;

    [Tooltip("טקסט הכפתור (אופציונלי)")]
    public Text label;

    private const string OwnedPrefix = "skin_owned_";

    void Reset()
    {
        button = GetComponent<Button>();
        label  = GetComponentInChildren<Text>(true);
    }

    void OnEnable() => Refresh();

    public void Refresh()
    {
        if (string.IsNullOrEmpty(skinName)) return;

        bool owned = PlayerPrefs.GetInt(OwnedPrefix + skinName, 0) == 1;
        if (owned)
        {
            if (button) button.interactable = false;
            if (label)  label.text = "PURCHASED";
        }
    }
}