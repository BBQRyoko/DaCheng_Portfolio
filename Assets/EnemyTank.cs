using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    [SerializeField] int health = 3;
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] Sprite brokenSprite;
    bool isSpawned;

    void Update()
    {
        if (health <= 0 && !isSpawned) 
        {
            health = 0;
            isSpawned = true;
            transform.GetComponent<SpriteRenderer>().sprite = brokenSprite;
            Instantiate(zombiePrefab, transform.position, transform.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet")) 
        {
            health -= 1;
        }
    }
}
