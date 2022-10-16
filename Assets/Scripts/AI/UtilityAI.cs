using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FSMMono.AIAgent))]
public class UtilityAI : MonoBehaviour
{
    public UAI_Actions Actions;

    private FSMMono.AIAgent AIAgent = null;

    public void OnValidate()
    {
        AIAgent = GetComponent<FSMMono.AIAgent>();
        Debug.Log("Ai Agent setted");
    }
}
