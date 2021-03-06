using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;


public class PlayerStats : CharacterStats
{
    public bool isDead;
    PlayerContrpller playerContrpller;
    [SerializeField] ZombieStats curRescueTarget;
    Collider2D rescueTarget; //僵尸目标检测
    public Collider2D doorTriggerTarget;
    public LayerMask doorTriggerLayer;
    [SerializeField] float rescueRadius; //救助范围
    public float rescueTime;

    public List<ZombieStats> npcList = new List<ZombieStats>();

    //状态
    public float poisionTimer = 0f;
    public float damageDuration = 1f;
    
    private void Awake()
    {
        playerContrpller = FindObjectOfType<PlayerContrpller>();
        curHealth = maxHealth;//当前血量为最大值
        isDead = false;
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        RescueFunction ();
        DoorTriggerNotice();
        if (npcList != null) 
        {
            foreach (ZombieStats npc in npcList)
            {
                if (!npc.isHuman)
                {
                    npcList.Remove(npc);
                }
            }
        }

        if (curHealth <= 0) 
        {
            animator.SetTrigger("Dead");
            isDead = true;
            playerContrpller.Invoke("PlayerDeath", 1.1f);
        }

        //中毒
        if (poisionTimer > 0) 
        {
            poisionTimer -= Time.deltaTime;
            if (damageDuration <= 0) 
            {
                getDamage(20f, new Vector2(0,0));
                damageDuration = 3f;
            }
            if (damageDuration > 0) 
            {
                damageDuration -= Time.deltaTime;
            }
        }
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
            else
            {

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
                    npcList.Add(curRescueTarget);
                    curRescue.isHuman = true;//救援对象的Type变为human
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

    void DoorTriggerNotice() 
    {
        doorTriggerTarget = Physics2D.OverlapCircle(transform.position, viewRadius, doorTriggerLayer);

        if (doorTriggerTarget != null)
        {
            for (int i = 0; i < npcList.Count; i++)
            {
                if (!npcList[i].isStayOnTrigger)
                {
                    npcList[i].npcTrigger(doorTriggerTarget.transform);
                    return;
                }
            }
        }
        else 
        {
            foreach (ZombieStats npc in npcList)
            {
                npc.isStayOnTrigger = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rescueRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

    IEnumerator poisionDamage()
    {
        curHealth -= 20f;
        yield return new WaitForSeconds(3f);
    }
}
