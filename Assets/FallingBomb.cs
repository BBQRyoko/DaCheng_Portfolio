using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBomb : MonoBehaviour
{
    [SerializeField] GameObject faliingBomb_Prefab;
    [SerializeField] GameObject poisionArea_Prefab;
    GameObject bombObject;
    [SerializeField] Transform fallingPos;

    
    // Start is called before the first frame update
    void Start()
    {
        bombObject = Instantiate(faliingBomb_Prefab, new Vector2(transform.position.x, transform.position.y + 6), Quaternion.EulerRotation(0,0,-90));
    }

    // Update is called once per frame
    void Update()
    {
        if (bombObject.transform.position.y <= transform.position.y) 
        {
            Destroy(bombObject);
            GameObject pos = Instantiate(poisionArea_Prefab, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
            pos.transform.parent = null;
            Destroy(gameObject);
        }
    }
}
