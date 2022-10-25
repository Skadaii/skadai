using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class UAI_Consideration : UAI_Method
{
    public AnimationCurve AnimationCurve;

    public float Evaluate()
    {
        object value = MethodInfo.Invoke(Component, null);
        return AnimationCurve.Evaluate((float)value);
    }
}
