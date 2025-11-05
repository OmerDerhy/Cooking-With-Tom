using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{

    [Tooltip("שם הסצנה לטעינה")]
    public string sceneName;

    [Tooltip("זמן המתנה בשניות לפני המעבר")]
    public float delaySeconds = 3f;

    void Start()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    private System.Collections.IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        SceneManager.LoadScene(sceneName);
    }
}
