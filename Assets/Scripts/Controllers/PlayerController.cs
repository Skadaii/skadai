using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables

    [SerializeField]
    float moveSpeed = 6f;

    PlayerAgent Player;

	Vector3 velocity;

    private Action<Vector3> OnMouseLeftClicked;
    private Action<Vector3> OnMouseRightClicked;

    private Movement movement;

    #endregion


    #region MonoBehaviour

    void Start ()
    {
        Player = GetComponent<PlayerAgent>();
        movement = GetComponent<Movement>();

        OnMouseLeftClicked  += Player.Shoot;
        OnMouseRightClicked += Player.NPCShootToPosition;
    }

    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale != 0f) GameInstance.Instance.Pause();
            else GameInstance.Instance.Resume();
        }

        if (Time.timeScale == 0f) return;

        int floorLayer = 1 << LayerMask.NameToLayer("Floor");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastInfo;
        Vector3 targetPos = Vector3.zero;

        if (Physics.Raycast(ray, out raycastInfo, Mathf.Infinity, floorLayer))
        {
            Vector3 newPos = raycastInfo.point;
            targetPos = newPos;
            targetPos.y += 0.1f;

            Player.AimAtPosition(targetPos);
        }

        velocity = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical")).normalized * moveSpeed;

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseLeftClicked(targetPos);
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnMouseRightClicked(targetPos);
        }
    }

	void FixedUpdate()
    {
        movement.MoveToward(velocity);
	}

    #endregion
}