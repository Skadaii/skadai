using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class UAI_Consideration
{
    public bool Show = false;

    public AnimationCurve AnimationCurve;

    public Component Component;
    public MethodInfo MethodInfo;

    public int ComponentIndex;
    public int MethodIndex;

    public void UpdateMethodInfo(string componentName, string methodName, UtilityAI utilityAI)
    {
        Component = utilityAI.GetComponent(componentName);
        MethodInfo = Component.GetType().GetMethod(methodName);
    }

    public float Evaluate()
    {
        object value = MethodInfo.Invoke(Component, null);
        return AnimationCurve.Evaluate((float)value);
    }

    public bool IsValid()
    {
        return MethodInfo != null && AnimationCurve.length != 0;
    }
}
