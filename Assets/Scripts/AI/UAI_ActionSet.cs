using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UAI_Actions", menuName = "ScriptableObjects/UAI_Actions", order = 1)]
public class UAI_ActionSet : ScriptableObject
{
    public List<UAI_Action> actions = new List<UAI_Action>();
    public UAI_Blackboard Blackboard;

    public void Setup(UtilityAI utilityAI)
    {
        if (Blackboard != null)
            Blackboard = Instantiate(Blackboard);

        foreach(UAI_Action action in actions)
        {
            foreach (UAI_Method method in action.methods)
                method.Setup(utilityAI);

            action.consideration.Setup(utilityAI);
        }
    }
}
