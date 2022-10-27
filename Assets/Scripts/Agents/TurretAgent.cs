using UnityEngine;

public class TurretAgent : AIAgent
{
    #region MonoBehaviour

    private new void Start()
    {
        base.Start();

        m_gunTransform = transform.Find("Body/Gun");
        if (m_gunTransform == null)
            Debug.Log("could not find gun transform");

        m_currentHealth = m_maxHealth;
    }


    private new void Update()
    {
        base.Update();

        CheckTarget();
    }

    #endregion

    #region Functions

    public override void ShootAtTarget()
    {
        base.ShootAtTarget();
    }
    public override float HasTarget()
    {
        return base.HasTarget();
    }

    #endregion
}
