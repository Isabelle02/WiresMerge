using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WireSystem
{
    private List<IWireCell> _wireCells = new List<IWireCell>();
    private List<IWireCell> _sourceCells = new List<IWireCell>();

    private int _bulbTurnedOnCount = 0;
    private int _bulbNeedToTurnOnCount = 0;

    public void AddWireCell(IWireCell wireCell)
    {
        if (wireCell.State == WireCellState.Bulb)
            _bulbNeedToTurnOnCount++;

        if (wireCell.State == WireCellState.Source)
            _sourceCells.Add(wireCell);

        wireCell.Rotated += OnRotated;
        wireCell.BulbTurnedOn += OnBulbTurnedOn;
        wireCell.BulbTurnedOff += OnBulbTurnedOff;
        _wireCells.Add(wireCell);
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

        foreach (var cell in _sourceCells)
        {
            CheckConnection(cell, _wireCells);
        }
    }

    private void CheckConnection(IWireCell wireCell, List<IWireCell> wireCells)
    {
        for (var i = 0; i < wireCell.OutputCount; i++)
        {
            var direction = GetDirection(wireCell.OutputAngles[i]);
            var hits = Physics2D.RaycastAll(wireCell.Position, direction, 1f);
            var cell = wireCells.FirstOrDefault(w => w.Position != wireCell.Position && hits.FirstOrDefault(hit => hit.transform.position == w.Position));

            if (cell != null && cell.OutputAngles.Any(angle => (-GetDirection(angle) == direction)))
            {
                Debug.Log("connected neighbor " + cell.Position);

                wireCell.OutputUsedCount++;
                cell.OutputUsedCount++;

                cell.Highlight();

                var remainedCells = new List<IWireCell>(wireCells);
                remainedCells.Remove(wireCell);
                remainedCells.Remove(cell);
                
                CheckConnection(cell, remainedCells);
            }
        }
    }

    private void OnBulbTurnedOn(IWireCell wireCell)
    {
        Debug.Log("OnBulbTurnedOn " + _bulbTurnedOnCount + " " + _bulbNeedToTurnOnCount);

        _bulbTurnedOnCount++;
        if (_bulbNeedToTurnOnCount == _bulbTurnedOnCount && _wireCells.Where(w => w.IsHighlighted).All(w => w.OutputUsedCount == w.OutputCount))
            Debug.Log("WIN");
    }

    private void OnBulbTurnedOff(IWireCell wireCell)
    {
        Debug.Log("OnBulbTurnedOff");
        _bulbTurnedOnCount--;
    }
}
