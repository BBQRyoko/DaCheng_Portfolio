using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShdowCover : MonoBehaviour
{
    public SpriteRenderer[] coverShadow;
    public CharacterStats[] zombieList;
    float alpha;
    
    public float dissolveRate = 1f;
    bool playerEntered;

    private void Start()
    {
        coverShadow = GetComponentsInChildren<SpriteRenderer>();
        zombieList = GetComponentsInChildren<CharacterStats>();

    }

    private void Update()
    {
        
        if (playerEntered) //玩家进入
        {
            //透明度变化
            alpha -= dissolveRate * Time.deltaTime;
            if (alpha <= 0)
                alpha = 0;
            foreach (SpriteRenderer tiles in coverShadow)
            {
                if(!tiles.CompareTag("Zombie") && !tiles.CompareTag("Human"))
                    tiles.color = new Color(tiles.color.r, tiles.color.g, tiles.color.b, alpha);
            }

            //僵尸启动
            foreach (CharacterStats zombie in zombieList) 
            {
                if (zombie.CompareTag("Zombie"))
                {
                    zombie. isActive=true;
                }
            }

            //将变成人的僵尸从列表删除
        }
        else //玩家离开
        {
            //透明度变化
            alpha += dissolveRate * Time.deltaTime;
            if (alpha >= 1)
                alpha = 1;
            foreach (SpriteRenderer tiles in coverShadow)
            {
                if (!tiles.CompareTag("Zombie") && !tiles.CompareTag("Human"))
                    tiles.color = new Color(tiles.color.r, tiles.color.g, tiles.color.b, alpha);
            }

            //列表中的僵尸重置
            foreach (CharacterStats zombie in zombieList)
            {
                if (zombie.CompareTag("Zombie"))
                {
                    zombie.isActive = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player"))
        {
            playerEntered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerEntered = false;
        }
    }
}
