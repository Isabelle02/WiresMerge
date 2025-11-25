using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WireSystem
{
    private List<IWireCell> _wireCells = new List<IWireCell>();
    private List<IWireCell> _sourceCells = new List<IWireCell>();

    private int _bulbTurnedOnCount = 0;
    private int _bulbNeedToTurnOnCount = 0;

    public bool IsWin { get; private set; }

    public void AddWireCell(IWireCell wireCell)
    {
        if (_wireCells.Contains(wireCell))
            return;

        if (wireCell.State == WireCellState.Bulb)
            _bulbNeedToTurnOnCount++;

        if (wireCell.State == WireCellState.Source)
            _sourceCells.Add(wireCell);

        wireCell.Rotated += OnRotated;
        wireCell.BulbTurnedOn += OnBulbTurnedOn;
        wireCell.BulbTurnedOff += OnBulbTurnedOff;
        _wireCells.Add(wireCell);
        OnRotated();
    }

    public void RemoveWireCell(IWireCell wireCell)
    {
        if (wireCell.State == WireCellState.Source)
            _sourceCells.Remove(wireCell);

        wireCell.Rotated -= OnRotated;
        wireCell.BulbTurnedOn -= OnBulbTurnedOn;
        wireCell.BulbTurnedOff -= OnBulbTurnedOff;
        _wireCells.Remove(wireCell);
    }

    private Vector2 GetDirection(float angleZ)
    {
        var angleRadians = angleZ * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
    }

    private void OnRotated()
    {
        Debug.Log("CheckConnections");
        foreach (var cell in _wireCells)
        {
            cell.OutputUsedCount = 0;
            cell.Unhighlight();
        }

        var wireCells = new List<IWireCell>(_wireCells);
        var usedSources = new List<IWireCell>();
        foreach (var cell in _sourceCells)
        {
            if (!usedSources.Contains(cell))
                usedSources.AddRange(CheckConnection(cell, wireCells));
        }

        CheckWin();
    }

    private List<IWireCell> CheckConnection(IWireCell wireCell, List<IWireCell> wireCells)
    {
        var usedSources = new List<IWireCell>();
        var remainedCells = new List<IWireCell>(wireCells);
        remainedCells.Remove(wireCell);
        for (var i = 0; i < wireCell.OutputCount; i++)
        {
            var direction = GetDirection(wireCell.OutputAngles[i]);
            var hits = Physics2D.RaycastAll(wireCell.Position, direction, 1f);
            var cell = remainedCells.FirstOrDefault(w => hits.FirstOrDefault(hit => hit.transform.position == w.Position));
            if (cell != null && cell.OutputAngles.Any(angle => (-GetDirection(angle) == direction)))
            {
                Debug.Log(wireCell.Position + " connected neighbor " + cell.Position);
                wireCell.OutputUsedCount++;
                cell.OutputUsedCount++;
                cell.Highlight();
                remainedCells.Remove(cell);
                if (_sourceCells.Contains(cell))
                    usedSources.Add(cell);

                usedSources.AddRange(CheckConnection(cell, remainedCells));
            }
        }

        return usedSources;
    }

    private void OnBulbTurnedOn(IWireCell wireCell)
    {
        Debug.Log("OnBulbTurnedOn " + _bulbTurnedOnCount + " " + _bulbNeedToTurnOnCount);
        _bulbTurnedOnCount++;
    }

    private void OnBulbTurnedOff(IWireCell wireCell)
    {
        Debug.Log("OnBulbTurnedOff");
        _bulbTurnedOnCount--;
    }

    private void CheckWin()
    {
        if (_bulbNeedToTurnOnCount == _bulbTurnedOnCount && _wireCells.Where(w => w.IsHighlighted).All(w => w.OutputUsedCount == w.OutputCount))
        {
            Debug.Log("WIN");
            IsWin = true;
        }
    }
}
