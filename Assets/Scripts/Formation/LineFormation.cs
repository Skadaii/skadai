using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LineFormation", menuName = "FormationRules/Line", order = 2)]
public class LineFormation : FormationRule
{
    [SerializeField]
    private int unitsPerLine = 2;

    [SerializeField]
    private float lineSpacing = 1.5f;

    [SerializeField]
    private float columnSpacing = 1.5f;

    [SerializeField]
    private Vector3 localOffset = Vector3.zero;

    override public Vector3 ComputePosition(Vector3 center, Quaternion rotation, int index)
    {
        int verticalIndex = Mathf.FloorToInt(index / unitsPerLine);

        int hDirSign = index % 2 * 2 - 1;

        int horizontalIndex = Mathf.FloorToInt((index % unitsPerLine + 1) * 0.5f);

        //float evenCountOffset = ((unitsPerLine + 1) % 2 + index % 2) * 0.5f;

        Vector3 horizontalDirection = (hDirSign * (horizontalIndex)) * columnSpacing * Vector3.right;
        Vector3 verticalDirection = verticalIndex * lineSpacing * Vector3.back;

        Vector3 direction = rotation * (horizontalDirection + verticalDirection + localOffset);

        return center + direction;
    }
}
