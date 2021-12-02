using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character", fileName = "New Character")]
public class CharacterInfo : ScriptableObject
{
    public Sprite sprite;
    public RuntimeAnimatorController animatorController;

    public float viewRange;
    public float attackDamage;
}
