using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSMMono
{
    public class HealerAgent : AIAgent
    {
        [SerializeField] private int healPower;
        
        void Heal(Agent target)
        {
            target.AddHealth(healPower);
        }
    }
}