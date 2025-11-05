using System.Collections;
using UnityEngine;

public class Oven : MonoBehaviour
{
    public Food Steak;
    public Food ApplePie;
    public Food MeatPie;
    public Food Cheesecake;
    public Food Cake;        // פריפאב חדש ל-Cake
    public Food Pizza;       // פריפאב חדש ל-Pizza
    public Food FailDish;
    
    public AudioClip clickSound;   // הצליל שיושמע
    private AudioSource audioSource;
    public AudioClip clickOven;
    private AudioSource audioOven;

    private Animator animator;
    private bool PowderIn = false;
    private bool PickledApplesIn = false;
    private bool CheeseIn = false;
    private bool ChocolateIn = false;
    private bool BreadIn = false;   

    private bool isCooking = false; // מונע הפעלה כפולה
    private string[] FoodIn = new string[2];
    private int foodCount = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator = GetComponent<Animator>();
        audioSource = gameObject.AddComponent<AudioSource>();
        clickSound = Resources.Load<AudioClip>("FoodIn");
        audioOven = gameObject.AddComponent<AudioSource>();
        clickOven = Resources.Load<AudioClip>("OvenSound");
    }

    private void ResetIngredients()
    {
        PowderIn = false;
        PickledApplesIn = false;
        CheeseIn = false;
        ChocolateIn = false;
        BreadIn = false;  
        
        FoodIn[0] = null;
        FoodIn[1] = null;
        foodCount = 0;
    }

    private IEnumerator CookFood(Food foodPrefab, Vector3 spawnPosition)
    {
        isCooking = true;

        animator.SetInteger("anim", 1);
        audioOven.PlayOneShot(clickOven);
        yield return new WaitForSeconds(2f);
        animator.SetInteger("anim", 0);

        Food newFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
        newFood.FinalOven = true;

        ResetIngredients();
        isCooking = false;
    }

    void OnTriggerStay2D(Collider2D target)
    {
        if (!target.TryGetComponent<Food>(out Food foodScript)) return;
        if (foodScript.FinalOven) return;
        if (foodScript.getIsDragging()) return;
        if (isCooking) return;

        Vector3 spawnPosition = new Vector3(
            target.transform.position.x,
            target.transform.position.y + 2f,
            target.transform.position.z
        );

        if (foodCount == 2)
        {
            Destroy(target.gameObject);
            Instantiate(FailDish, transform.position, Quaternion.identity);
            ResetIngredients();
            return;
        }

        if (foodCount == 1)
        {

            // מתכון Cake - Chocolate + Powder
            if ((PowderIn && target.CompareTag("Chocolate")) || (ChocolateIn && target.CompareTag("Powder")))
            {
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(CookFood(Cake, spawnPosition));
                ResetIngredients();
                return;
            }

            // מתכון Pizza - Bread + Cheese
            if ((BreadIn && target.CompareTag("Cheese")) || (CheeseIn && target.CompareTag("Bread")))
            {
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(CookFood(Pizza, spawnPosition));
                ResetIngredients();
                return;
            }

            // שאר המתכונים קיימים:
            if ((PowderIn && target.CompareTag("PickledApples")) ||
                (PickledApplesIn && target.CompareTag("Powder")))
            {
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(CookFood(ApplePie, spawnPosition));
                ResetIngredients();
                return;
            }

            if (PowderIn && target.CompareTag("Meat"))
            {
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(CookFood(MeatPie, spawnPosition));
                ResetIngredients();
                return;
            }

            if ((PowderIn && target.CompareTag("Cheese")) ||
                (CheeseIn && target.CompareTag("Powder")))
            {
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(CookFood(Cheesecake, spawnPosition));
                ResetIngredients();
                return;
            }
        }

        // הכנסת מרכיבים ראשונים
        if (target.CompareTag("Powder")) {audioSource.PlayOneShot(clickSound); PowderIn = true;
            foodCount++;Destroy(target.gameObject); return; }
        if (target.CompareTag("PickledApples")) {audioSource.PlayOneShot(clickSound); PickledApplesIn = true;foodCount++; Destroy(target.gameObject); return; }
        if (target.CompareTag("Cheese")) {audioSource.PlayOneShot(clickSound); CheeseIn = true;foodCount++; Destroy(target.gameObject); return; }
        if (target.CompareTag("Chocolate")) {audioSource.PlayOneShot(clickSound); ChocolateIn = true;foodCount++; Destroy(target.gameObject); return; }
        if (target.CompareTag("Bread")) {audioSource.PlayOneShot(clickSound); BreadIn = true;foodCount++; Destroy(target.gameObject); return; }

        if (foodCount == 0)
        {

            // בשר - סטייק
            if (target.CompareTag("Meat"))
            {
                Destroy(target.gameObject);
                audioSource.PlayOneShot(clickSound);
                StartCoroutine(CookFood(Steak, spawnPosition));
                ResetIngredients();
                return;
            }
        }

        // כל דבר אחר = כישלון
        Destroy(target.gameObject);
        audioSource.PlayOneShot(clickSound);
        StartCoroutine(CookFood(FailDish, spawnPosition));
        ResetIngredients();
    }
}
