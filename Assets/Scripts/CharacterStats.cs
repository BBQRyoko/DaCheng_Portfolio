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
    public bool isGetingDamage;

    [Header("Layers")]
    public LayerMask zombieLayer; //僵尸层
    [SerializeField] protected float viewRadius; //可见范围

    private void LateUpdate()
    {
        isGetingDamage = animator.GetBool("GetDamage");
    }
    public void getDamage(float damage, Vector2 direction)
    {
        if (transform.CompareTag("Player")) 
        {
            transform.GetComponent<PlayerContrpller>().moveV = 0;
            transform.GetComponent<PlayerContrpller>().moveH = 0;
        }
        Rigidbody2D rig2D = transform.GetComponent<Rigidbody2D>();
        if (!isGetingDamage) 
        {
            animator.SetBool("GetDamage", true);
            curHealth -= damage;
            if (curHealth <= 0)
            {
                curHealth = 0;
                if (transform.CompareTag("Zombie"))
                {
                    animator.SetBool("preDead", true);
                }
                else if (transform.CompareTag("Human"))
                {
                    //人类的预死亡
                }
            }
            rig2D.AddForce(direction, ForceMode2D.Impulse);        
        }
    }
}
