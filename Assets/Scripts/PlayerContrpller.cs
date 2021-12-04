using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerContrpller : MonoBehaviour
{
    PlayerStats playerStats;
    SpriteRenderer characterSprite;
    Rigidbody2D rb;
    public float moveH, moveV;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] Animator animator;
    [SerializeField] int gunsNum;
    [SerializeField] GameObject[] guns;
    Vector2 movenment;
    Vector2 originalScale;

    private void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        characterSprite = GetComponentInChildren<SpriteRenderer>();
        guns[0].SetActive(true);
        originalScale = characterSprite.transform.localScale;
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
        if (!playerStats.isDead) 
        {
            if (moveH != 0 || moveV != 0)
            {
                rb.velocity = new Vector2(moveH, moveV);
                rb.MovePosition(rb.position + movenment * moveSpeed * Time.fixedDeltaTime);
            }

            if (moveH < 0)
            {
                characterSprite.transform.localScale = new Vector2(-originalScale.x, originalScale.y);
            }
            else
            {
                characterSprite.transform.localScale = new Vector2(originalScale.x, originalScale.y);
            }
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

    public void PlayerDeath() 
    {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}