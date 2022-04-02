using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inverter : Decorator
{
    public Inverter(BTNode _child) : base(_child)
    {

    }

    protected override Result Evaluate()
    {
        Result r = child.Execute();

        if (r != Result.Failure)
            return Result.Failure;

        return Result.Success;
    }
}