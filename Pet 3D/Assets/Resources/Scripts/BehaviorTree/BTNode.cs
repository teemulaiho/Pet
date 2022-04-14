using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTNode
{
    public enum Result { Running, Failure, Success };
    public Result result = Result.Failure;

    protected virtual Result Evaluate() { return Result.Failure; }

    public Result Execute()
    {
        return Evaluate();
    }
}
