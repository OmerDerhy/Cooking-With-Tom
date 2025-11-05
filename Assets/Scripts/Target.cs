using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    public GameObject WinButton;
    public Food[] objectsArray;
    public Face face;
    public int currentLevel;
    public Image Cookies;
    public Button NextLevelButton;

    public AudioClip clickSound;
    protected AudioSource audioSource;   // ← protected כדי שגם בת ירשה

    protected Animator animator;         // ← protected
    protected int points;                // ← protected
    protected int Goal;                  // ← protected
    protected bool win;                  // ← protected
    protected List<GameObject> foodImages = new List<GameObject>();
    protected bool InAnimation;          // ← protected

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        points = 0;
        Goal = (objectsArray != null) ? objectsArray.Length : 0;
        win = false;
        InAnimation = true;

        audioSource = gameObject.AddComponent<AudioSource>();
        if (!clickSound)
            clickSound = Resources.Load<AudioClip>("CashSound");

        StartCoroutine(ShowFoodImages()); // הצגה ראשונית
    }

    public void AnimationAgain()
    {
        if (InAnimation) return;
        StartCoroutine(AnimationWithLock());
    }

    private IEnumerator AnimationWithLock()
    {
        InAnimation = true;
        yield return StartCoroutine(ShowFoodImages());
        InAnimation = false;
    }

    public IEnumerator ShowFoodImages()
    {
        float startX = 2.3f;
        float startY = 1.1f;
        float spacing = 1.1f;

        if (animator) animator.Play("TargetAnimation", 0, 0f);

        yield return new WaitForSeconds(0.5f);

        // נקה שאריות קודמות אם יש
        foreach (var img in foodImages) if (img) Destroy(img);
        foodImages.Clear();

        if (objectsArray != null)
        {
            for (int i = 0; i < objectsArray.Length; i++)
            {
                Food food = objectsArray[i];
                if (food != null)
                {
                    GameObject imageObj = new GameObject("FoodImage_" + i);
                    imageObj.transform.position = new Vector3(startX + i * spacing, startY, 0);

                    if (food.CompareTag("Egg"))
                        imageObj.transform.localScale = new Vector3(0.3f, 0.3f, 2f);
                    else if (food.CompareTag("Banana"))
                        imageObj.transform.localScale = new Vector3(0.15f, 0.15f, 2f);
                    else if (food.CompareTag("CheeseCake") || food.CompareTag("Wine")|| food.CompareTag("Pudding")|| food.CompareTag("CupCake"))
                        imageObj.transform.localScale = new Vector3(0.4f, 0.4f, 2f);
                    else if (food.CompareTag("Orange"))
                        imageObj.transform.localScale = new Vector3(0.5f, 0.5f, 2f);
                    else
                        imageObj.transform.localScale = new Vector3(0.2f, 0.2f, 2f);

                    SpriteRenderer renderer = imageObj.AddComponent<SpriteRenderer>();
                    var src = food.GetComponent<SpriteRenderer>();
                    if (src) renderer.sprite = src.sprite;
                    renderer.sortingOrder = 3;

                    foodImages.Add(imageObj);
                }
            }
        }

        yield return new WaitForSeconds(3.4f);

        foreach (var img in foodImages) if (img) Destroy(img);
        foodImages.Clear();
        InAnimation = false;
    }

    public void RemoveFoodImage(int index)
    {
        if (index >= 0 && index < foodImages.Count)
        {
            if (foodImages[index]) Destroy(foodImages[index]);
            foodImages.RemoveAt(index);
        }
    }

    // ⬇️ חשוב: virtual כדי שנוכל לדרוס באינסופי
    public virtual void addpoint()
    {
        points++;
        if (points >= Goal && !win)
        {
            Finish();
        }
    }

    public void RemoveFood(int index)
    {
        if (objectsArray == null || objectsArray.Length == 0 || index < 0 || index >= objectsArray.Length)
            return;

        Food[] newArray = new Food[objectsArray.Length - 1];
        int newIndex = 0;

        for (int i = 0; i < objectsArray.Length; i++)
        {
            if (i == index) continue;
            newArray[newIndex++] = objectsArray[i];
        }

        objectsArray = newArray;
    }

    public virtual void Finish()
    {
        // יצירת כפתור הניצחון
        Vector3 spawnPosition = new Vector3(-1, 0.5f, 0);
        Instantiate(WinButton, spawnPosition, Quaternion.identity);
        NextLevelButton.gameObject.SetActive(true);
        win = true;
        if (Cookies) Cookies.gameObject.SetActive(true);
        if (audioSource && clickSound) audioSource.PlayOneShot(clickSound);

        // עדכון HighestLevel
        int highestLevel = PlayerPrefs.GetInt("HighestLevel", 1);
        if (currentLevel >= highestLevel)
        {
            PlayerPrefs.SetInt("HighestLevel", currentLevel + 1);
        }

        // בדיקה אם השלב כבר הושלם בעבר
        string levelKey = "Level_" + currentLevel + "_Completed";
        bool alreadyCompleted = PlayerPrefs.GetInt(levelKey, 0) == 1;

        if (!alreadyCompleted)
        {
            int currentCoins = PlayerPrefs.GetInt("Coins", 0);
            currentCoins += 200;
            PlayerPrefs.SetInt("Coins", currentCoins);

            PlayerPrefs.SetInt(levelKey, 1);

            PlayerPrefs.Save();
            Debug.Log("Coins: " + currentCoins);
        }
    }
}
