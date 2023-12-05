using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public string unitName;

    public string unitRace;

    public int unitLevel;

    public int damage;
    public string target;

    public int initiative;

    public int maxHP;
    public int currentHP { get; private set; }
    public int index;
    public bool isAlive { get; private set; }

    public bool defending;
    public bool waited;
    public float animationAttackTimer { get; private set; }

    public UnitHUD HUD { get; set; }

    public Sprite sprite;
    public Sprite spriteSmall;

    public GameObject unitModel;

    private void Awake()
    {
        currentHP = maxHP;
        isAlive = true;
        defending = false;
        waited = false;
        animationAttackTimer = unitModel.GetComponent<UnitModel>().animationAttackTimer;
    }
    
    public void TakeDamage(int damage, UnitHUD targetHUD)
    {
        if (damage == 0)
            return;
        damage += Random.Range(0, 6);
        if (defending)
            damage /= 2;
        damage = Mathf.Clamp(damage, 0, currentHP);
        currentHP -= damage;

        if (currentHP <= 0)
            isAlive = false;

        targetHUD.AffectedInCombat("damage", damage, this);
    }

    public void InvertDeath()
    {
        isAlive = !isAlive;
    }

    public void SetIndex(int unitIndex)
    {
        index = unitIndex;
    }

    public void heal()
    {
        currentHP = maxHP;
    }
}