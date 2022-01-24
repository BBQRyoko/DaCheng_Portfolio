using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    Collider2D collider2D;

    private void Awake()
    {
        collider2D = GetComponentInParent<Collider2D>();
    }
    private void DamageEnding() 
    {
        collider2D.isTrigger = false;
    }
}
