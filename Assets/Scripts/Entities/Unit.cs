using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Movement))]
public class Unit : MonoBehaviour
{
    #region Variables

    [HideInInspector] public Movement movement = null;
    [HideInInspector] public Agent agent = null;

    [HideInInspector] public HealerUnit  assignedHealer = null;
    [HideInInspector] public SupportUnit assignedSupport = null;

    public UnitSquad m_Squad { get; private set; } = null;

    private bool m_callForHeal = false;

    #endregion

    #region MonoBehaviour

    // Start is called before the first frame update
    protected void Awake()
    {
        movement = GetComponent<Movement>();
        agent    = GetComponent<Agent>();
    }

    protected void Start()
    {
        if(agent && m_Squad) agent.AgentTeam = m_Squad.SquadTeam;
    }

    protected void FixedUpdate()
    {
        if(m_callForHeal)
        {
            m_callForHeal = !m_Squad.AssignHealerTo(this);
        }
    }

    protected void OnEnable()
    {
        agent?.OnHit.AddListener(CallForHeal);
    }

    protected void OnDisable()
    {
        agent?.OnHit.RemoveListener(CallForHeal);

        if (assignedHealer) assignedHealer.Target   = null;
        if (assignedSupport) assignedSupport.Target = null;
    }

    #endregion

    #region Functions

    public void CallForHeal()
    {
        m_callForHeal = true;
    }

    public virtual void SetSquad(UnitSquad squad) => m_Squad = squad;

    public virtual bool HasDuty() { return false; }

    #endregion
}
