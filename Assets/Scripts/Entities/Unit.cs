using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Movement))]
public class Unit : MonoBehaviour
{
    [HideInInspector] public Movement movement = null;
    [HideInInspector] public Agent agent = null;

    [HideInInspector] public Unit assignedHealer = null;
    [HideInInspector] public Unit assignedSupport = null;

    public UnitSquad m_Squad { get; private set; } = null;

    // Start is called before the first frame update
    void Awake()
    {
        movement = GetComponent<Movement>();
        agent = GetComponent<Agent>();

    }

    protected void OnEnable()
    {
        agent?.OnHit.AddListener(CallForHeal);
    }

    protected void OnDisable()
    {
        agent?.OnHit.RemoveListener(CallForHeal);
    }

    public void CallForHeal()
    {
        m_Squad.AssignHealerTo(this);
    }

    public virtual void SetSquad(UnitSquad squad) => m_Squad = squad;

    public virtual bool HasDuty() { return false; }
}
