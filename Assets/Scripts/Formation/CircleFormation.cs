using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CircleFormation", menuName = "FormationRules/Circle", order = 2)]
public class CircleFormation : FormationRule
{
    [SerializeField]
    private float unitsPerCircle = 2f;

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

    override public Vector3 ComputePosition(Transform center, int index)
    {
        int lineIndex = Mathf.FloorToInt(index / unitsPerCircle);

        float positionAngle = Mathf.PI * 2.0f / unitsPerCircle * index;
        float angleOffset = baseAngleOffset + lineAngleOffset * lineIndex;
        float finalAngle = positionAngle + angleOffset;

        float spacing = baseSpacing + lineIndex * circleSpacing;

        Vector3 finalOffset = worldOffset + center.TransformVector(localOffset);

        return finalOffset + center.position +
                (center.right * Mathf.Cos(finalAngle) +
                center.forward * Mathf.Sin(finalAngle)) * spacing;
    }
}
