using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIAgent))]
public class UtilityAI : MonoBehaviour
{
    public UAI_ActionSet Actions;

    private AIAgent agent = null;

    struct BestValue
    {
        public float heuristic;
        public UAI_Action action;
    }

    private void Start()
    {
        agent = GetComponent<AIAgent>();

        Actions = Instantiate(Actions);

        if (Actions != null)
            Actions.Setup();
    }

    public void Update()
    {
        BestValue bestValue;
        bestValue.heuristic = 0f;
        bestValue.action = null;

        foreach (UAI_Action action in Actions.actions)
        {
            if (!action.consideration.IsValid())
                continue;

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
