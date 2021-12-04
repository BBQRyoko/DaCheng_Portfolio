using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun2 : MonoBehaviour
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
        muzzlePos = transform.Find("Gun2 shoot point");
        bulletShellPOS = transform.Find("ShellSpawn1");
        flipY = transform.localScale.y;
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //·­×ª
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
        if (Input.GetButtonDown("Fire1"))
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
        bullet.GetComponent<Bullet>().SetSpeed(direction);

        Instantiate(bulletShellPrefab, bulletShellPOS.position, bulletShellPOS.rotation);
    }
}
