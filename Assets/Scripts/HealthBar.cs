using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine. UI;

public class HealthBar : MonoBehaviour
{
    PlayerStats playerStats;
    public Image hpImage;
    public Image hpEffectImage;
    public float hp;
    [SerializeField] private float maxHP;
    [SerializeField] private float hurtSpeed=0.005f;

    private void Start()
    {
        playerStats = FindObjectOfType<PlayerStats>();
        hp = playerStats.curHealth;
    }
    private void Update()
    {
        hpImage.fillAmount = hp / playerStats.maxHealth;
        if(hpEffectImage.fillAmount >hpImage.fillAmount)
        {
            hpEffectImage.fillAmount -= hurtSpeed;
        }
        else
        {
            hpEffectImage.fillAmount = hpImage.fillAmount;
        }
    }
}
