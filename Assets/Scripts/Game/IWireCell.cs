using System;
using System.Collections.Generic;
using UnityEngine;

public interface IWireCell
{
    public Vector3 Position { get; }
    public ShapeType ShapeType { get; }
    public WireCellState State { get; }
    public int OutputCount { get; }
    public int OutputUsedCount { get; set; }
    public bool IsHighlighted { get; }
    public List<int> OutputAngles { get; }
    public Action Rotated { get; set; }
    public Action<IWireCell> BulbTurnedOn { get; set; }
    public Action<IWireCell> BulbTurnedOff { get; set; }

    public int SideCount => ShapeType == ShapeType.Rect ? 4 : 6;
    public float Width => 1f;
    public float Height => ShapeType == ShapeType.Rect ? 1f : 0.8659766f;

    public void Highlight();
    public void Unhighlight();
}
