using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun3 : MonoBehaviour
{
    public  CameraController cameraController;
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
    public GameObject frontSight;
    public LayerMask blockingLayer;

    bool isAiming;


    void Start()
    {
        animator = GetComponent<Animator>();
        muzzlePos = transform.Find("MuzzlePos3");
        bulletShellPOS = transform.Find("ShellSpawn3");
        flipY = transform.localScale.y;
    }

    void Update()
    {
        Aim();
        Shoot();

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //·­×ª
        if (mousePos.x < transform.position.x)
            transform.localScale = new Vector3(flipY, -flipY, 1);
        else
            transform.localScale = new Vector3(flipY, flipY, 1);

        Color color = new Color(0, 0, 1f);
        Debug.DrawLine(transform.position, frontSight.transform.position);

        if (isAiming && timer == 0)
        {
            frontSight.SetActive(true);
            frontSight.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            cameraController.target = frontSight.transform;
        }
        else 
        {
            frontSight.SetActive(false);
            cameraController.target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }

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

    void Aim()
    {
        if (Input.GetButton("Fire2"))
        {
            isAiming = true;

            Vector2 direction = new Vector2(frontSight.transform.position.x - muzzlePos.transform.position.x, frontSight.transform.position.y - muzzlePos.transform.position.y);

            float distance = Vector2.Distance(frontSight.transform.position, muzzlePos.transform.position);

            RaycastHit2D hitInfo = Physics2D.Raycast(muzzlePos.transform.position, direction, distance, blockingLayer);

            if (hitInfo.collider != null)
            {

                frontSight.SetActive(false);
            }
        }
        else
        {
            isAiming = false;
        }
    }


    void Fire()
    {
        animator.SetTrigger("Shoot");

        GameObject bullet = Instantiate(bulletPrefab, muzzlePos.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().SetSpeed(direction);

        Instantiate(bulletShellPrefab, bulletShellPOS.position, bulletShellPOS.rotation);

        isAiming = false;
        Debug.Log("Tried");
    }
}