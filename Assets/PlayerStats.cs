using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    PlayerContrpller playerContrpller;
    [SerializeField] ZombieStats curRescueTarget;
    Collider2D rescueTarget; //僵尸目标检测
    [SerializeField] float rescueRadius; //救助范围
    public float rescueTime;

    private void Awake()
    {
        playerContrpller = FindObjectOfType<PlayerContrpller>();
        curHealth = maxHealth;//当前血量为最大值
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        RescueFunction ();
    }

    void RescueFunction() //救助功能
    {
        rescueTarget = Physics2D.OverlapCircle(transform.position, rescueRadius, zombieLayer);

        if (rescueTarget != null)
        {
            ZombieStats zombie = rescueTarget.GetComponent<ZombieStats>();

            if (zombie.isDisable)
            {
                curRescueTarget = zombie;
            }
        }
        if (curRescueTarget)
        {
            ZombieStats curRescue = curRescueTarget.GetComponent<ZombieStats>();//curRescue就为 curRescueTarget
            if (Input.GetKey(KeyCode.E))//若输入键盘E
            {
                rescueTime += Time.deltaTime;//救援时间就为按下时间
                curRescue.rescueBar.fillAmount = rescueTime / 1.5f;//救助对象的救助的量就为按下时间/1.5
                if (rescueTime >= 1.5f) //若救援时间大于1.5
                {
                    //curRescue.curType = characterType.human;//救援对象的Type变为human
                    curRescue.curHealth = maxHealth;//救援对象的当前血量变为maxHealth
                    curRescue.rescueButton.gameObject.SetActive(false);//救援按钮不显示出来
                    curRescueTarget = null;//救助对象变为空项目
                    curRescue.GetComponentInChildren<HealthBar>().hp = maxHealth;
                    curRescue.gameObject.tag = "Human";
                    rescueTime = 0;
                }
            }
            else if (Input.GetKeyUp(KeyCode.E)) //若输入E的时间 变为0时 则救助对象的救助进度量变为0
            {
                rescueTime = 0;
                curRescue.rescueBar.fillAmount = 0;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rescueRadius);
    }
}
