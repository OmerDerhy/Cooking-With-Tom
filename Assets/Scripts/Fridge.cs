using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour
{
    private bool OpenFridge = false;

    public GameObject fridge; 
    public Food[] objectsArray;
    
    private GameObject spawnedFridgeObject; 
    private Capsule spawnedFridgeScript;    
    public void Open()
    {
        if (!OpenFridge)
        {
            Vector3 spawnPosition = new Vector3(0, 3.6f, 0);

            spawnedFridgeObject = Instantiate(fridge, spawnPosition, Quaternion.identity);
            spawnedFridgeScript = spawnedFridgeObject.GetComponent<Capsule>();

            for (int i = 0; i < objectsArray.Length; i++)
            {
                Vector3 foodPosition = new Vector3(-4 + i * 1.4f, 4, 0);
                GameObject food = Instantiate(objectsArray[i].gameObject, foodPosition, Quaternion.identity);
                // לא לשייך הורה! food.transform.SetParent(spawnedFridgeObject.transform); <-- לא עושים את זה
            }

            OpenFridge = true;
        }
        else
        {
            if (spawnedFridgeScript != null)
            {
                spawnedFridgeScript.DestroyFoods();
            }

            Destroy(spawnedFridgeObject);
            OpenFridge = false;
        }
    }
}