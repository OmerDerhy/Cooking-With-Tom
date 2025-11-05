using System.Collections;
using UnityEngine;

public class TargetEndless : Target
{
    [Header("Endless Settings")]
    [Tooltip("מאגר כל המאכלים האפשריים להגרלה בכל סבב")]
    public Food[] foodPool;

    protected override void Start()
    {
        PickTwoRandomFoods();
        base.Start(); // Goal=2 ו-ShowFoodImages
    }

    public override void addpoint()
    {
        points++;

        if (points >= 2)
        {
            // אל תחליף מיד – חכה לסוף הפריים כדי לא להיתפס ע״י Taste שרצה עדיין
            StartCoroutine(BeginNextRoundAtEndOfFrame());
        }
        // אין Finish באינסוף
    }

    public override void Finish()
    {
        // אין סיום
    }

    private IEnumerator BeginNextRoundAtEndOfFrame()
    {
        // מחכים לסוף הפריים הנוכחי כדי ש-Face.Taste יסיים את הלולאה שלו
        yield return new WaitForEndOfFrame();

        points = 0;
        PickTwoRandomFoods();
        Goal = 2;

        // להצגה מחדש (אם כרגע יש אנימציית הצגה שרצה, היא תסתיים ותוכל להפעיל שוב)
        StartCoroutine(ShowFoodImages());
    }

    private void PickTwoRandomFoods()
    {
        if (foodPool == null || foodPool.Length < 2)
        {
            objectsArray = new Food[0];
            return;
        }

        int a = Random.Range(0, foodPool.Length);
        int b;
        do
        {
            b = Random.Range(0, foodPool.Length);
        } while (b == a); // שניים שונים בתוך הסבב

        objectsArray = new Food[2] { foodPool[a], foodPool[b] };
    }
}