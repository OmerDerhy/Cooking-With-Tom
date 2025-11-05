using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject Object; // 🧃 תגרור את ה-Prefab לכאן דרך ה-Inspector

    void OnMouseDown()
    {
        Vector3 spawnPosition = new Vector3(
                gameObject.transform.position.x,
                gameObject.transform.position.y,
                gameObject.transform.position.z
            );
        
            Instantiate(Object, spawnPosition, Quaternion.identity);
    }
    
}