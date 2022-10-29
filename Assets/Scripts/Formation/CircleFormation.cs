using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CircleFormation", menuName = "FormationRules/Circle", order = 2)]
public class CircleFormation : FormationRule
{
    [SerializeField]
    private int unitsPerCircle = 2;

    [SerializeField]
    private bool followRotation = true;

    [SerializeField]
    private float circleSpacing = 1.5f;

    [SerializeField]
    private float baseSpacing = 1.5f;

    [SerializeField]
    private Vector3 worldOffset = Vector3.zero;

    [SerializeField]
    private Vector3 localOffset = Vector3.zero;

    [SerializeField]
    private float baseAngleOffset = 0f;

    [SerializeField]
    private float lineAngleOffset = Mathf.PI * 0.5f;

    override public Vector3 ComputePosition(Vector3 center, Quaternion rotation, int index)
    {
        float unitsCount = Math.Max(unitsPerCircle, 1);

        int lineIndex = Mathf.FloorToInt(index / unitsCount);

        float positionAngle = Mathf.PI * 2.0f / unitsCount * index;
        float angleOffset = baseAngleOffset + lineAngleOffset * lineIndex;
        float finalAngle = positionAngle + angleOffset;

        float spacing = baseSpacing + lineIndex * circleSpacing;

        Vector3 finalOffset = worldOffset + rotation * localOffset;

        Vector3 circularOffset = (Vector3.right * Mathf.Cos(finalAngle) + Vector3.forward * Mathf.Sin(finalAngle)) * spacing;

        if (followRotation)
            circularOffset = rotation * circularOffset;

        return finalOffset + center + circularOffset;
    }
}
