using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityAI : MonoBehaviour
{
    public UAI_ActionSet Actions;


    struct BestValue
    {
        public float heuristic;
        public UAI_Action action;
    }

    private void Start()
    {
        Actions = Instantiate(Actions);

        if (Actions != null)
            Actions.Setup(this);
    }

    public void Update()
    {
        BestValue bestValue;
        bestValue.heuristic = 0f;
        bestValue.action = null;

        foreach (UAI_Action action in Actions.actions)
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
