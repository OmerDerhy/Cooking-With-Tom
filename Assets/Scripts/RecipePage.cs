using UnityEngine;
using UnityEngine.UI;
public class RecipePage : MonoBehaviour
{
    public RecipesManager recipesManager;
    
    public void Next()
    {
        recipesManager.NextPage();
    }

    public void Previous()
    {
        recipesManager.PreviousPage();
    }
}