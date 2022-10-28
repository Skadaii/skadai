using UnityEngine;

public class TurretAgent : AIAgent
{
    private GameObject fireParticles;
    private GameObject fireParticlesInstance;

    #region MonoBehaviour

    private new void Awake()
    {
        base.Awake();

        fireParticles = Resources.Load<GameObject>("FXs/ParticleFire");
    }

    private new void Update()
    {
        base.Update();

        CheckTarget();
    }

    private new void OnDisable()
    {
        base.OnDisable();
    }

    private new void OnEnable()
    {
        base.OnEnable();

        if (fireParticlesInstance)
        {
            Destroy(fireParticlesInstance);
        }
    }

    #endregion

    #region Functions

    protected override void OnDeath()
    {
        base.OnDeath();

        if (fireParticles)
        {
            fireParticlesInstance = Instantiate(fireParticles, null);
            fireParticlesInstance.transform.position = transform.position;
        }
    }

    public override float HasTarget() => base.HasTarget();
    public override void ShootAtTarget() => base.ShootAtTarget();

    #endregion
}
