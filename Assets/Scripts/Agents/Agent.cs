using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Agent : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected int MaxHP = 100;
    protected bool IsDead = false;

    protected int CurrentHP;

    public void AddDamage(int amount)
    {
        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            IsDead = true;
            CurrentHP = 0;

            OnDeath();
        }

        OnHealthChange();
    }

    protected virtual void OnHealthChange() { }
    protected virtual void OnDeath() { }
}
