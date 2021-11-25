using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{


    public  Transform target;
    [SerializeField] private float smootSpeed;
    [SerializeField] private float minX,maxX,minY,maxY;
  
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    private void LateUpdate()
    {
        
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), smootSpeed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x,minX ,maxX ),
                                         Mathf.Clamp(transform.position.y, minY, maxY),
                                         transform.position.z);
        

    }
}
