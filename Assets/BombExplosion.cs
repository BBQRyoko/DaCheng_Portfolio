using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplosion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Boss"))
        {
            BossStats bossStats = other.GetComponent<BossStats>();
            if (bossStats.isProtected) 
            {
                bossStats.curShieldHealth -= 1;
                bossStats.dashingDamager.SetActive(false);
            }
        }
    }
}
