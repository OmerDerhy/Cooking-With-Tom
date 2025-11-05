using UnityEngine;

public class PerformanceSettings : MonoBehaviour
{
    void Awake()
    {
        // מניעת ירידת ביצועים
        Application.targetFrameRate = 60; // או 120 אם המכשיר תומך
        QualitySettings.vSyncCount = 0;   // מבטל VSync (חשוב בנייד)
    }
}