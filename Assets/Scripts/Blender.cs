
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blender : MonoBehaviour
{
    public Food orangeJuicePrefab;
    public Food PickeldApples;
    public Food Wine;
    public Food Smoothie;
    public Food Milkshake;
    public Food Powder;
    public Food FailDish;
    
    public AudioClip clickSound;   // הצליל שיושמע
    private AudioSource audioSource;
    public AudioClip clickBlender;
    private AudioSource audioBlender;
    
    private bool ChocolateIn = false;
    private bool MilkIn = false;
    private bool AppleIn = false;
    private bool BananaIn = false;
    private bool EggIn = false;

    private Animator animator;
    private bool canReleaseFood = true;
    private string[] FoodIn = new string[2];
    private int foodCount = 0; // כדי לדעת איפה לשים את הרכיב הבא

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        clickSound = Resources.Load<AudioClip>("FoodIn");
        audioBlender = gameObject.AddComponent<AudioSource>();
        clickBlender = Resources.Load<AudioClip>("Blender");
    }

    private void ResetFlags()
    {
        MilkIn = false;
        BananaIn = false;
        ChocolateIn = false;
        EggIn = false;
        AppleIn = false;

        // איפוס רשימת המאכלים
        FoodIn[0] = null;
        FoodIn[1] = null;
        foodCount = 0;
    }

    private void AddFoodTag(string tag)
    {
        if (foodCount < FoodIn.Length)
        {
            FoodIn[foodCount] = tag;
            foodCount++;
        }
    }

    IEnumerator PlayAnimationAndRelease(System.Action releaseAction)
    {
        animator.SetInteger("anim", 1);   // מפעיל את האנימציה
        audioBlender.PlayOneShot(clickBlender);
        canReleaseFood = false;

        yield return new WaitForSeconds(5f); 

        animator.SetInteger("anim", 0);   // עוצר את האנימציה
        releaseAction?.Invoke();          // משחרר את המאכל החדש
        canReleaseFood = true;
    }

    void OnTriggerStay2D(Collider2D target)
    {
        if (!target.TryGetComponent<Food>(out Food foodScript)) return;
        if (foodScript.FinalBlender) return;
        if (foodScript.getIsDragging()) return;
        
        if (!canReleaseFood) return; // לא מאפשר להכניס בזמן עבודה

        Vector3 spawnPosition = new Vector3(
            target.transform.position.x,
            target.transform.position.y + 2f,
            target.transform.position.z
        );
        if (foodCount == 2)
        {
            Destroy(target.gameObject);
            Instantiate(FailDish, transform.position, Quaternion.identity);
            ResetFlags();
            return;
        }
        if (foodCount == 1)
        {

            // === אם כבר יש חלב בפנים ===
            if (MilkIn)
            {
                AddFoodTag(target.tag);
                if (target.CompareTag("Banana"))
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food newFood = Instantiate(Smoothie, spawnPosition, Quaternion.identity);
                        newFood.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
                else if (target.CompareTag("Chocolate"))
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food newFood = Instantiate(Milkshake, spawnPosition, Quaternion.identity);
                        newFood.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
                else if (target.CompareTag("Egg"))
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food newFood = Instantiate(Powder, spawnPosition, Quaternion.identity);
                        newFood.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
                else
                {
                    // מרכיב שלא מתאים לחלב → FailDish
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food fail = Instantiate(FailDish, spawnPosition, Quaternion.identity);
                        fail.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
            }

            // === אם יש כבר Banana בפנים ===
            if (BananaIn)
            {
                AddFoodTag(target.tag);
                if (target.CompareTag("Milk"))
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food newFood = Instantiate(Smoothie, spawnPosition, Quaternion.identity);
                        newFood.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
                else
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food fail = Instantiate(FailDish, spawnPosition, Quaternion.identity);
                        fail.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
            }

            // === אם יש כבר Chocolate בפנים ===
            if (ChocolateIn)
            {
                AddFoodTag(target.tag);
                if (target.CompareTag("Milk"))
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food newFood = Instantiate(Milkshake, spawnPosition, Quaternion.identity);
                        newFood.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
                else
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food fail = Instantiate(FailDish, spawnPosition, Quaternion.identity);
                        fail.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
            }

            // === אם יש כבר Egg בפנים ===
            if (EggIn)
            {
                AddFoodTag(target.tag);
                if (target.CompareTag("Milk"))
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food newFood = Instantiate(Powder, spawnPosition, Quaternion.identity);
                        newFood.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
                else
                {
                    Destroy(target.gameObject);
                    audioSource.PlayOneShot(clickSound);
                    StartCoroutine(PlayAnimationAndRelease(() =>
                    {
                        Food fail = Instantiate(FailDish, spawnPosition, Quaternion.identity);
                        fail.FinalBlender = true;
                        ResetFlags();
                    }));
                    return;
                }
            }
        }

        // === הכנסת רכיב ראשון ===
        if (target.CompareTag("Milk")) {audioSource.PlayOneShot(clickSound); MilkIn = true; AddFoodTag("Milk"); Destroy(target.gameObject); return; }
        if (target.CompareTag("Banana")) { audioSource.PlayOneShot(clickSound);BananaIn = true; AddFoodTag("Banana"); Destroy(target.gameObject); return; }
        if (target.CompareTag("Chocolate")) {audioSource.PlayOneShot(clickSound); ChocolateIn = true; AddFoodTag("Chocolate"); Destroy(target.gameObject); return; }
        if (target.CompareTag("Egg")) { audioSource.PlayOneShot(clickSound);EggIn = true; AddFoodTag("Egg"); Destroy(target.gameObject); return; }

        if (foodCount == 0)
        {


            // === מתכונים חד־שלביים ===
            if (target.CompareTag("Apple"))
            {
                AddFoodTag("Apple");
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(PlayAnimationAndRelease(() =>
                {
                    Food newFood = Instantiate(PickeldApples, spawnPosition, Quaternion.identity);
                    newFood.FinalBlender = true;
                }));
                ResetFlags();
                return;
            }

            if (target.CompareTag("Berries"))
            {
                AddFoodTag("Berries");
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(PlayAnimationAndRelease(() =>
                {
                    Food newFood = Instantiate(Wine, spawnPosition, Quaternion.identity);
                    newFood.FinalBlender = true;
                }));
                ResetFlags();
                return;
                ResetFlags();
            }

            if (target.CompareTag("Orange"))
            {
                AddFoodTag("Orange");
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(PlayAnimationAndRelease(() =>
                {
                    Food newFood = Instantiate(orangeJuicePrefab, spawnPosition, Quaternion.identity);
                    newFood.FinalBlender = true;
                }));
                ResetFlags();
                return;
                ResetFlags();
            }
        }

        // ❌ מרכיב לא מתאים בכלל
        AddFoodTag(target.tag);
        Destroy(target.gameObject);
        audioSource.PlayOneShot(clickSound);
        StartCoroutine(PlayAnimationAndRelease(() =>
        {
            Food fail = Instantiate(FailDish, spawnPosition, Quaternion.identity);
            fail.FinalBlender = true;
            ResetFlags();
        }));
    }
}
