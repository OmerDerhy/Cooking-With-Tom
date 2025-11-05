using UnityEngine;

public class Sound : MonoBehaviour
{
    public AudioClip clickSound;   // הצליל שיושמע
    private AudioSource audioSource;
    private bool isPlaying=false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnMouseDown() // מופעל כשעושים לחיצה עם העכבר על האובייקט
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void PlaySound()
    {
        if (!isPlaying && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
            isPlaying = true;
            return;
        }
        isPlaying = false;
    }
}