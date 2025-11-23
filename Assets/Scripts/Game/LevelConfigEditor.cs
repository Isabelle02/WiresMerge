#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelsConfigWindow : EditorWindow
{
    private Transform _wiresParentTransform;
    private PoolConfig _poolConfig;
    private LevelsConfig _levelsConfig;
    private int _selectedLevelIndex = 0;
    private Vector2 _scrollPosition;
    private Vector2 _wireCellsScrollPosition;
    private bool _showWireCells = true;
    private bool[] _wireCellFoldouts = new bool[0];

    [MenuItem("Tools/Levels Config Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<LevelsConfigWindow>("Levels");
        window.minSize = new Vector2(400, 600);
    }

    private void OnEnable()
    {
        LoadLevelsConfig();
    }

    private void LoadLevelsConfig()
    {
        // Try to find a LevelsConfig asset in the project
        var guids = AssetDatabase.FindAssets("t:LevelsConfig");
        if (guids.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _levelsConfig = AssetDatabase.LoadAssetAtPath<LevelsConfig>(path);
        }

        // Try to find a PoolConfig asset in the project
        var poolGuids = AssetDatabase.FindAssets("t:PoolConfig");
        if (poolGuids.Length > 0)
        {
            var path = AssetDatabase.GUIDToAssetPath(poolGuids[0]);
            _poolConfig = AssetDatabase.LoadAssetAtPath<PoolConfig>(path);
        }
    }

    private void OnGUI()
    {
        DrawToolbar();
        if (_levelsConfig == null)
        {
            DrawNoConfigHelp();
            return;
        }

        EditorGUILayout.Space();
        using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition))
        {
            _scrollPosition = scrollView.scrollPosition;

            DrawLevelSelection();
            EditorGUILayout.Space();
            if (_levelsConfig.Levels.Count > 0 && _selectedLevelIndex < _levelsConfig.Levels.Count)
            {
                DrawSelectedLevel();
            }
            else
            {
                DrawNoLevelsHelp();
            }
        }

        // Apply changes if any
        if (GUI.changed && _levelsConfig != null)
        {
            EditorUtility.SetDirty(_levelsConfig);
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        // LevelsConfig object field
        EditorGUI.BeginChangeCheck();
        _levelsConfig = (LevelsConfig)EditorGUILayout.ObjectField(_levelsConfig, typeof(LevelsConfig), false);
        if (EditorGUI.EndChangeCheck() && _levelsConfig != null)
        {
            _selectedLevelIndex = 0;
            _wireCellFoldouts = new bool[0];
        }

        // PoolConfig object field
        EditorGUI.BeginChangeCheck();
        _poolConfig = (PoolConfig)EditorGUILayout.ObjectField(_poolConfig, typeof(PoolConfig), false);
        EditorGUI.EndChangeCheck();

        // WiresParent object field
        EditorGUI.BeginChangeCheck();
        _wiresParentTransform = (Transform)EditorGUILayout.ObjectField(_wiresParentTransform, typeof(Transform), true);
        EditorGUI.EndChangeCheck();

        GUILayout.FlexibleSpace();

        // Create new LevelsConfig button
        if (GUILayout.Button("Create New", EditorStyles.toolbarButton))
        {
            CreateNewLevelsConfig();
        }

        // Refresh button
        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
        {
            LoadLevelsConfig();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawNoConfigHelp()
    {
        EditorGUILayout.HelpBox("No LevelsConfig asset found or selected. Please assign a LevelsConfig asset or create a new one.", MessageType.Warning);

        EditorGUILayout.Space();
        if (GUILayout.Button("Create New LevelsConfig", GUILayout.Height(30)))
        {
            CreateNewLevelsConfig();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Find LevelsConfig and PoolConfig in Project", GUILayout.Height(30)))
        {
            LoadLevelsConfig();
        }
    }

    private void DrawNoLevelsHelp()
    {
        EditorGUILayout.HelpBox("No levels available. Add a new level or drag WireCell objects from scene.", MessageType.Info);
    }

    private void DrawLevelSelection()
    {
        EditorGUILayout.BeginHorizontal();

        // Level dropdown
        if (_levelsConfig.Levels.Count > 0)
        {
            var levelOptions = _levelsConfig.Levels.Select((level, index) => $"Level {level.LevelId} (Index: {index})").ToArray();
            _selectedLevelIndex = EditorGUILayout.Popup("Selected Level", _selectedLevelIndex, levelOptions);
        }
        else
        {
            EditorGUILayout.Popup("Selected Level", 0, new string[] { "No Levels" });
        }

        // Add level button
        if (GUILayout.Button("Add Level", GUILayout.Width(80)))
        {
            AddNewLevel();
        }

        // Remove level button
        if (_levelsConfig.Levels.Count > 0)
        {
            if (GUILayout.Button("Remove", GUILayout.Width(80)))
            {
                RemoveLevel(_selectedLevelIndex);
            }
        }

        EditorGUILayout.EndHorizontal();

        // Clear all button
        if (_levelsConfig.Levels.Count > 0)
        {
            if (GUILayout.Button("Clear All Levels", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Clear All Levels", "Are you sure you want to clear all levels?", "Yes", "No"))
                {
                    _levelsConfig.ClearData();
                    _selectedLevelIndex = 0;
                    _wireCellFoldouts = new bool[0];
                }
            }
        }
    }

    private void DrawSelectedLevel()
    {
        var selectedLevel = _levelsConfig.Levels[_selectedLevelIndex];

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
        var headerRect = EditorGUILayout.GetControlRect();

        // Custom foldout with count
        _showWireCells = EditorGUI.Foldout(headerRect, _showWireCells, $"Wire Cells ({level.WireCells.Count})", true);

        if (_showWireCells)
        {
            EditorGUILayout.Space();

            // Drag and drop area
            DrawDragDropArea(level);
            EditorGUILayout.Space();

            // Wire cells list
            using (var scrollView = new EditorGUILayout.ScrollViewScope(_wireCellsScrollPosition, GUILayout.MaxHeight(400)))
            {
                _wireCellsScrollPosition = scrollView.scrollPosition;

                // Ensure foldouts array matches wire cells count
                if (_wireCellFoldouts.Length != level.WireCells.Count)
                {
                    var newFoldouts = new bool[level.WireCells.Count];
                    for (var i = 0; i < Mathf.Min(_wireCellFoldouts.Length, level.WireCells.Count); i++)
                    {
                        newFoldouts[i] = _wireCellFoldouts[i];
                    }

                    _wireCellFoldouts = newFoldouts;
                }

                for (var i = 0; i < level.WireCells.Count; i++)
                {
                    DrawWireCellItem(level.WireCells[i], i, level);
                }
            }

            if (level.WireCells.Count > 0)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Draw All Wire Cells", GUILayout.Height(25)))
                {
                    foreach (var cell in level.WireCells)
                    {
                        var cellObj = Instantiate(_poolConfig.Get<WireCell>(), _wiresParentTransform);
                        cellObj.Set(cell);
                        cellObj.OnValidate();
                        cell.WireCell = cellObj;
                    }
                }

                EditorGUILayout.Space();
                if (GUILayout.Button("Clear All Wire Cells", GUILayout.Height(25)))
                {
                    if (EditorUtility.DisplayDialog("Clear Wire Cells", "Are you sure you want to clear all wire cells?", "Yes", "No"))
                    {
                        level.WireCells.Clear();
                        _wireCellFoldouts = new bool[0];
                    }
                }
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawDragDropArea(LevelConfig level)
    {
        var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag WireCell objects from scene here", EditorStyles.helpBox);

        var evt = Event.current;

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
                    foreach (var draggedObject in DragAndDrop.objectReferences)
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

                    // Resize foldouts array
                    _wireCellFoldouts = new bool[level.WireCells.Count];
                }
                break;
        }
    }

    private void DrawWireCellItem(WireCellData wireCellData, int index, LevelConfig level)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.BeginHorizontal();

        // Foldout for this wire cell
        _wireCellFoldouts[index] = EditorGUILayout.Foldout(_wireCellFoldouts[index], $"Wire Cell {index}", true);

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Remove", GUILayout.Width(60)))
        {
            level.WireCells.RemoveAt(index);
            var newFoldouts = new bool[level.WireCells.Count];
            for (var i = 0; i < level.WireCells.Count; i++)
            {
                var oldIndex = i < index ? i : i + 1;
                if (oldIndex < _wireCellFoldouts.Length)
                    newFoldouts[i] = _wireCellFoldouts[oldIndex];
            }

            _wireCellFoldouts = newFoldouts;
            return;
        }

        EditorGUILayout.EndHorizontal();
        if (_wireCellFoldouts[index])
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            var newWireCell = (WireCell)EditorGUILayout.ObjectField("Scene Object", wireCellData.WireCell, typeof(WireCell), true);
            if (EditorGUI.EndChangeCheck())
            {
                ApplyWireCellSceneToData(level, wireCellData, newWireCell);
                Debug.Log($"Updated WireCell {newWireCell.name} to level {level.LevelId}");
            }

            wireCellData.Position = EditorGUILayout.Vector3Field("Position", wireCellData.Position);
            wireCellData.ShapeType = (ShapeType)EditorGUILayout.EnumPopup("Shape Type", wireCellData.ShapeType);
            wireCellData.State = (WireCellState)EditorGUILayout.EnumPopup("State", wireCellData.State);
            wireCellData.IsClickable = EditorGUILayout.Toggle("Is Clickable", wireCellData.IsClickable);
            wireCellData.CurveIntensity = EditorGUILayout.FloatField("Curve Intensity", wireCellData.CurveIntensity);
            wireCellData.QuizNodeId = EditorGUILayout.IntField("Quiz Node ID", wireCellData.QuizNodeId);
            DrawOutputAngles(wireCellData);

            if (wireCellData.WireCell != null)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button("Apply to Scene Object"))
                {
                    ApplyWireCellDataToScene(wireCellData);
                }
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawOutputAngles(WireCellData wireCellData)
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("Output Angles", EditorStyles.boldLabel);
        var newSize = EditorGUILayout.IntField("Size", wireCellData.OutputAngles.Count);

        // Resize array if needed
        if (newSize != wireCellData.OutputAngles.Count)
        {
            while (wireCellData.OutputAngles.Count < newSize)
                wireCellData.OutputAngles.Add(0);
            while (wireCellData.OutputAngles.Count > newSize)
                wireCellData.OutputAngles.RemoveAt(wireCellData.OutputAngles.Count - 1);
        }

        // Array elements
        for (var i = 0; i < wireCellData.OutputAngles.Count; i++)
        {
            wireCellData.OutputAngles[i] = EditorGUILayout.IntField($"Angle {i}", wireCellData.OutputAngles[i]);
        }

        EditorGUILayout.EndVertical();
    }

    private void AddWireCellToLevel(LevelConfig level, WireCell wireCell)
    {
        var newCellData = new WireCellData();
        ApplyWireCellSceneToData(level, newCellData, wireCell);
        level.WireCells.Add(newCellData);

        // Resize foldouts array and set the new one to be expanded
        var newFoldouts = new bool[level.WireCells.Count];
        for (var i = 0; i < _wireCellFoldouts.Length; i++)
        {
            newFoldouts[i] = _wireCellFoldouts[i];
        }
        newFoldouts[newFoldouts.Length - 1] = true; // Expand the new one
        _wireCellFoldouts = newFoldouts;

        Debug.Log($"Added WireCell {wireCell.name} to level {level.LevelId}");
    }

    private void ApplyWireCellSceneToData(LevelConfig level, WireCellData data, WireCell wireCell)
    {
        // Check if this WireCell is already in the list
        if (level.WireCells.Any(w => w.WireCell == wireCell))
        {
            Debug.LogWarning($"WireCell {wireCell.name} is already in the level configuration");
        }

        data.WireCell = wireCell;
        data.Position = wireCell.transform.position;
        data.ShapeType = wireCell.ShapeType;
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
        var newLevel = new LevelConfig
        {
            LevelId = _levelsConfig.Levels.Count > 0 ? _levelsConfig.Levels.Max(l => l.LevelId) + 1 : 1,
            DialogNodeId = 0,
            WireCells = new List<WireCellData>()
        };

        _levelsConfig.Levels.Add(newLevel);
        _selectedLevelIndex = _levelsConfig.Levels.Count - 1;
        _wireCellFoldouts = new bool[0];
    }

    private void RemoveLevel(int index)
    {
        if (index >= 0 && index < _levelsConfig.Levels.Count)
        {
            _levelsConfig.Levels.RemoveAt(index);
            _selectedLevelIndex = Mathf.Clamp(_selectedLevelIndex, 0, _levelsConfig.Levels.Count - 1);
            _wireCellFoldouts = new bool[0];
        }
    }

    private void CreateNewLevelsConfig()
    {
        var path = EditorUtility.SaveFilePanelInProject(
            "Create New LevelsConfig",
            "LevelsConfig",
            "asset",
            "Please enter a file name to save the LevelsConfig to");

        if (!string.IsNullOrEmpty(path))
        {
            var newConfig = CreateInstance<LevelsConfig>();
            AssetDatabase.CreateAsset(newConfig, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _levelsConfig = newConfig;
            _selectedLevelIndex = 0;
            _wireCellFoldouts = new bool[0];
        }
    }
}
#endif
