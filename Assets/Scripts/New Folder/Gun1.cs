using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun1 : MonoBehaviour
{

    public float interval;
    public GameObject bulletPrefab;
    public GameObject bulletShellPrefab;
    private Transform bulletShellPOS;
    private Transform muzzlePos;
    public Vector2 mousePos;
    private Vector2 direction;
    private float timer;

    private float flipY;
    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
        muzzlePos = transform.Find("Gun shoot point");
        bulletShellPOS = transform.Find("ShellSpawn");
        flipY = transform.localScale.y;
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //翻转
        if (mousePos.x < transform.position.x)
            transform.localScale = new Vector3(flipY, -flipY, 1);
        else
            transform.localScale = new Vector3(flipY, flipY, 1);

        Shoot();
    }
     void Shoot()
    {
        direction = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
        transform.right = direction;

        if (timer != 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
                timer = 0;
        }
        if(Input.GetButtonDown("Fire1"))
        {
            if (timer == 0)
            {
                Fire();
                timer = interval;
            }
        }
    }

    void Fire() 
    {
        animator.SetTrigger("Shoot");
       
        GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, Quaternion.identity);
       
        float angle = Random.Range(-5f, 5f);
        bullet.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis(angle ,Vector3.forward)*direction);

       Instantiate(bulletShellPrefab, bulletShellPOS.position, bulletShellPOS.rotation);
    }
}