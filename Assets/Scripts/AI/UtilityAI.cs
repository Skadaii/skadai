using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAI : MonoBehaviour
{
    public string ActionSetName = "NewActionSet";
    public UAI_ActionSet ActionSet;

    struct BestValue
    {
        public float heuristic;
        public UAI_Action action;
    }

    private void Start()
    {
        ActionSet = Instantiate(ActionSet);

        if (ActionSet != null)
            ActionSet.Setup(this);
    }

    public void Update()
    {
        BestValue bestValue;
        bestValue.heuristic = 0f;
        bestValue.action = null;

        foreach (UAI_Action action in ActionSet.actions)
        {
            float newHeuristic = action.consideration.Evaluate();
            if (newHeuristic > bestValue.heuristic)
            {
                bestValue.heuristic = newHeuristic;
                bestValue.action = action;
            }
        }

        if (bestValue.action != null)
            bestValue.action.InvokeMethods();
    }
}
