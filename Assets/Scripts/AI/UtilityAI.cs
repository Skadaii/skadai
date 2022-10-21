using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FSMMono.AIAgent))]
public class UtilityAI : MonoBehaviour
{
    public UAI_Actions Actions;

    private FSMMono.AIAgent AIAgent = null;

    struct BestValue
    {
        public float heuristic;
        public UAI_Action action;
    }

    private void Start()
    {
        AIAgent = GetComponent<FSMMono.AIAgent>();

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
