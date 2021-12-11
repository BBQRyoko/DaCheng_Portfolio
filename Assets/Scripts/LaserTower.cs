using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : MonoBehaviour
{
    public enum RunType { Static, Rotate };
    public RunType runType;

    public Transform firePoint;
    [SerializeField] private float distance;
    public LayerMask objects;

    [Header("Laser")]
    private LineRenderer lineRenderer;
    [SerializeField] private Gradient red, green, blue;

    private void Start()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //静止模式
        if ((int)runType == 0)
        {
            laserDetect();
        }
        //旋转模式
        if ((int)runType == 1)
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
            if (hitInfo.collider.tag == "Box")
            {
                lineRenderer.SetPosition(1, hitInfo.point); //line终点
                lineRenderer.colorGradient = red;
            }
            if (hitInfo.collider.tag == "Human")
            {
                lineRenderer.SetPosition(1, hitInfo.point); //line终点
                lineRenderer.colorGradient = blue;

            }
            lineRenderer.SetPosition(0, firePoint.position); //line初始点
        }
    }
}
