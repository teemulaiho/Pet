using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Composite : BTNode
{
    protected List<BTNode> children = new List<BTNode>();

    public Composite(List<BTNode> nodes)
    {
        children = nodes;
    }
}
