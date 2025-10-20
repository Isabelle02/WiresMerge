using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogSystem
{
    private DialogGraph _graph;

    public DialogNode CurrentRootNode { get; private set; }
    public List<DialogNode> NextDialogNodes { get; private set; } = new List<DialogNode>();

    public Action OnNextStep { get; set; }

    public DialogSystem(DialogGraph dialogGraph)
    {
        _graph = dialogGraph;
    }

    public void Start()
    {
        NextStep(_graph.RootNode);
    }

    public bool NextStep(DialogNode node = null)
    {
        if (node == null)
            return false;

        var children = _graph.GetChildren(node);
        if ((node.IsPlayer && children.Count == 0))
            return false;

        Debug.Log(node.NodeID);

        CurrentRootNode = node;
        NextDialogNodes = children;
        if (CurrentRootNode.IsPlayer && NextDialogNodes.Any(n => !n.IsPlayer))
            return NextStep(NextDialogNodes.First());
        else
            OnNextStep?.Invoke();

        return true;
    }
}
