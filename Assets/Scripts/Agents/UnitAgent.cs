using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


[RequireComponent(typeof(SphereCollider))]
public class UnitAgent : AIAgent
{
    [Header("Perception Ranges")]

    [SerializeField]
    float HearingRadius = 10f;

    [SerializeField]
    float SightAngle = 0.5f;

    [SerializeField]
    float PrivacyRadius = 2f;

    NavMeshAgent NavMeshAgentInst;
    Material MaterialInst;

    private void SetMaterial(Color col)
    {
        MaterialInst.color = col;
    }
    public void SetWhiteMaterial() { SetMaterial(Color.white); }
    public void SetRedMaterial() { SetMaterial(Color.red); }
    public void SetBlueMaterial() { SetMaterial(Color.blue); }
    public void SetYellowMaterial() { SetMaterial(Color.yellow); }

    #region MonoBehaviour

    protected new void Awake()
    {
        base.Awake();

        NavMeshAgentInst = GetComponent<NavMeshAgent>();

        Renderer rend = transform.Find("Body").GetComponent<Renderer>();
        MaterialInst = rend.material;

        GunTransform = transform.Find("Body/Gun");
        if (GunTransform == null)
            Debug.Log("could not find gun transform");

        Trigger.radius = Mathf.Max(HearingRadius, SightAngle, PrivacyRadius);

    }

    protected new void OnEnable()
    {
        base.OnEnable();
        OnHit.AddListener(SetTargetToAgressor);
    }

    protected new void OnDisable()
    {
        base.OnDisable();
        OnHit.RemoveListener(SetTargetToAgressor);
    }

    #endregion

    #region PerceptionMethods

    public bool IsInIntimateZone(Agent otherAgent)
    {
        if (!AgentTrespassers.Contains(otherAgent)) return false;

        float distance = Vector3.Magnitude(otherAgent.transform.position - transform.position);

        return distance < PrivacyRadius;
    }

    public bool IsInSightZone(Agent otherAgent)
    {
        if (!AgentTrespassers.Contains(otherAgent)) return false;

        Vector3 dir = Vector3.Normalize(otherAgent.transform.position - transform.position);
        float dot = Vector3.Dot(dir, transform.forward);

        return SightAngle < dot;
    }

    public bool IsInHearingZone(Agent otherAgent)
    {
        if (!AgentTrespassers.Contains(otherAgent)) return false;

        float distance = Vector3.Magnitude(otherAgent.transform.position - transform.position);

        return distance < HearingRadius;
    }

    #endregion

    #region MoveMethods
    public void StopMove()
    {
        NavMeshAgentInst.isStopped = true;
    }
    public void MoveTo(Vector3 dest)
    {
        NavMeshAgentInst.isStopped = false;
        NavMeshAgentInst.SetDestination(dest);
    }

    #endregion

    #region ActionMethods

    private void SetTargetToAgressor()
    {
        if(Target == null)
        {
            Target = Aggressor;
        }
    }

    #endregion

    #region UtilsMethod

    #endregion
}
