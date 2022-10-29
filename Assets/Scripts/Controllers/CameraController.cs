using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform m_target = null;
    [SerializeField] private float m_smoothingTime = 5f;

    [SerializeField] private Vector3 m_offset = new Vector3(0f, 13f, -7f);
    [SerializeField]  private GameObject m_cameraObject;

	private Vector3 m_smoothVel = Vector3.zero;

    public Transform Target { get { return m_target; } }
    void Start ()
	{
        if (m_target == null)
            return;
        transform.position = m_target.position + m_offset;
        transform.LookAt(m_target);
    }

    void FixedUpdate ()
	{
        if (m_target == null)
            return;
		Vector3 targetCamPos = m_target.position + m_offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetCamPos, ref m_smoothVel, m_smoothingTime);
    }

    public void Shake(float strength, float duration)
    {
        StartCoroutine(Shaking(strength, duration));
    }

    IEnumerator Shaking(float strength, float duration)
    {
        Vector3 originalPosition = m_cameraObject.transform.localPosition;
        float startTime = Time.time;

        while(Time.time - startTime < duration)
        {
            if (Time.timeScale != 0f)
            {
                float x = Random.Range(-1f, 1f) * strength;
                float y = Random.Range(-1f, 1f) * strength;

                m_cameraObject.transform.localPosition = new Vector3(x, y, originalPosition.z);
            }

            yield return null;
        }

        m_cameraObject.transform.localPosition = originalPosition;
    }
}
