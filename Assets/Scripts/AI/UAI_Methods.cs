using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

[System.Serializable]
public class UAI_Method
{
    public MethodInfo MethodInfo;

    public int ComponentIndex;
    public int MethodIndex;

    public bool Show = false;

    public List<object> Args = new List<object>();

    public void UpdateMethodInfo(string componentName, string methodName, UtilityAI utilityAI)
    {
        Component component = utilityAI.GetComponent(componentName);
        MethodInfo = component.GetType().GetMethod(methodName);

        Type[] args = MethodInfo.GetGenericArguments();

        foreach (Type arg in args)
            Args.Add(arg);
    }
}
