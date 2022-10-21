using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UAI_Actions", menuName = "ScriptableObjects/UAI_Actions", order = 1)]
public class UAI_ActionSet : ScriptableObject
{
    public List<UAI_Action> actions = new List<UAI_Action>();
    public UAI_Blackboard Blackboard;

    public void Setup()
    {
        if (Blackboard == null)
            return;

        Blackboard = Instantiate(Blackboard);
    }
}
