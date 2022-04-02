using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorator : BTNode
{
    protected BTNode child;

    public Decorator(BTNode _child)
    {
        child = _child;
    }
}
