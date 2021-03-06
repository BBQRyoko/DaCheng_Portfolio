using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed;
    public float bulletDamage;
    public float impactNum;
    public GameObject explosionPrefab;
    new private Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetSpeed(Vector2 direction)
    {
        rigidbody.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Zombie") && !other.CompareTag("Bullet") && !other.CompareTag("Shadow"))
        {
            if (other.CompareTag("Player"))
            {
                PlayerStats player = other.GetComponent<PlayerStats>();
                other.GetComponentInChildren<HealthBar>().hp -= bulletDamage;
                Vector2 difference = other.transform.position - transform.position;
                difference.Normalize();
                player.getDamage(bulletDamage, difference * impactNum);
                //other.transform.position = new Vector2(other.transform.position.x + difference.x,
                //other.transform.position.y + difference.y);
            }
            // Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            GameObject exp = ObjectPool.Instance.GetObject(explosionPrefab);
            exp.transform.position = transform.position;

            Destroy(gameObject);
            ObjectPool.Instance.PushObject(gameObject);
        }
    }
}
