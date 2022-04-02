using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : Composite
{
    public Parallel(List<BTNode> nodes) : base(nodes)
    {

    }
    protected override Result Evaluate()
    {
        bool oneRunning = false;
        foreach (BTNode node in children)
        {
            Result r = node.Execute();

            if (r != Result.Failure)
                oneRunning = true;

            if (children.IndexOf(node) == children.Count - 1)
            {
                if (oneRunning)
                    return Result.Running;
                else
                    return Result.Failure;
            }
        }

        return Result.Failure;
    }
}
