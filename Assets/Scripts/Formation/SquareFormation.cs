using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SquareFormation", menuName = "FormationRules/Square", order = 2)]
public class SquareFormation : FormationRule
{
    [SerializeField]
    int UnitsPerLine = 4;
    [SerializeField]
    float UnitSpacing = 1.5f;
    override public Vector3 ComputePosition(Transform center, int index)
    {
        // set position in line
        int rowIndex = index % UnitsPerLine;
        int lineIndex = Mathf.FloorToInt(index / UnitsPerLine);
        Vector3 offset = center.right * Mathf.FloorToInt((rowIndex + 1) / 2) * UnitSpacing;

        // set to right or left from leader position
        if (index % 2 == 0)
            offset *= -1.0f;

        // add offset for each new line
        if (lineIndex > 0)
            offset -= center.forward * lineIndex * UnitSpacing;

        return center.position + offset;
    }
}
