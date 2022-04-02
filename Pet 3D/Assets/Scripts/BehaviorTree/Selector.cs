using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Composite
{
    public Selector(List<BTNode> nodes) : base(nodes)
    {

    }
    protected override Result Evaluate()
    {
        foreach (BTNode node in children)
        {
            Result r = node.Execute();

            if (r != Result.Failure)
                return r;

            if (children.IndexOf(node) == children.Count - 1)
                return Result.Failure;
        }

        return Result.Success;
    }
}
