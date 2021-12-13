using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : MonoBehaviour
{
    public enum RunType { Static, Move, Rotate };
    public RunType runType;

    public Transform firePoint;
    [SerializeField] private float distance;
    public LayerMask objects;

    [Header("Laser")]
    private LineRenderer lineRenderer;
    [SerializeField] private Gradient red, green, blue;

    public List<Transform> patrolPos = new List<Transform>();
    public int curPatrolIndex = 0;
    [SerializeField] float moveSpeed;

    [SerializeField] float laserDamage;

    private void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();

        if (runType == RunType.Move)
        {
            foreach (Transform child in gameObject.transform.parent) //巡逻模式
            {
                if (child.tag != "LaserTower")
                {
                    patrolPos.Add(child);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //静止模式
        if ((int)runType == 0)
        {
            laserDetect();
        }
        //移动模式
        if ((int)runType == 1)
        {
            laserDetect();
            PatrolMove();
        }
        //旋转模式
        if ((int)runType == 2) 
        {
        
        }
    }

    void laserDetect()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(firePoint.position, -transform.right, distance, objects);
        
        if (hitInfo.collider != null)
        {
            lineRenderer.SetPosition(1, hitInfo.point); //line终点
            lineRenderer.colorGradient = green;
            if (hitInfo.collider.CompareTag("Player"))
            {
                lineRenderer.SetPosition(1, hitInfo.point); //line终点
                lineRenderer.colorGradient = red;
                if (!hitInfo.collider.GetComponent<CharacterStats>().isGetingDamage) 
                {
                    hitInfo.collider.GetComponent<CharacterStats>().getDamage(laserDamage, new Vector2(0, 0));
                }
            }
            lineRenderer.SetPosition(0, firePoint.position); //line初始点
        }
    }

    void PatrolMove()
    {
        float distanceFromTarget = Vector2.Distance(patrolPos[curPatrolIndex].position, transform.position);

        if (distanceFromTarget > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, patrolPos[curPatrolIndex].position, moveSpeed * Time.deltaTime);
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
