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

    private DialogGraph Config
    {
        get
        {
            if (_graph == null)
                _graph = Resources.Load<DialogGraph>("Dialogs/DialogGraph");

            return _graph;
        }
    }

    public void Start(int nodeId)
    {
        NextStep(Config.GetNode(nodeId));
    }

    public bool NextStep(DialogNode node = null)
    {
        if (node == null)
            return false;

        var children = Config.GetChildren(node);
        if ((node.IsPlayer && children.Count == 0))
            return false;

        Debug.Log(node.Id);

        CurrentRootNode = node;
        NextDialogNodes = children;
        if (CurrentRootNode.IsPlayer && NextDialogNodes.Any(n => !n.IsPlayer))
            return NextPersNode();
        else
            OnNextStep?.Invoke();

        LevelManager.LastDialogNodeId = node.Id;
        return true;
    }

    public bool NextPersNode()
    {
        return NextStep(NextDialogNodes.FirstOrDefault(n => n.Available));
    }
}
