using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WireCellData
{
    [NonSerialized] public WireCell WireCell;
    public Vector3 Position;
    public ShapeType ShapeType;
    public WireCellState State;
    public bool IsClickable;
    public List<int> OutputAngles;
    public float CurveIntensity;
    public int QuizNodeId;
}

[Serializable]
public class LevelConfig
{
    public int LevelId;
    public int DialogNodeId;

    public List<WireCellData> WireCells = new List<WireCellData>();
}

[Serializable]
[CreateAssetMenu(fileName = "Levels", menuName = "Level System/Levels")]
public class LevelsConfig : ScriptableObject
{
    public List<LevelConfig> Levels = new List<LevelConfig>();

    public void ClearData()
    {
        Levels.Clear();
    }
}
