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

    public string ComponentName = string.Empty;
    public string MethodName = string.Empty;

    public int ComponentIndex;
    public int MethodIndex;

    public void Setup(UtilityAI utilityAI)
    {
        Component = utilityAI.GetComponent(ComponentName);
        MethodInfo = Component.GetType().GetMethod(MethodName);
    }

    public void UpdateMethodInfo(string componentName, string methodName)
    {
        ComponentName = componentName;
        MethodName = methodName;
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
