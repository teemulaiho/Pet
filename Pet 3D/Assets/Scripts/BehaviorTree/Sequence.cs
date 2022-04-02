using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Composite
{
    public Sequence(List<BTNode> nodes) : base(nodes)
    {

    }
    protected override Result Evaluate()
    {
        foreach (BTNode node in children)
        {
            Result r = node.Execute();

            if (r != Result.Success)
                return r;

            if (children.IndexOf(node) == children.Count - 1)
                return Result.Success;
        }

        return Result.Success;
    }
}
