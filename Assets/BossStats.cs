using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    Rigidbody2D rig;

    //基础数值
    public bool isProtected = true;
    public int shieldAmount = 3;
    public int curShieldHealth = 3;
    public float curHealth;
    float maxHealth = 100f;

    //盾
    [SerializeField] GameObject shield;
    public bool isThirdPhase;
    [SerializeField] float shieldTimer = 30f;


    //技能
    //冲刺
    [SerializeField] GameObject aim_Object;
    [SerializeField] float DashingTimer = 5f;
    [SerializeField] float CountTimer = 5f; //持续时间
    [SerializeField] float dashMutiplier =15;
    public GameObject dashingDamager;
    bool isDashing;

    //扫射
    [SerializeField] float shootingPeriod;
    [SerializeField] GameObject bulletPrefab;

    //导弹
    [SerializeField] GameObject aimingArea;
    [SerializeField] GameObject aimingArea_Prefab;
    public float innerTimer = 0;

    [SerializeField] Transform targetPos;
    [SerializeField] Vector2 finalDir;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        curHealth = maxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Dash_LockingTarget();
        //ShootingTarget();
        //ThrowingBomb();
        ShieldController();
        if (curHealth >= 70)
        {
            phase1();
        }
        else if (curHealth >= 33 && curHealth < 70) 
        {
            phase2();
        }

    }

    void phase1() 
    {
        Dash_LockingTarget();
    }

    void phase2()
    {
        isProtected = true;

        curShieldHealth = 3;
        //...
    }

    void phase3()
    {

    }


    void ShieldController() 
    {
        if (curShieldHealth > 0)
        {
            isProtected = true;
        }
        else 
        {
            isProtected = false;
        }

        if (isProtected)
        {
            shield.SetActive(true);
        }
        else 
        {
            shield.SetActive(false);
        }

        if (isThirdPhase)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0)
            {
                shieldTimer = 0;
                shield.SetActive(false);
            }
        }
    }

    private void Dash_LockingTarget() 
    {
        Vector2 dir = new Vector2(targetPos.position.x - transform.position.x, targetPos.position.y - transform.position.y);
        dir.Normalize();

        DashingTimer -= Time.deltaTime;

        if (DashingTimer >= 0.5f)
        {
            finalDir = dir;
            aim_Object.transform.right = dir;
        }
        else if(DashingTimer <= 0)
        {
            DashingTimer = 0;
            Dashing(finalDir);
            isDashing = true;
        }
    }
    private void Dashing(Vector2 dashDir) 
    {
        if (isDashing) 
        {
            dashingDamager.SetActive(true);
            rig.AddForce(dashDir * dashMutiplier, ForceMode2D.Impulse);
        }
    }
    private void ShootingTarget() 
    {
        Vector2 dir = new Vector2(targetPos.position.x - transform.position.x, targetPos.position.y - transform.position.y);

        CountTimer -= Time.deltaTime;

        if (CountTimer > 0f)
        {
            if (shootingPeriod > 0)
            {
                shootingPeriod -= Time.deltaTime;
                if (shootingPeriod <= 0)
                {
                    shootingPeriod = 0;
                }
            }
            else 
            {
                Fire(dir);
            }
        }
    }
    private void Fire(Vector2 dir) 
    {
        shootingPeriod = 0.1f;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        float angle = Random.Range(-5f, 5f);
        bullet.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis(angle, Vector3.forward) * dir);
    }
    private void ThrowingBomb() 
    {
        CountTimer -= Time.deltaTime;

        if (CountTimer > 0f)
        {
            aimingArea.gameObject.SetActive(true);
            innerTimer += Time.deltaTime;
            aimingArea.transform.position = Vector2.Lerp(aimingArea.transform.position, new Vector2(targetPos.position.x, targetPos.position.y), 7.5f * Time.deltaTime);
            if (innerTimer >= 3)
            {
                Instantiate(aimingArea_Prefab, aimingArea.transform.position, Quaternion.identity);
                innerTimer = 0;
            }
        }
        else 
        {
            aimingArea.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            isDashing = false;
            DashingTimer = 5f;
            dashingDamager.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            if (isDashing)
            {
                isDashing = false;
                DashingTimer = 5f;
                dashingDamager.SetActive(false);
            }
        }
    }
}
