using System;
using System.Collections.Generic;
using UnityEngine;

public interface IWireCell
{
    public Vector3 Position { get; }
    public WireCellState State { get; }
    public int WireCount { get; }
    public int OutputCount { get; }
    public List<int> OutputAngles { get; }
    public Action Rotated { get; set; }
    public Action<IWireCell> BulbTurnedOn { get; set; }
    public Action<IWireCell> BulbTurnedOff { get; set; }

    public bool IsHighlighted { get; }
    public int OutputUsedCount { get; set; }

    public void Highlight();
    public void Unhighlight();
}
