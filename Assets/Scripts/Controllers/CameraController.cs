using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
	public Transform Target = null;
	public float SmoothingTime = 5f;

    [SerializeField]
	private Vector3 offset = new Vector3(0f, 13f, -7f);

	private Vector3 smoothVel = Vector3.zero;
    void Start ()
	{
        if (Target == null)
            return;
        transform.position = Target.position + offset;
        transform.LookAt(Target);
    }
	void FixedUpdate ()
	{
        if (Target == null)
            return;
		Vector3 targetCamPos = Target.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetCamPos, ref smoothVel, SmoothingTime);
    }
}
