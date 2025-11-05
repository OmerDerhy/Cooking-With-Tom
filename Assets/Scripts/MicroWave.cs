using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroWave : MonoBehaviour
{
    public Food Pudding;      
    public Food FailDish;     
    public Food Sandwich;     
    public Food CreamPuff;    
    public Food CupCake;

    public AudioClip clickSound;
    public AudioClip FoodClick;
    private AudioSource audioFood;
    private AudioSource audioSource;
    
    private bool MilkIn = false;
    private bool ChocolateIn = false;
    private bool BreadIn = false;
    private bool MeatIn = false;
    private bool PowderIn = false;
    
    private int foodCount = 0;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        clickSound = Resources.Load<AudioClip>("MicroWaveSound"); 
        audioFood = gameObject.AddComponent<AudioSource>();
        FoodClick = Resources.Load<AudioClip>("FoodIn");
    }

    private void ResetFlags()
    {
        MilkIn = false;
        ChocolateIn = false;
        BreadIn = false;
        MeatIn = false;
        PowderIn = false;
        foodCount = 0;
    }

    void OnTriggerStay2D(Collider2D target)
    {
        if (!target.TryGetComponent<Food>(out Food foodScript)) return;
        if (foodScript.FinalMicroWave) return;
        if (foodScript.getIsDragging()) return;

        Vector3 spawnPosition = new Vector3(
            target.transform.position.x,
            target.transform.position.y + 2f,
            target.transform.position.z
        );

        if (foodCount.Equals(2))
        {
            Destroy(target.gameObject);
            audioSource.PlayOneShot(clickSound);
            StartCoroutine(SpawnAfterDelay(FailDish, spawnPosition, 4.5f));
            ResetFlags();
            return;
        }

        if (foodCount.Equals(1))
        {
            // מתכון: Milk + Chocolate -> Pudding
            if (MilkIn && target.CompareTag("Chocolate") || 
                ChocolateIn && target.CompareTag("Milk"))
            {
                ResetFlags();
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(SpawnAfterDelay(Pudding, spawnPosition, 4.5f));
                return;
            }

            // מתכון: Bread + Meat -> Sandwich
            if (BreadIn && target.CompareTag("Meat") || 
                MeatIn && target.CompareTag("Bread"))
            {
                ResetFlags();
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(SpawnAfterDelay(Sandwich, spawnPosition, 4.5f));
                return;
            }

            // מתכון חדש: Milk + Bread -> CreamPuff
            if (MilkIn && target.CompareTag("Bread") || 
                BreadIn && target.CompareTag("Milk"))
            {
                ResetFlags();
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(SpawnAfterDelay(CreamPuff, spawnPosition, 4.5f));
                return;
            }

            // מתכון: Milk + Powder -> CupCake
            if (MilkIn && target.CompareTag("Powder") || 
                PowderIn && target.CompareTag("Milk"))
            {
                ResetFlags();
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(SpawnAfterDelay(CupCake, spawnPosition, 4.5f));
                return;
            }
        }

        if (foodCount.Equals(0))
        {
            // הכנסת רכיב ראשון
            if (target.CompareTag("Milk")) { MilkIn = true; foodCount++; DestroyAndSound(target); return; }
            if (target.CompareTag("Powder")) { PowderIn= true; foodCount++; DestroyAndSound(target); return; }
            if (target.CompareTag("Chocolate")) { ChocolateIn = true; foodCount++; DestroyAndSound(target); return; }
            if (target.CompareTag("Bread")) { BreadIn = true; foodCount++; DestroyAndSound(target); return; }
            if (target.CompareTag("Meat")) { MeatIn = true; foodCount++; DestroyAndSound(target); return; }
        }

        // מרכיב לא מתאים
        Destroy(target.gameObject);
        audioSource.PlayOneShot(clickSound);
        StartCoroutine(SpawnAfterDelay(FailDish, spawnPosition, 4.5f));
        ResetFlags();
    }

    // 📌 פונקציה שמחכה X שניות ואז מייצרת אוכל
    private IEnumerator SpawnAfterDelay(Food prefab, Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);
        Food newFood = Instantiate(prefab, position, Quaternion.identity);
        newFood.FinalMicroWave = true;
    }

    // 📌 פונקציה עזר להרוס + לנגן צליל הכנסת אוכל
    private void DestroyAndSound(Collider2D target)
    {
        Destroy(target.gameObject);
        audioFood.PlayOneShot(FoodClick);
    }
}
