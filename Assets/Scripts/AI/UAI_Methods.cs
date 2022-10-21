using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

[System.Serializable]
public class UAI_Method
{
    public Component Component;
    public MethodInfo MethodInfo;

    public int ComponentIndex;
    public int MethodIndex;

    public bool Show = false;

    public void UpdateMethodInfo(string componentName, string methodName, UtilityAI utilityAI)
    {
        Component = utilityAI.GetComponent(componentName);
        MethodInfo = Component.GetType().GetMethod(methodName);
    }

    public void Invoke()
    {
        MethodInfo.Invoke(Component, null);
    }
}
