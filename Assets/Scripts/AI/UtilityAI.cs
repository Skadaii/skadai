using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAI : MonoBehaviour
{
    public string ActionSetName = "NewActionSet";

    public bool Show = false;
    public List<UAI_ActionSet> ActionSetList;

    private void Start()
    {
        List<UAI_ActionSet> actionSetListInstantiate = new List<UAI_ActionSet>();

        foreach (UAI_ActionSet actionSet in ActionSetList)
        {
            UAI_ActionSet actionSetInstantiate = Instantiate(actionSet);
            actionSetListInstantiate.Add(actionSetInstantiate);

            if (actionSetInstantiate != null)
                actionSetInstantiate.Setup(this);
        }

        ActionSetList = actionSetListInstantiate;
    }

    public void Update()
    {
        foreach (UAI_ActionSet actionSet in ActionSetList)
        {
            if (!actionSet.ExecuteInUpdate)
                continue;

            UAI_Action action = GetBestActionFromActionSet(actionSet);

            if (action != null)
                action.InvokeMethods();
        }
    }

    private UAI_Action GetBestActionFromActionSet(UAI_ActionSet actionSet)
    {
        float heuristic = 0f;
        UAI_Action bestAction = null;

        foreach (UAI_Action action in actionSet.Actions)
        {
            float newHeuristic = action.consideration.Evaluate();
            if (newHeuristic > heuristic)
            {
                heuristic = newHeuristic;
                bestAction = action;
            }
        }

        return bestAction;
    }

    public bool GetBestActionFromActionSet(string actionSetName, out UAI_Action action)
    {
        foreach (UAI_ActionSet actionSet in ActionSetList)
        {
            if (actionSet.ActionSetName == actionSetName)
            {
                action = GetBestActionFromActionSet(actionSet);
                return true;
            }
        }

        action = null;
        return false;
    }
}
