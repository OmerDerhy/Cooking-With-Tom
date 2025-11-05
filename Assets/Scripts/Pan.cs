using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pan : KitchenTool
{
    public Food eggEyePrefab;      // פריפאב של EggEye
    public Food Bacon;             // פריפאב של Bacon
    public Food AppleStrudel;   
    public Food Panini;
    
    private string[] FoodIn = new string[2];
    private int foodCount = 0;

    private bool PickledApplesIn = false;
    private bool BreadIn = false;
    private bool PowderIn = false;
    private bool ChocolateIn = false;
    
    private void ResetFlags()
    {
        PickledApplesIn = false;
        BreadIn = false;
        PowderIn = false;
        ChocolateIn = false;
        FoodIn[0] = null;
        FoodIn[1] = null;
        foodCount = 0;
    }

    void OnTriggerStay2D(Collider2D target)
    {
        if (!target.TryGetComponent<Food>(out Food foodScript)) return;
        if (foodScript.getIsDragging()) return;

        if (!canDrag)
        {
            Vector3 spawnPosition = new Vector3(
                target.transform.position.x,
                target.transform.position.y + 2f,
                target.transform.position.z
            );
            if (foodCount == 2)
            {
                ResetFlags();
                return;
            }

            if (foodCount == 1)
            {
                if (PowderIn && target.tag == "Chocolate")
                {
                    Instantiate(Panini, spawnPosition, Quaternion.identity);
                    ResetFlags();
                    Destroy(target.gameObject);
                    Destroy(gameObject);
                    return;
                }

                if (ChocolateIn && target.tag == "Powder")
                {
                    Instantiate(Panini, spawnPosition, Quaternion.identity);
                    ResetFlags();
                    Destroy(target.gameObject);
                    Destroy(gameObject);
                    return;
                }

                if (PickledApplesIn)
                {
                    if (target.CompareTag("Powder"))
                    {
                        Instantiate(AppleStrudel, spawnPosition, Quaternion.identity);
                        ResetFlags();
                        Destroy(target.gameObject);
                        Destroy(gameObject);
                        return;
                    }
                }

                if (PowderIn)
                {
                    if (target.CompareTag("PickledApples"))
                    {
                        Instantiate(AppleStrudel, spawnPosition, Quaternion.identity);
                        ResetFlags();
                        Destroy(target.gameObject);
                        Destroy(gameObject);
                        return;
                    }
                }
            }

            // Egg -> EggEye
            if (target.CompareTag("Egg"))
            {
                Instantiate(eggEyePrefab, spawnPosition, Quaternion.identity);
                Destroy(target.gameObject);
                Destroy(gameObject);
                ResetFlags();
                return;
            }

            // Meat -> Bacon
            if (target.CompareTag("Meat"))
            {
                Instantiate(Bacon, spawnPosition, Quaternion.identity);
                Destroy(target.gameObject);
                Destroy(gameObject);
                ResetFlags();
                return;
            }

            // מתכון חדש: PickledApples + Bread -> AppleStrudel
            if (target.CompareTag("PickledApples"))
            {
                PickledApplesIn = true;
                foodCount++;
                Destroy(target.gameObject);
                return;
            }

            if (target.CompareTag("Bread"))
            {
                BreadIn = true;
                foodCount++;
                Destroy(target.gameObject);
                return;
            }
            if (target.CompareTag("Powder"))
            {
                PowderIn = true;
                foodCount++;
                Destroy(target.gameObject);
                return;
            }
            

            if (target.CompareTag("Chocolate"))
            {
                ChocolateIn = true;
                foodCount++;
                Destroy(target.gameObject);
            }
            
        }
    }
}
