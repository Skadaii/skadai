using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

[System.Serializable]
public class UAI_Method
{
    public Component Component;
    public MethodInfo MethodInfo;

    public bool Show = false;

    public string ComponentName = string.Empty;
    public string MethodName = string.Empty;

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

    public void Invoke()
    {
        MethodInfo.Invoke(Component, null);
    }
}
