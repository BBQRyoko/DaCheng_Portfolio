using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCache : MonoBehaviour
{
    [SerializeField] GameObject[] containedItem;
    [SerializeField] Sprite openedCache;
    [SerializeField] GameObject interactIcon;
    bool canOpen;
    bool isOpened;
    [SerializeField] float itemInitialSpeed;
    [SerializeField] float fallTime;


    // Update is called once per frame
    void Update()
    {
        if (canOpen && !isOpened) 
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Opened");
                transform.GetComponent<SpriteRenderer>().sprite = openedCache;
                interactIcon.SetActive(false);
                foreach (GameObject curItem in containedItem) 
                {
                    GameObject item = Instantiate(curItem, transform.position, transform.rotation);
                    float angel = Random.Range(-45f, 45f);
                    item.GetComponent<Rigidbody2D>().velocity = Quaternion.AngleAxis(angel, Vector3.forward) * Vector3.up * itemInitialSpeed;
                    item.GetComponent<Rigidbody2D>().gravityScale = 3;
                    item.GetComponent<BoxCollider2D>().isTrigger = true;
                    StartCoroutine(Stop(item));
                }
                isOpened = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isOpened) 
        {
            interactIcon.SetActive(true);
            canOpen = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            interactIcon.SetActive(false);
        }
    }

    IEnumerator Stop(GameObject item)
    {
        yield return new WaitForSeconds(fallTime);
        item.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        item.GetComponent<Rigidbody2D>().gravityScale = 0;
    }
}
