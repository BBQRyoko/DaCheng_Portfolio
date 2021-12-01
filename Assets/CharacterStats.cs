using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    public Animator animator;

    [Header("Common Stats")] //基础数值
    public float maxHealth;
    public float curHealth;

    [Header("Layers")]
    public LayerMask zombieLayer; //僵尸层
    [SerializeField] protected float viewRadius; //可见范围

    public void getDamage(float damage, Vector2 direction)
    {
        if (transform.CompareTag("Player")) 
        {
            transform.GetComponent<PlayerContrpller>().moveV = 0;
            transform.GetComponent<PlayerContrpller>().moveH = 0;
        }
        Rigidbody2D rig2D = transform.GetComponent<Rigidbody2D>();
        curHealth -= damage;
        if (curHealth <= 0)
        {
            curHealth = 0;
        }
        rig2D.AddForce(direction, ForceMode2D.Impulse);
        animator.SetTrigger("GetDamage");
    }
}
