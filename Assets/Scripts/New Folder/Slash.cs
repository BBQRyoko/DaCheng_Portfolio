using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    CharacterStats zombieStats;

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
                CharacterStats player = other.GetComponent<CharacterStats>();
                //other.GetComponentInChildren<HealthBar>().hp -= 20;
                Vector2 difference = other.transform.position - transform.position;
                difference.Normalize();
                player.getDamage(15f, difference * 10f);
        }
    }
}