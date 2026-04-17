using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct HealthSystemInfo
{
    public int currentHealth;
    public int recentDamage;
};

public class HealthSystem : MonoBehaviour
{
    [SerializeField] protected int maxHp = 3;
    [SerializeField] protected int hp = 3;
    
    [SerializeField] protected ValueBar healthBar;

    [SerializeField] protected UnityEvent<int> OnDamaged;
    [SerializeField] protected UnityEvent<int> OnHealed;
    [SerializeField] protected UnityEvent OnZero;

    /*
    //Event Setup
    public delegate void HealthHandler(HealthSystemInfo info);
    public event HealthHandler OnDamaged;
    public event HealthHandler OnZero;
    */
    
    protected virtual void Start()
    {
        //Health Bar
        if (healthBar != null)
        {
            healthBar.SetMax(maxHp, hp);
        }
    }

    public virtual void Damage(int hpAmount)
    {
        hp -= hpAmount;
        //Debug.Log("hp amount changed by " + hpAmount + " and is now " + hp);

        //tell any subscriber to this event that damage happened!
        OnDamaged?.Invoke(hpAmount);
        
        //Health Bar
        UpdateHealthBar();

        //If we hit zero health, raise the HitZero event with all that registered
        if (hp <= 0)
        {
            OnZero?.Invoke();
        }
    }

    public virtual void Heal(int hpAmount)
    {
        hp += hpAmount;
        OnHealed?.Invoke(hpAmount);
        UpdateHealthBar();
    }

    protected virtual void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.Set(hp);
    }
}