using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    private List<GameObject> objectsInTrigger = new List<GameObject>();

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Food>() != null && !objectsInTrigger.Contains(other.gameObject))
        {
            objectsInTrigger.Add(other.gameObject);
        }
    }
    

    void OnTriggerExit2D(Collider2D other)
    {
        if (objectsInTrigger.Contains(other.gameObject))
        {
            objectsInTrigger.Remove(other.gameObject);
        }
    }

    public void DestroyFoods()
    {
        for (int i = objectsInTrigger.Count - 1; i >= 0; i--)
        {
            GameObject obj = objectsInTrigger[i];
            if (obj != null && GetComponent<Collider2D>().bounds.Contains(obj.transform.position))
            {
                Destroy(obj);
            }

        }
    }
}