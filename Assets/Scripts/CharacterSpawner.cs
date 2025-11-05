using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    private const string Key = "SelectedSkinIndex";

    [Tooltip("אותם Prefabs של הסקינים, באותו סדר כמו במסך הבחירה")]
    public GameObject[] skinPrefabs;

    public Transform spawnPoint; // אופציונלי; אם ריק נשתמש ב-transform של האובייקט

    void Start()
    {
        if (skinPrefabs == null || skinPrefabs.Length == 0)
        {
            Debug.LogError("[CharacterSpawner] אין Prefabs מחוברים.", this);
            return;
        }

        int idx = Mathf.Clamp(PlayerPrefs.GetInt(Key, 0), 0, skinPrefabs.Length - 1);
        var prefab = skinPrefabs[idx];
        if (!prefab)
        {
            Debug.LogError($"[CharacterSpawner] prefab חסר לאינדקס {idx}.", this);
            return;
        }

        var pos = spawnPoint ? spawnPoint.position : transform.position;
        var rot = spawnPoint ? spawnPoint.rotation : transform.rotation;

        Instantiate(prefab, pos, rot);
    }
}