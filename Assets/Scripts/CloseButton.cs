using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public RecipesManager recipesManager;
    public Sound soundManager;
    public void Close()
    {
        recipesManager.OpenRecipe();
        soundManager.PlaySound();
    }
}
