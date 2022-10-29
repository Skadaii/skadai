using UnityEngine;
using System.Collections.Generic;

interface IDamageable
{
    public CapsuleCollider DamageCollider { get; }
    public GameObject HurtFX { get; }

    void AddDamage(int damage, Agent attacker, RaycastHit hit, bool playFX = false);
}
