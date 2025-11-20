#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(LevelsConfig))]
public class LevelConfigEditor : Editor
{
    private LevelsConfig levelsConfig;
    private int selectedLevelIndex = 0;
    private Vector2 scrollPosition;
    private bool showWireCells = true;

    private void OnEnable()
    {
        levelsConfig = (LevelsConfig)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space();

        // Level selection
        DrawLevelSelection();

        EditorGUILayout.Space();

        if (levelsConfig.Levels.Count > 0 && selectedLevelIndex < levelsConfig.Levels.Count)
        {
            DrawSelectedLevel();
        }
        else
        {
            EditorGUILayout.HelpBox("No levels available. Add a new level or drag WireCell objects from scene.", MessageType.Info);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawLevelSelection()
    {
        EditorGUILayout.BeginHorizontal();

        // Level dropdown
        string[] levelOptions = levelsConfig.Levels.Select((level, index) => $"Level {level.LevelId} (Index: {index})").ToArray();
        selectedLevelIndex = EditorGUILayout.Popup("Selected Level", selectedLevelIndex, levelOptions);

        // Add level button
        if (GUILayout.Button("Add Level", GUILayout.Width(80)))
        {
            AddNewLevel();
        }

        // Remove level button
        if (levelsConfig.Levels.Count > 0 && GUILayout.Button("Remove", GUILayout.Width(80)))
        {
            RemoveLevel(selectedLevelIndex);
        }

        EditorGUILayout.EndHorizontal();

        // Clear all button
        if (levelsConfig.Levels.Count > 0)
        {
            if (GUILayout.Button("Clear All Levels"))
            {
                if (EditorUtility.DisplayDialog("Clear All Levels",
                    "Are you sure you want to clear all levels?", "Yes", "No"))
                {
                    levelsConfig.ClearData();
                    selectedLevelIndex = 0;
                    EditorUtility.SetDirty(levelsConfig);
                }
            }
        }
    }

    private void DrawSelectedLevel()
    {
        var selectedLevel = levelsConfig.Levels[selectedLevelIndex];

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Level Configuration - ID: {selectedLevel.LevelId}", EditorStyles.boldLabel);

        // Level ID
        selectedLevel.LevelId = EditorGUILayout.IntField("Level ID", selectedLevel.LevelId);

        // Dialog Node ID
        selectedLevel.DialogNodeId = EditorGUILayout.IntField("Dialog Node ID", selectedLevel.DialogNodeId);

        EditorGUILayout.Space();

        // Wire Cells section
        DrawWireCellsSection(selectedLevel);
    }

    private void DrawWireCellsSection(LevelConfig level)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        showWireCells = EditorGUILayout.Foldout(showWireCells, $"Wire Cells ({level.WireCells.Count})", true);

        if (showWireCells)
        {
            EditorGUILayout.Space();

            // Drag and drop area
            DrawDragDropArea(level);

            EditorGUILayout.Space();

            // Wire cells list
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(300));

            for (int i = 0; i < level.WireCells.Count; i++)
            {
                DrawWireCellItem(level.WireCells[i], i, level);
            }

            EditorGUILayout.EndScrollView();

            // Clear wire cells button
            if (level.WireCells.Count > 0)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Clear All Wire Cells"))
                {
                    if (EditorUtility.DisplayDialog("Clear Wire Cells",
                        "Are you sure you want to clear all wire cells?", "Yes", "No"))
                    {
                        level.WireCells.Clear();
                        EditorUtility.SetDirty(levelsConfig);
                    }
                }
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawDragDropArea(LevelConfig level)
    {
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag WireCell objects from scene here");

        Event evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (draggedObject is GameObject gameObject)
                        {
                            WireCell wireCell = gameObject.GetComponent<WireCell>();
                            if (wireCell != null)
                            {
                                AddWireCellToLevel(level, wireCell);
                            }
                        }
                    }

                    EditorUtility.SetDirty(levelsConfig);
                }
                break;
        }
    }

    private void DrawWireCellItem(WireCellData wireCellData, int index, LevelConfig level)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.BeginHorizontal();

        // Header with index and remove button
        EditorGUILayout.LabelField($"Wire Cell {index}", EditorStyles.boldLabel);

        if (GUILayout.Button("Remove", GUILayout.Width(60)))
        {
            level.WireCells.RemoveAt(index);
            EditorUtility.SetDirty(levelsConfig);
            return;
        }

        EditorGUILayout.EndHorizontal();

        // WireCell reference (read-only)
        EditorGUI.BeginChangeCheck();
        var newWireCell = (WireCell)EditorGUILayout.ObjectField("Scene Object", wireCellData.WireCell, typeof(WireCell), true);

        if (EditorGUI.EndChangeCheck())
        {
            ApplyWireCellSceneToData(level, wireCellData, newWireCell);
            Debug.Log($"Updated WireCell {newWireCell.name} to level {level.LevelId}");
        }

        // Position
        wireCellData.Position = EditorGUILayout.Vector3Field("Position", wireCellData.Position);

        // Shape Type
        wireCellData.ShapeType = (ShapeType)EditorGUILayout.EnumPopup("Shape Type", wireCellData.ShapeType);

        // State
        wireCellData.State = (WireCellState)EditorGUILayout.EnumPopup("State", wireCellData.State);

        // Is Clickable
        wireCellData.IsClickable = EditorGUILayout.Toggle("Is Clickable", wireCellData.IsClickable);

        // Output Angles array
        DrawOutputAngles(wireCellData);

        // Curve Intensity
        wireCellData.CurveIntensity = EditorGUILayout.FloatField("Curve Intensity", wireCellData.CurveIntensity);

        // Quiz Node ID
        wireCellData.QuizNodeId = EditorGUILayout.IntField("Quiz Node ID", wireCellData.QuizNodeId);

        // Apply to scene object button
        if (wireCellData.WireCell != null)
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply to Scene Object"))
            {
                ApplyWireCellDataToScene(wireCellData);
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawOutputAngles(WireCellData wireCellData)
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Output Angles");

        // Current array size
        int newSize = EditorGUILayout.IntField("Size", wireCellData.OutputAngles.Count);

        // Resize array if needed
        if (newSize != wireCellData.OutputAngles.Count)
        {
            while (wireCellData.OutputAngles.Count < newSize)
                wireCellData.OutputAngles.Add(0);
            while (wireCellData.OutputAngles.Count > newSize)
                wireCellData.OutputAngles.RemoveAt(wireCellData.OutputAngles.Count - 1);
        }

        // Array elements
        for (int i = 0; i < wireCellData.OutputAngles.Count; i++)
        {
            wireCellData.OutputAngles[i] = EditorGUILayout.IntField($"Angle {i}", wireCellData.OutputAngles[i]);
        }

        EditorGUILayout.EndVertical();
    }

    private void AddWireCellToLevel(LevelConfig level, WireCell wireCell)
    {
        WireCellData newCellData = new WireCellData();
        ApplyWireCellSceneToData(level, newCellData, wireCell);

        level.WireCells.Add(newCellData);
        Debug.Log($"Added WireCell {wireCell.name} to level {level.LevelId}");
    }

    private void ApplyWireCellSceneToData(LevelConfig level, WireCellData data, WireCell wireCell)
    {
        // Check if this WireCell is already in the list
        if (level.WireCells.Any(wcd => wcd.WireCell == wireCell))
        {
            Debug.LogWarning($"WireCell {wireCell.name} is already in the level configuration");
        }

        data.WireCell = wireCell;
        data.Position = wireCell.transform.position;
        data.ShapeType = wireCell.ShapeType; // Assuming you have getter methods
        data.State = wireCell.State;
        data.IsClickable = wireCell.IsClickable;
        data.OutputAngles = new List<int>(wireCell.OutputAngles);
        data.CurveIntensity = wireCell.Intensity;
        data.QuizNodeId = wireCell.QuizNodeId;
    }

    private void ApplyWireCellDataToScene(WireCellData wireCellData)
    {
        if (wireCellData.WireCell != null)
        {
            // Assuming you have a method in WireCell to apply all data at once
            // or individual setters for each property
            wireCellData.WireCell.Set(wireCellData);
            wireCellData.WireCell.OnValidate();
            EditorUtility.SetDirty(wireCellData.WireCell);
            Debug.Log($"Applied data to WireCell {wireCellData.WireCell.name}");
        }
        else
        {
            Debug.LogWarning("WireCell reference is null. This WireCell might have been deleted from the scene.");
        }
    }

    private void AddNewLevel()
    {
        LevelConfig newLevel = new LevelConfig
        {
            LevelId = levelsConfig.Levels.Count > 0 ? levelsConfig.Levels.Max(l => l.LevelId) + 1 : 1,
            DialogNodeId = 0,
            WireCells = new List<WireCellData>()
        };

        levelsConfig.Levels.Add(newLevel);
        selectedLevelIndex = levelsConfig.Levels.Count - 1;
        EditorUtility.SetDirty(levelsConfig);
    }

    private void RemoveLevel(int index)
    {
        if (index >= 0 && index < levelsConfig.Levels.Count)
        {
            levelsConfig.Levels.RemoveAt(index);
            selectedLevelIndex = Mathf.Clamp(selectedLevelIndex, 0, levelsConfig.Levels.Count - 1);
            EditorUtility.SetDirty(levelsConfig);
        }
    }
}
#endif
