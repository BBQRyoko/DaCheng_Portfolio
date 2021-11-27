using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerContrpller : MonoBehaviour
{
    CharacterStats characterStats;
    private Rigidbody2D rb;
    public float moveH, moveV;
    [SerializeField] private float moveSpeed = 5f;
    private Animator animator;
    public int gunsNum;

    Vector2 movenment;
    Vector2 mousePos;
    public Camera cam;

    public GameObject[] guns;

    Vector2 originalScale;
   
    private void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        guns[0].SetActive(true);
        originalScale = transform.localScale;
    }

    private void Update()
    {
        CharacterMoveAnimator();
        SwitchGun();
        moveH = Input.GetAxisRaw("Horizontal")*moveSpeed ;
        moveV = Input.GetAxisRaw("Vertical")*moveSpeed ;
        movenment.x = Input.GetAxisRaw("Horizontal");
        movenment.y = Input.GetAxisRaw("Vertical");
    }
    private void FixedUpdate()
    {
        if (moveH != 0 || moveV != 0)
        {
            rb.velocity = new Vector2(moveH, moveV);
            rb.MovePosition(rb.position + movenment * moveSpeed * Time.fixedDeltaTime);
        }

        if (moveH < 0)
        {
            transform.localScale = new Vector2(-originalScale.x, originalScale.y);
        }
        else 
        {
            transform.localScale = new Vector2(originalScale.x, originalScale.y);
        }
    }
    void SwitchGun()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            guns[gunsNum].SetActive(false);
            if (gunsNum >= 0)
            {
                gunsNum = gunsNum + 1;
                if (gunsNum >= guns.Length ) 
                {
                    gunsNum = 0;
                }
            }
            guns[gunsNum].SetActive(true);
        }
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    guns[gunsNum].SetActive(false);
        //    if (gunsNum <= guns.Length)
        //    {
        //        gunsNum = gunsNum -1;
        //        if (gunsNum <= -1)
        //        {
        //            gunsNum = guns.Length-1;
        //        }
        //    }
        //    guns[gunsNum].SetActive(true);
        //}
    }

    void CharacterMoveAnimator() 
    {
        if (moveH == 0 && moveV == 0)
        {
            animator.SetTrigger("Idle");
        }
        else if(moveV > 0)
        {
            animator.SetTrigger("Up");
        }
        else if (moveV <= 0)
        {
            animator.SetTrigger("Down");
        }
    }
}