using UnityEngine;

public class TurretAgent : AIAgent
{
    #region MonoBehaviour

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
