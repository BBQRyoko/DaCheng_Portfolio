using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine. UI;

public class HealthBar : MonoBehaviour
{
    public Image hpImage;
    public Image hpEffectImage;
    public float hp;
    [SerializeField] private float maxHP;
    [SerializeField] private float hurtSpeed=0.005f;

    private void Start()
    {
        hp = maxHP;
    }
    private void Update()
    {
        hpImage.fillAmount = hp / maxHP;
        if(hpEffectImage .fillAmount >hpImage .fillAmount)
        {
            hpEffectImage.fillAmount -= hurtSpeed;
        }
        else
        {
            hpEffectImage.fillAmount = hpImage.fillAmount;
        }
    }
}
