using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : KitchenTool
{
    public Food eggEyePrefab;      // פריפאב של EggEye
    public Food Bacon;             // פריפאב של Bacon
    public Food AppleStrudel;      

    private bool PickledApplesIn = false;
    private bool BreadIn = false;
    
    private void ResetFlags()
    {
        PickledApplesIn = false;
        BreadIn = false;
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
            if (PickledApplesIn)
            {
                if (target.CompareTag("Bread"))
                {
                    Instantiate(AppleStrudel, spawnPosition, Quaternion.identity);
                    ResetFlags();
                    Destroy(target.gameObject);
                    Destroy(gameObject);
                    return;
                }
            }

            if (BreadIn)
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

            // Egg -> EggEye
            if (target.CompareTag("Egg"))
            {
                Instantiate(eggEyePrefab, spawnPosition, Quaternion.identity);
                Destroy(target.gameObject);
                Destroy(gameObject);
                return;
            }

            // Meat -> Bacon
            if (target.CompareTag("Meat"))
            {
                Instantiate(Bacon, spawnPosition, Quaternion.identity);
                Destroy(target.gameObject);
                Destroy(gameObject);
                return;
            }

            // מתכון חדש: PickledApples + Bread -> AppleStrudel
            if (target.CompareTag("PickledApples"))
            {
                PickledApplesIn = true;
                Destroy(target.gameObject);
                return;
            }

            if (target.CompareTag("Bread"))
            {
                BreadIn = true;
                Destroy(target.gameObject);
                return;
            }
            
        }
    }
}
