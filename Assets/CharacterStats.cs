using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStats : MonoBehaviour
{
    //枚举 确认角色类型
    public enum characterType { human, zombie };
    public characterType curType;

    PlayerContrpller playerContrpller;
    Animator animator;
    Vector2 originalScale;

    [Header("Common Stats")] //基础数值
    [SerializeField] float maxHealth;
    public float curHealth;
    [SerializeField] float viewRadius; //可见范围

    [Header("Zombie Stats")] //僵尸专属的参数
    [SerializeField] LayerMask soundLayer; //声音层
    [SerializeField] LayerMask humanLayer; //人类层
    Collider2D curTarget; //当前目标
    public Transform npcSprite;
    public bool isActive;
    public GameObject rescueButton; //UI 救助按钮
    public Image rescueBar; //救助进度
    public float deadTime;
    private float stayTime = 1f;

    public enum IdleType { Stay, Patrol };
    public IdleType idleType;
    public List<Transform> patrolPos = new List<Transform>();
    public int curPatrolIndex = 0;


    //受到伤害时的颜色
    [Header("Hurt")]
    public SpriteRenderer sp;
    public float hurtLength;
    public float hurtCounter;

    //Attack
    public GameObject hAttackSp;
    public GameObject vAttackSp;
    public bool attacked;
    public float attackTimer;
    public float curAttack;

    public bool isDisable; //是否无法移动

    //参数
    [SerializeField] float hearRadius; //听觉距离
    [SerializeField] float attackRadius; //攻击距离
    [SerializeField] float moveSpeed1;
    [SerializeField] float moveSpeed2;

    public float rescueTime;

    [Header("Testing")] //测试用
    public Collider2D[] soundSources; //声源列表
    public Collider2D humanTarget; //人类目标
    public Collider2D attackTarget;

    [Header("Human Stats")]
    public Collider2D rescueTarget; //僵尸目标
    [SerializeField] LayerMask zombieLayer; //僵尸层


    public Collider2D curRescueTarget; //当前救助目标
    public Collider2D zombieTarget;
    [SerializeField] float rescueRadius; //救助范围
    [SerializeField] float followSpeed; //跟随速度
    public enum npcMode { follow, stay }; //NPC的行动模式
    public npcMode curMode; //当前模式

    public GameObject npcCommand;

    float originalScaleX;

    // Start is called before the first frame update
    void Awake()
    {
        playerContrpller = FindObjectOfType<PlayerContrpller>();
        curHealth = maxHealth;//当前血量为最大值
        curAttack = attackTimer;
        //sp = npcSprite.gameObject.GetComponent<SpriteRenderer>();
        originalScaleX = npcSprite.localScale.x;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        originalScale = transform.localScale;
        if (idleType == IdleType.Patrol) 
        {
            foreach (Transform child in gameObject.transform.parent) //巡逻模式
            {
                if (child.tag != "Zombie")
                {
                    patrolPos.Add(child);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (curType == characterType.zombie)//若当前状态为僵尸则运行僵尸代码
        {
            ZombieUpdate();

        }
        else if (curType == characterType.human) //若当前状态为人类则运行人类代码
        {
            HumanUpdate();
        }
    }

    void ZombieUpdate()
    {
        gameObject.layer = 8;//所有僵尸所在物理层为第8层

        if (curHealth <= 0) //如果当前血量低于0 =死亡
        {
            isDisable = true;//就不可以移动
            rescueButton.gameObject.SetActive(true);//救助按钮显示出来
            deadTime += Time.deltaTime;
            if (deadTime > 8)
            {
                isDisable = false;
                curHealth = maxHealth;
                GetComponentInChildren<HealthBar>().hp = maxHealth;
                deadTime = 0;
            }
        }

        if (curTarget == null) 
        {
            PatrolMove();
        }

        if (curTarget != null && isDisable == false && isActive == true && attacked == false) //当前有目标且僵尸可以移动时
        {
            transform.position = Vector2.MoveTowards(transform.position, curTarget.transform.position, moveSpeed2 * Time.deltaTime);//则僵尸向当前目标以moveSpeed2的速度移动
        }

        //目标检测
        soundSources = Physics2D.OverlapCircleAll(transform.position, hearRadius, soundLayer); //声音检测范围：以僵尸的transform.position为中心，以hearRadius为半径，又在soundLayer的层的所有目标
        humanTarget = Physics2D.OverlapCircle(transform.position, viewRadius, humanLayer);//人类目标检测：以以僵尸的transform.position为中心，，以viewRadius为半径，在人类层的目标
        attackTarget = Physics2D.OverlapCircle(transform.position, attackRadius, humanLayer);
        if (isDisable == false) //若僵尸可以移动
        {
            if (humanTarget != null)//有人类目标时 当前目标就为人类目标
            {
                curTarget = humanTarget;
            }
            else//没有人类目标 
            {
                if (soundSources.Length > 0 && !curTarget)//声音检测到目标且不为当前目标
                {
                    int latestSound = soundSources.Length;
                    transform.position = Vector2.MoveTowards(transform.position, soundSources[latestSound - 1].transform.position, moveSpeed1 * Time.deltaTime);//僵尸就会向着最后出现的声音检测到的目标以moveSpeed1的速度移动
                }
            }
            if (attackTarget != null)
            {
                ZombieAttack(attackTarget);
            }

        }


        //受伤效果
        if (hurtCounter > 0)
        {
            hurtCounter -= Time.deltaTime;
            sp.material.SetFloat("_FlashAmount", hurtCounter);
        }
        else
        {
            hurtCounter = 0;
        }


        //攻击冷却
        if (attacked)
        {
            curAttack -= Time.deltaTime;
            if (curAttack <= 0)
            {
                attacked = false;
                curAttack = attackTimer;
            }
        }

    }


    void HumanUpdate()
    {
        gameObject.layer = 6;//人类所在的物理层为6


        //救助功能
        if (curRescueTarget)
        {
            CharacterStats curRescue = curRescueTarget.GetComponent<CharacterStats>();//curRescue就为 curRescueTarget
            if (Input.GetKey(KeyCode.E))//若输入键盘E
            {
                rescueTime += Time.deltaTime;//救援时间就为按下时间
                curRescue.rescueBar.fillAmount = rescueTime / 1.5f;//救助对象的救助的量就为按下时间/1.5
                if (rescueTime >= 1.5f) //若救援时间大于1.5
                {
                    curRescue.curType = characterType.human;//救援对象的Type变为human
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

        //NPC模式
        if (curMode == npcMode.follow) //若当前NPC的模式为跟随模式时
        {
            float distance = Vector2.Distance(transform.position, playerContrpller.transform.position);//NPC与玩家距离

            if (distance >= 2)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerContrpller.transform.position, followSpeed * Time.deltaTime);//当距离大于2时 则NPC以followSpeed的速递向玩家移动
                npcCommand.SetActive(false);
            }
            else if (distance <= 1 && distance > 0)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    npcCommand.SetActive(true);
                }
            }
        }
        else if (curMode == npcMode.stay) //若当前NPC的模式为跟随模式时
        {
            float distance = Vector2.Distance(transform.position, playerContrpller.transform.position);//NPC与玩家距离

            if (distance >= 2)
            {
                npcCommand.SetActive(false);
            }
            else if (distance <= 1 && distance > 0)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    npcCommand.SetActive(true);
                }
            }
        }

        //僵尸检测
        rescueTarget = Physics2D.OverlapCircle(transform.position, rescueRadius, zombieLayer);
        zombieTarget = Physics2D.OverlapCircle(transform.position, viewRadius, zombieLayer);
        if (zombieTarget != null && zombieTarget.GetComponent<CharacterStats>().isActive)
        {
            float distance = Vector2.Distance(transform.position, zombieTarget.transform.position);

            if (distance >= 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, zombieTarget.transform.position, followSpeed * Time.deltaTime);//当距离大于2时 则NPC以followSpeed的速递向玩家移动
            }
            else if (distance < 1 && distance > 0 && !isDisable)
            {
                if (!gameObject.CompareTag("Player")) 
                {
                    ZombieAttack(zombieTarget);
                }
            }
        }
        if (rescueTarget != null)
        {
            CharacterStats zombie = rescueTarget.GetComponent<CharacterStats>();

            if (zombie.isDisable)
            {
                curRescueTarget = rescueTarget;
            }
        }


        //受伤效果
        if (hurtCounter > 0)
        {
            hurtCounter -= Time.deltaTime;
            sp.material.SetFloat("_FlashAmount", hurtCounter);
        }
        else
        {
            hurtCounter = 0;
        }

        //攻击冷却
        if (attacked)
        {
            curAttack -= Time.deltaTime;
            if (curAttack <= 0)
            {
                attacked = false;
                curAttack = attackTimer;
            }
        }

    }

    void PatrolMove() 
    {
        if (idleType == IdleType.Patrol) 
        {
            float distanceFromTarget = Vector2.Distance(patrolPos[curPatrolIndex].position, transform.position);

            if (distanceFromTarget > 0.05f)
            {
                transform.position = Vector2.MoveTowards(transform.position, patrolPos[curPatrolIndex].position, moveSpeed1 * Time.deltaTime);//则僵尸向当前目标以moveSpeed2的速度移动
            }
            else
            {
                curPatrolIndex = curPatrolIndex + 1;
                if (curPatrolIndex >= patrolPos.Count)
                {
                    curPatrolIndex = 0;
                }
            }
        }
    }

    public void getDamage(float damage, Vector2 direction)
    {
        playerContrpller.moveV = 0;
        playerContrpller.moveH = 0;
        Rigidbody2D rig2D = transform.GetComponent<Rigidbody2D>();

        curHealth -= damage;
        HurtShader();
        if (curHealth <= 0)
        {
            curHealth = 0;
        }
        rig2D.AddForce(direction, ForceMode2D.Impulse);
        animator.SetTrigger("GetDamage");
    }
    private void HurtShader()
    {
        sp.material.SetFloat("_FlashAmount", 1);

        hurtCounter = hurtLength;
    }
    private void ZombieAttack(Collider2D target)
    {
        Vector2 targerDir = target.transform.position - transform.position;

        if (!isDisable) 
        {
            if (targerDir.x <= 0) //Left
            {
                npcSprite.localScale = new Vector2(-originalScaleX, npcSprite.localScale.y);
            }
            else //Right
            {
                npcSprite.localScale = new Vector2(originalScaleX, npcSprite.localScale.y);
            }
            if (!attacked)
            {
                hAttackSp.SetActive(true);
                attacked = true;
            }
        }
        
    }

    public void changeMode(int modeIndex)
    {
        if (modeIndex == 0)
        {
            curMode = npcMode.follow;
            npcCommand.SetActive(false);
        }
        else if (modeIndex == 1)
        {
            curMode = npcMode.stay;
            npcCommand.SetActive(false);
        }
    }
    void OnDrawGizmosSelected()
    {
        if (curType == characterType.zombie)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, hearRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
        else if (curType == characterType.human)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, rescueRadius);
        }
    }
}
