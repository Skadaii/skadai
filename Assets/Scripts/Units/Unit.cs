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

    public UnitSquad m_squad { get; private set; } = null;

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
        if(agent && m_squad) agent.AgentTeam = m_squad.SquadTeam;
    }

    protected void FixedUpdate()
    {
        if(m_callForHeal)
        {
            m_callForHeal = !m_squad.AssignHealerTo(this);
        }
    }

    protected void OnEnable()
    {
        agent?.OnHit.AddListener(CallForHeal);
        agent?.OnHit.AddListener(Alert);
    }

    protected void OnDisable()
    {
        agent?.OnHit.RemoveListener(CallForHeal);
        agent?.OnHit.RemoveListener(Alert);

        if (assignedHealer)  assignedHealer.Target  = null;
        if (assignedSupport) assignedSupport.Target = null;
    }

    #endregion

    #region Functions

    public void CallForHeal()
    {
        m_callForHeal = true;
    }

    private void Alert()
    {
        m_squad.ReceiveAlert(this);
    }

    public virtual void SetSquad(UnitSquad squad) => m_squad = squad;

    public virtual bool HasDuty() { return false; }

    #endregion
}
