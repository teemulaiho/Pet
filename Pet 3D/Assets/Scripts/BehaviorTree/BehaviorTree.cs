using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTree
{
    BTNode rootNode;

    public BehaviorTree(BTNode root)
    {
        rootNode = root;
    }

    public void Run()
    {
        BTNode.Result result = rootNode.Execute();
    }
}