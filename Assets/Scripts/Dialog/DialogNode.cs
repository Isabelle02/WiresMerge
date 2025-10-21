using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogNode
{
    public int NodeID;
    public string Speaker;
    public string Text;
    public bool IsPlayer;
    public string Parameters;

    public List<int> ParentIds = new List<int>();
    public List<int> ChildrenIds = new List<int>();

    public string FormattedText
    {
        get
        {
            var paramNames = Parameters.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var args = new List<object>();
            foreach (var name in paramNames)
                args.Add(DynamicParameters.Get(name));

            return string.Format(Text, args.ToArray());
        }
    }

    public DialogNode(int nodeId, string speaker, string text, bool isPlayer, string parameters)
    {
        NodeID = nodeId;
        Speaker = speaker;
        Text = text;
        IsPlayer = isPlayer;
        Parameters = parameters;
    }
}

public class DialogRelation
{
    public int ParentID;
    public int ChildID;

    public DialogRelation(int parentId, int childId)
    {
        ParentID = parentId;
        ChildID = childId;
    }
}

[Serializable]
[CreateAssetMenu(fileName = "DialogGraph", menuName = "Dialog System/Dialog Graph")]
public class DialogGraph : ScriptableObject
{
    [NonSerialized]
    public List<DialogRelation> Relations = new List<DialogRelation>();
    public List<DialogNode> Nodes = new List<DialogNode>();
    public DialogNode RootNode;

    public void Set(DialogGraph graph)
    {
        Relations = graph.Relations;
        Nodes = graph.Nodes;
        RootNode = graph.RootNode;
    }

    public DialogNode GetNode(int id)
    {
        return Nodes.Count > id ? Nodes[id] : null;
    }

    public List<DialogNode> GetChildren(DialogNode node)
    {
        var children = new List<DialogNode>();
        foreach (var childId in node.ChildrenIds)
        {
            var child = GetNode(childId);
            if (child != null)
                children.Add(child);
        }

        return children;
    }

    public List<DialogNode> GetParents(DialogNode node)
    {
        var parents = new List<DialogNode>();
        foreach (var parentId in node.ParentIds)
        {
            var parent = GetNode(parentId);
            if (parent != null) 
                parents.Add(parent);
        }

        return parents;
    }

    public void ClearData()
    {
        Relations.Clear();
        Nodes.Clear();
        RootNode = null;
    }
}
