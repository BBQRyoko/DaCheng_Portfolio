using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    CharacterStats zombieStats;
    [SerializeField] float damage = 15f;

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
                PlayerStats player = other.GetComponent<PlayerStats>();
                other.GetComponentInChildren<HealthBar>().hp -= damage;
                Vector2 difference = other.transform.position - transform.position;
                difference.Normalize();
                player.getDamage(damage, difference * 10f);
        }
    }
}