using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public Target target;
    bool check;
    
    public AudioClip clickSound;   // הצליל שיושמע
    private AudioSource audioSource;
    public AudioClip clickSound2;
    private AudioSource audioSource2;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        clickSound = Resources.Load<AudioClip>("HappyVoice");
        audioSource2 = gameObject.AddComponent<AudioSource>();
        clickSound2 = Resources.Load<AudioClip>("SadVoice");
        animator = GetComponent<Animator>();
        if (target == null)
        {
            target = FindObjectOfType<Target>();
            if (target == null)
            {
                Debug.LogError("No Target found in the scene!", this);
            }
            else
            {
                Debug.Log("Target found automatically: " + target.name, this);
            }
        }
        else
        {
            Debug.Log("Target assigned in Inspector: " + target.name, this);
        }
    }

    public void Taste(string tagName)
    {
        check = false;
        int len = target.objectsArray.Length;

        for (int i = 0; i < len; i++)
        {
            if (target.objectsArray[i] != null && target.objectsArray[i].CompareTag(tagName))
            {
                target.addpoint();
                target.RemoveFoodImage(i);
                target.RemoveFood(i);
                audioSource.PlayOneShot(clickSound);
                // נגן ישירות את הסטייט של הטעימה
                animator.CrossFade("TasteGood", 0.05f, 0, 0f); // שם הסטייט ב-Animator
                check = true;
                break;
            }
        }

        if (!check)
        {
            // דילי של 1.5 שניות
            Invoke(nameof(PlaySadVoice), 0.8f);

            // נגן ישירות את הסטייט של הטעות
            animator.CrossFade("BadTaste", 0.05f, 0, 0f);
        }
    }
    private void PlaySadVoice()
    {
        audioSource2.PlayOneShot(clickSound2);
    }

    void OnMouseDown()
    {
        if (target != null)
        {
            target.AnimationAgain();
        }
    }


    public void OpenMouth()
    {
        
        animator.SetInteger("Anim",1);

    }

    public void CloseMouth()
    {
        animator.SetInteger("Anim",0);
    }
}