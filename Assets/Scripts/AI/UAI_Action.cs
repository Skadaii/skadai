using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UAI_Action
{
    public string actionName = "Action";
    public bool show = false;
    public List<UAI_Method> methods;
    public UAI_Consideration consideration;
}
