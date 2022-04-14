using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionNode : BTNode
{
    public delegate Result ActionNodeDelegate();

    private ActionNodeDelegate action;

    public ActionNode(ActionNodeDelegate _action)
    {
        action = _action;
    }

    protected override Result Evaluate()
    {
        return action();
    }
}
