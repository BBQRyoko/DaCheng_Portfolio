using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    private SpriteRenderer sp;
    public float hurtLength;
    public float hurtCounter;
    private Transform target;
    private void Start()
    {

        sp = GetComponent<SpriteRenderer>();
       
    }
    public void TakenDamage(float _amount)
    {

        HurtShader();
    }
    private void HurtShader()
    {
        sp.material.SetFloat("_FlashAmout", 1);
        hurtCounter = hurtLength;
    }
}