using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class UAI_Methods : ISerializationCallbackReceiver
{
    private MethodInfo MethodInfo;

    public static List<string> StaticComponentsName = new List<string>();
    [HideInInspector] public List<string> ComponentsName = new List<string>();

    [CustomDropdown(typeof(UAI_Methods), "StaticComponentsName")]
    public string ComponentName;

    public static List<string> StaticMethodsName = new List<string>();
    [HideInInspector] public List<string> MethodsName = new List<string>();

    [CustomDropdown(typeof(UAI_Methods), "StaticMethodsName")]
    public string MethodName;

    public void OnBeforeSerialize()
    {
        StaticComponentsName = ComponentsName;
        StaticMethodsName = MethodsName;
    }

    public void OnAfterDeserialize() { }
}
