using UnityEngine;
using System.Collections.Generic;

interface IDamageable
{
    public CapsuleCollider DamageCollider { get; }

    void AddDamage(int damage, Agent attacker);
}
