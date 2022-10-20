using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;

namespace FSMMono
{
    [RequireComponent(typeof(SphereCollider))]
    public class AIAgent : Agent
    {

        [SerializeField]
        float BulletPower = 1000f;
        [SerializeField]
        GameObject BulletPrefab;

        [SerializeField]
        Slider HPSlider = null;

        [SerializeField]
        float HearingRadius = 10f;

        [SerializeField]
        float SightAngle = 0.5f;

        [SerializeField]
        float PrivacyRadius = 2f;

        Transform GunTransform;
        NavMeshAgent NavMeshAgentInst;
        Material MaterialInst;

        SphereCollider Trigger;

        List<GameObject> TriggerTrespasser = new List<GameObject>();

        private void SetMaterial(Color col)
        {
            MaterialInst.color = col;
        }
        public void SetWhiteMaterial() { SetMaterial(Color.white); }
        public void SetRedMaterial() { SetMaterial(Color.red); }
        public void SetBlueMaterial() { SetMaterial(Color.blue); }
        public void SetYellowMaterial() { SetMaterial(Color.yellow); }

        #region MonoBehaviour

        private void Awake()
        {
            CurrentHP = MaxHP;

            NavMeshAgentInst = GetComponent<NavMeshAgent>();

            Renderer rend = transform.Find("Body").GetComponent<Renderer>();
            MaterialInst = rend.material;

            GunTransform = transform.Find("Body/Gun");
            if (GunTransform == null)
                Debug.Log("could not find gun transform");

            if (HPSlider != null)
            {
                HPSlider.maxValue = MaxHP;
                HPSlider.value = CurrentHP;
            }

            Trigger = gameObject.GetComponent<SphereCollider>();
            Trigger.isTrigger = true;
            Trigger.radius = Mathf.Max(HearingRadius, SightAngle, PrivacyRadius);

        }
        private void Start()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out AIAgent agent) || other.gameObject.TryGetComponent(out PlayerAgent player))
            {
                TriggerTrespasser.Add(other.gameObject);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out AIAgent agent) || other.gameObject.TryGetComponent(out PlayerAgent player))
            {
                TriggerTrespasser.Remove(other.gameObject);
            }
        }

        private void Update()
        {
        }

        private void OnDrawGizmos()
        {
        }

        #endregion

        #region PerceptionMethods

        public bool IsInIntimateZone(GameObject other)
        {
            if (!TriggerTrespasser.Contains(other)) return false;

            float distance = Vector3.Magnitude(other.transform.position - transform.position);

            return distance < PrivacyRadius;
        }

        public bool IsInSightZone(GameObject other)
        {
            if (!TriggerTrespasser.Contains(other)) return false;

            Vector3 dir = Vector3.Normalize(other.transform.position - transform.position);
            float   dot = Vector3.Dot(dir, transform.forward);

            return SightAngle < dot;
        }

        public bool IsInHearingZone(GameObject other)
        {
            if (!TriggerTrespasser.Contains(other)) return false;

            float distance = Vector3.Magnitude(other.transform.position - transform.position);

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
        public bool HasReachedPos()
        {
            return NavMeshAgentInst.remainingDistance - NavMeshAgentInst.stoppingDistance <= 0f;
        }

        #endregion

        #region ActionMethods
        protected override void OnHealthChange()
        {
            if (HPSlider != null)
            {
                HPSlider.value = CurrentHP;
            }
        }

        void ShootToPosition(Vector3 pos)
        {
            // look at target position
            transform.LookAt(pos + Vector3.up * transform.position.y);

            // instantiate bullet
            if (BulletPrefab)
            {
                GameObject bullet = Instantiate<GameObject>(BulletPrefab, GunTransform.position + transform.forward * 0.5f, Quaternion.identity);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * BulletPower);
            }
        }
        #endregion

        #region UtilsMethod

        #endregion
    }
}