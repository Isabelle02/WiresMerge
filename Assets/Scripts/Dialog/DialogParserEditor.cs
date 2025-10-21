#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogGraph))]
public class DialogParserEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DialogGraph graph = (DialogGraph)target;
        GUILayout.Space(10);
        EditorGUILayout.HelpBox("This parser works using WebClient.", MessageType.Info);
        GUILayout.Space(10);
        if (GUILayout.Button("Load from Google Sheets"))
        {
            var success = DialogParser.LoadAndParseFromGoogleSheets();
            if (success)
            {
                graph.Set(DialogParser.DialogGraph);
                EditorUtility.DisplayDialog("Success", $"Successfully loaded {graph.Nodes.Count} dialog nodes", "OK");
                EditorUtility.SetDirty(graph);
                AssetDatabase.SaveAssets();
            }
            else
                EditorUtility.DisplayDialog("Error", "Failed to load dialog data from Google Sheets", "OK");
        }

        GUILayout.Space(5);
        if (GUILayout.Button("Clear Data"))
        {
            graph.ClearData();
            EditorUtility.SetDirty(graph);
            AssetDatabase.SaveAssets();
        }

        GUILayout.Space(15);
        GUILayout.Label("Statistics:", EditorStyles.boldLabel);
        GUILayout.Label($"Loaded Nodes: {graph.Nodes.Count}");

        var rootNode = graph.RootNode;
        if (rootNode != null)
        {
            GUILayout.Label($"Root Node: {rootNode.NodeID} - {rootNode.Speaker}: {rootNode.Text}");
            if (rootNode?.ChildrenIds?.Count > 0)
            {
                GUILayout.Label($"Root has {rootNode.ChildrenIds.Count} children:");
                var children = graph.GetChildren(rootNode);
                foreach (var child in children)
                    GUILayout.Label($"  - {child.NodeID}: {child.Speaker}: {child.Text}");
            }
        }
        else
            GUILayout.Label("No root node found");
    }
}
#endif