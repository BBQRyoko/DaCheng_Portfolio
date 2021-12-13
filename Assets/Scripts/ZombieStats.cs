using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ZombieStats : CharacterStats
{
    PlayerContrpller playerContrpller;
    NavMeshAgent navMeshAgent;

    //当前的状态
    public bool isHuman;
    public bool isStayOnTrigger;
    public CharacterInfo[] statusList;
    public CharacterInfo curStatusInfo;
    [SerializeField] SpriteRenderer characterSprite;

    public enum EnemyType {Melee, Range};
    public EnemyType enemyType;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootPos;

    [SerializeField] float hearRadius; //听觉距离 (zombie状态专用)
    [SerializeField] float attackRadius; //攻击距离
    [SerializeField] float moveSpeed1;
    [SerializeField] float moveSpeed2;

    //Layers
    [SerializeField] LayerMask soundLayer; //声音层(Zombie)
    [SerializeField] LayerMask humanLayer; //人类层
    [SerializeField] LayerMask blockingLayer;

    [SerializeField] Collider2D curTarget; //当前目标
    [SerializeField] Collider2D[] soundSources; //声源列表
    [SerializeField] Collider2D humanTarget; //人类目标
    [SerializeField] Collider2D attackTarget;
    public bool isActive;
    float deadTime;

    [Header("UI")]
    public GameObject npcCommand; //指令UI
    public GameObject rescueButton; //UI 救助按钮
    public Image rescueBar; //救助进度图

    [Header("AttackAttribute")]
    public bool attacked;
    public float attackTimer;
    public float curAttack;

    public enum IdleType { Stay, Patrol };
    public IdleType idleType;
    public List<Transform> patrolPos = new List<Transform>();
    public int curPatrolIndex = 0;

    public bool isDisable; //是否无法移动

    public Collider2D zombieTarget;
    [SerializeField] float followSpeed; //跟随速度

    public enum npcMode { follow, stay }; //NPC的行动模式
    public npcMode curMode; //当前模式

    float originalScaleX;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        playerContrpller = FindObjectOfType<PlayerContrpller>();
        curHealth = maxHealth; //当前血量为最大值
        animator = GetComponentInChildren<Animator>();
        characterSprite = GetComponentInChildren<SpriteRenderer>();
        curAttack = attackTimer;
        originalScaleX = characterSprite.transform.localScale.x;
    }

    private void Start()
    {

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
    void Update()
    {
        animator.runtimeAnimatorController = curStatusInfo.animatorController;
        characterSprite.sprite = curStatusInfo.sprite;

        if (!isHuman)
        {
            ZombieUpdate();
        }
        else 
        {
            HumanUpdate();
        }
    }

    void ZombieUpdate()
    {
        gameObject.layer = 8;//所有僵尸所在物理层为第8层
        curStatusInfo = statusList[0];

        if (curHealth <= 0) //如果当前血量低于0=死亡
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
                animator.SetBool("preDead", false);
            }
        }

        if (!curTarget && !isDisable)
        {
            PatrolMove();
        }

        if (curTarget != null && isDisable == false && isActive == true && attacked == false) //当前有目标且僵尸可以移动时
        {
            Vector2 direction = new Vector2(curTarget.transform.position.x - transform.position.x, curTarget.transform.position.y - transform.position.y);

            float distance = Vector2.Distance(curTarget.transform.position, transform.position);

            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, distance, blockingLayer);

            if (hitInfo.collider == null)
            {
                transform.position = Vector2.MoveTowards(transform.position, curTarget.transform.position, moveSpeed2 * Time.deltaTime);//则僵尸向当前目标以moveSpeed2的速度移动
            }

            PatrolMove();
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
                curTarget = null;
                if (soundSources.Length > 0 && !curTarget && isActive)//声音检测到目标且不为当前目标
                {
                    int latestSound = soundSources.Length;

                    Vector2 direction = new Vector2(soundSources[latestSound - 1].transform.position.x - transform.position.x, soundSources[latestSound - 1].transform.position.y - transform.position.y);

                    float distance = Vector2.Distance(soundSources[latestSound - 1].transform.position, transform.position);

                    RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, distance, blockingLayer);

                    if (hitInfo.collider == null)
                    {
                        transform.position = Vector2.MoveTowards(transform.position, soundSources[latestSound - 1].transform.position, moveSpeed1 * Time.deltaTime);//僵尸就会向着最后出现的声音检测到的目标以moveSpeed1的速度移动
                    }
                }
            }

            if (attackTarget != null)
            {
                ZombieAttack(attackTarget);
            }
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
        curStatusInfo = statusList[1];

        ////NPC模式
        if (curMode == npcMode.follow) //若当前NPC的模式为跟随模式时
        {
            float distance = Vector2.Distance(transform.position, playerContrpller.transform.position);//NPC与玩家距离

            if (distance >= 2)
            {
                navMeshAgent.SetDestination(curTarget.transform.position);
                //transform.position = Vector2.MoveTowards(transform.position, playerContrpller.transform.position, followSpeed * Time.deltaTime);//当距离大于2时 则NPC以followSpeed的速递向玩家移动
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
        zombieTarget = Physics2D.OverlapCircle(transform.position, viewRadius, zombieLayer);
        if (zombieTarget != null && zombieTarget.GetComponent<ZombieStats>().isActive)
        {
            float distance = Vector2.Distance(transform.position, zombieTarget.transform.position);

            if (distance >= 1)
            {
                //transform.position = Vector2.MoveTowards(transform.position, zombieTarget.transform.position, followSpeed * Time.deltaTime);//当距离大于2时 则NPC以followSpeed的速递向玩家移动
            }
            else if (distance < 1 && distance > 0 && !isDisable)
            {
                if (!gameObject.CompareTag("Player"))
                {
                    //ZombieAttack(zombieTarget);
                }
            }
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

    private void ZombieAttack(Collider2D target)
    {
        Vector2 targerDir = target.transform.position - transform.position;
        float distance = Vector2.Distance(target.transform.position, transform.position);
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, targerDir, distance, blockingLayer);

        if (hitInfo.collider == null)
        {
            if (!isDisable && !attacked)
            {
                attacked = true;
                if (targerDir.x <= 0) //Left
                {
                    characterSprite.transform.localScale = new Vector2(-originalScaleX, characterSprite.transform.localScale.y);
                }
                else //Right
                {
                    characterSprite.transform.localScale = new Vector2(originalScaleX, characterSprite.transform.localScale.y);
                }

                //Melee
                if (enemyType == EnemyType.Melee)
                {
                    if (targerDir.y <= 0)
                    {
                        animator.SetTrigger("Down");
                    }
                    else
                    {
                        animator.SetTrigger("Up");
                    }
                }
                else if (enemyType == EnemyType.Range)
                {
                    animator.SetTrigger("Range");
                    targerDir.Normalize();
                    GameObject bullet = Instantiate(bulletPrefab, shootPos.position, Quaternion.identity);

                    float angle = Random.Range(-5f, 5f);
                    bullet.GetComponent<EnemyBullet>().SetSpeed(Quaternion.AngleAxis(angle, Vector3.forward) * targerDir);
                }
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

    public void npcTrigger(Transform triggerTransform) 
    {
        Vector2 direction = new Vector2(triggerTransform.position.x - transform.position.x, triggerTransform.position.y - transform.position.y);

        float distance = Vector2.Distance(triggerTransform.position, transform.position);

        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, distance, blockingLayer);

        if (hitInfo.collider == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, triggerTransform.position, followSpeed * Time.deltaTime);
            if (distance <= 0.001) 
            {
                isStayOnTrigger = true;
                curMode = npcMode.stay;
                playerContrpller.GetComponent<PlayerStats>().npcList.Remove(this);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!isHuman)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, hearRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
        else if (isHuman)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, viewRadius);
        }
    }
}
