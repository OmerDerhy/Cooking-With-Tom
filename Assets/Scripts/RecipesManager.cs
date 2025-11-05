using UnityEngine;
using UnityEngine.UI;
public class RecipesManager : MonoBehaviour
{
    public GameObject[] pages;
    public Button closeButton;
    public Button RightArrowButton;
    public Button LeftArrowButton;
    public Text text;
    public Transform canvasTransform; // assign in Inspector
    
    private Vector3 spawnPosition = new Vector3(-0.6f,-0.3f, 0);
    private GameObject currentPage;
    private int currentIndex = -1;
    private bool IsOpen = false;

    public void OpenRecipe()
    {
        if (IsOpen)
        {
            IsOpen = false;
            Destroy(currentPage);
            RightArrowButton.gameObject.SetActive(false);
            LeftArrowButton.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            closeButton.gameObject.SetActive(false);
            return;
        }
        closeButton.gameObject.SetActive(true);
        IsOpen = true;
        currentIndex = 0;
        
        currentPage = Instantiate(pages[currentIndex], spawnPosition, Quaternion.identity);

        // חיבור ל-RecipePage
        RecipePage page = currentPage.GetComponent<RecipePage>();
        if (page != null)
            page.recipesManager = this;
        
        RightArrowButton.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
    }

    public void NextPage()
    {
        if (currentIndex + 1 >= pages.Length) return; // בדיקה שלא חורגים מהמערך

        Destroy(currentPage);
        currentIndex++;
        currentPage = Instantiate(pages[currentIndex], spawnPosition, Quaternion.identity);
        LeftArrowButton.gameObject.SetActive(true);
        if (currentIndex == pages.Length - 1)
        {
            RightArrowButton.gameObject.SetActive(false);
        }

        // חיבור ל-RecipePage החדש
        RecipePage page = currentPage.GetComponent<RecipePage>();
        if (page != null)
            page.recipesManager = this;
    }

    public void PreviousPage()
    {
        if (currentIndex - 1 < 0) return;
        Destroy(currentPage);
        currentIndex--;
        currentPage = Instantiate(pages[currentIndex], spawnPosition, Quaternion.identity);
        
        RightArrowButton.gameObject.SetActive(true);
        if (currentIndex == 0)
        {
            LeftArrowButton.gameObject.SetActive(false);
        }
        RecipePage page = currentPage.GetComponent<RecipePage>();
        if (page != null)
            page.recipesManager = this;
    }
}