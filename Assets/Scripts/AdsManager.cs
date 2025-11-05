// using System;
// using System.Threading.Tasks;
// using UnityEngine;
// using Unity.Services.Core;
// using Unity.Services.Mediation;
//
// public class AdsManager : MonoBehaviour
// {
//     [Header("Ad Unit IDs")]
//     [SerializeField] private string androidAdUnitId = "Interstitial_Android"; // מה־Unity Dashboard
//     [SerializeField] private string iOSAdUnitId = "Interstitial_iOS";
//
//     private IInterstitialAd interstitialAd;
//     private string adUnitId;
//
//     async void Start()
//     {
//         // אתחול Unity Services
//         await UnityServices.InitializeAsync();
//
// #if UNITY_ANDROID
//         adUnitId = androidAdUnitId;
// #elif UNITY_IOS
//         adUnitId = iOSAdUnitId;
// #endif
//
//         // יצירת המודעה
//         interstitialAd = MediationService.Instance.CreateInterstitialAd(adUnitId);
//
//         // רישום לאירועים
//         interstitialAd.OnLoaded += OnAdLoaded;
//         interstitialAd.OnFailedLoad += OnAdFailedToLoad;
//         interstitialAd.OnShowed += OnAdShowed;
//         interstitialAd.OnFailedShow += OnAdFailedToShow;
//         interstitialAd.OnClosed += OnAdClosed;
//
//         // טעינה ראשונית
//         await LoadAdAsync();
//     }
//
//     // --- טעינת מודעה אסינכרונית ---
//     public async Task LoadAdAsync()
//     {
//         try
//         {
//             Debug.Log("Loading interstitial ad...");
//             await interstitialAd.LoadAsync();
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"Failed to load interstitial ad: {e.Message}");
//         }
//     }
//
//     // --- הצגת מודעה אסינכרונית ---
//     public async Task ShowAdAsync()
//     {
//         if (interstitialAd.AdState == AdState.Loaded)
//         {
//             try
//             {
//                 Debug.Log("Showing interstitial ad...");
//                 await interstitialAd.ShowAsync();
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError($"Failed to show interstitial ad: {e.Message}");
//             }
//         }
//         else
//         {
//             Debug.Log("Ad not ready, reloading...");
//             await LoadAdAsync();
//         }
//     }
//
//     // --- פונקציה רגילה לכפתור ---
//     public void OnShowAdButtonPressed()
//     {
//         // מפעילים את הקריאה האסינכרונית בלי להמתין לה
//         _ = ShowAdAsync();
//     }
//
//     // --- Callbacks ---
//     private void OnAdLoaded(object sender, EventArgs e)
//     {
//         Debug.Log("Ad loaded successfully!");
//     }
//
//     private void OnAdFailedToLoad(object sender, LoadErrorEventArgs e)
//     {
//         Debug.LogError($"Failed to load ad: {e.Message}");
//     }
//
//     private void OnAdShowed(object sender, EventArgs e)
//     {
//         Debug.Log("Ad is now showing!");
//     }
//
//     private void OnAdFailedToShow(object sender, ShowErrorEventArgs e)
//     {
//         Debug.LogError($"Failed to show ad: {e.Message}");
//     }
//
//     private void OnAdClosed(object sender, EventArgs e)
//     {
//         Debug.Log("Ad closed, reloading...");
//         _ = LoadAdAsync(); // טען מחדש לאחר סגירה
//     }
// }
