using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class open : MonoBehaviour
{
    public GameObject Door;
    public float stayTime;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")||other.CompareTag("Human") )
        {           
            Door.GetComponent<Animator>().SetBool("Open", true);
        }
       
       
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Human")) 
        {
            Door.GetComponent<Animator>().SetBool("Open", false);
        }
    }
}
