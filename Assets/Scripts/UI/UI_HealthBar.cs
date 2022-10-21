using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform HealthBarBackGround;
    [SerializeField] private RectTransform HealthBar;

    private Canvas canvas;
    private float CurrentPercent  = 1f;
    private float TargetedPercent = 1f;

    private Transform agentTransform;
    private Vector3   offset;

    private void Start()
    {
        offset = transform.localPosition;
        agentTransform = transform.parent;
        canvas = GetComponent<Canvas>();

        canvas.worldCamera = Camera.main;
        transform.SetParent(null);
    }

    private void Update()
    {
        transform.position = agentTransform.position + offset;

        CurrentPercent     = Mathf.Lerp(CurrentPercent, TargetedPercent, .05f);

        float width         = HealthBarBackGround.rect.width * CurrentPercent;
        HealthBar.sizeDelta = new Vector2(width, HealthBar.sizeDelta.y);

        transform.forward = Camera.main.transform.forward;
    }

    public void SetHealthPercentage(float healthPercent)
    {
        TargetedPercent = healthPercent;
    }

    public void Destroy()
    {
        StartCoroutine("DestroyAnimation", .15f);
    }

    IEnumerator DestroyAnimation(float time)
    {
        float startTime = Time.time;
        float startWidth = HealthBarBackGround.rect.width;

        while (time > Time.time - startTime)
        {
            HealthBarBackGround.sizeDelta = new Vector2(Mathf.Lerp(startWidth, 0f, (Time.time - startTime) / time), HealthBarBackGround.sizeDelta.y);
            yield return null;
        }

        Destroy(gameObject);
    }
}
