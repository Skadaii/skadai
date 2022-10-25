using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UAI_Action
{
    public string actionName = "Action";

    public bool Show = false;
    public bool ShowMethods = false;

    public List<UAI_Method> methods = new List<UAI_Method>();
    public UAI_Consideration consideration = new UAI_Consideration();

    public void InvokeMethods()
    {
        foreach (UAI_Method method in methods)
            method.Invoke();
    }
}
