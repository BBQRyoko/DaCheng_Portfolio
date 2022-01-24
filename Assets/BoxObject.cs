using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObject : MonoBehaviour
{
    Rigidbody2D rig;
    public Vector2 curVelocity;
    public bool isBossPushing;

    [SerializeField] bool isBomb;
    [SerializeField] GameObject explosion;

    


    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        curVelocity = rig.velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss")) 
        {
            isBossPushing = true;
        }

        if (collision.gameObject.CompareTag("Wall")) 
        {
            if (isBossPushing) 
            {
                if (isBomb) 
                {
                    GameObject exp = ObjectPool.Instance.GetObject(explosion);
                    exp.transform.position = transform.position;
                }
                Destroy(gameObject);
            }
        }
    }
}
