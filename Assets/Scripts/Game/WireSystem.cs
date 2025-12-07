using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WireSystem
{
    private List<IWireCell> _wireCells = new List<IWireCell>();

    private int _bulbTurnedOnCount = 0;
    private int _bulbNeedToTurnOnCount = 0;

    public static bool IsWin { get; private set; }

    public void AddWireCell(IWireCell wireCell)
    {
        if (_wireCells.Contains(wireCell))
            return;

        if (wireCell.State == WireCellState.Bulb)
            _bulbNeedToTurnOnCount++;

        wireCell.Rotated += OnRotated;
        wireCell.BulbTurnedOn += OnBulbTurnedOn;
        wireCell.BulbTurnedOff += OnBulbTurnedOff;
        _wireCells.Add(wireCell);
        OnRotated();
    }

    public void RemoveWireCell(IWireCell wireCell)
    {
        wireCell.Rotated -= OnRotated;
        wireCell.BulbTurnedOn -= OnBulbTurnedOn;
        wireCell.BulbTurnedOff -= OnBulbTurnedOff;
        _wireCells.Remove(wireCell);
    }

    public static Vector2 GetDirection(int angleZ, float width, float height)
    {
        var angleRadians = angleZ * Mathf.Deg2Rad;
        var vectorY = height * 0.5f * Mathf.Sin(angleRadians);
        var vectorX = vectorY != 0 ? vectorY / Mathf.Tan(angleRadians) : width / 2f;
        return new Vector2(vectorX, vectorY);
    }

    private void OnRotated()
    {
        Debug.Log("CheckConnections");
        foreach (var cell in _wireCells)
        {
            cell.OutputUsedCount = 0;
            cell.Unhighlight();
        }

        var usedSources = new List<IWireCell>();
        foreach (var cell in _wireCells)
        {
            if (cell.State == WireCellState.Source && !usedSources.Contains(cell))
            {
                var wireCells = new List<IWireCell>(_wireCells);
                usedSources.AddRange(CheckConnection(cell, wireCells));
            }
        }

        CheckWin();
    }

    private List<IWireCell> CheckConnection(IWireCell wireCell, List<IWireCell> wireCells)
    {
        var usedSources = new List<IWireCell>();
        wireCells.Remove(wireCell);
        var remained = new List<IWireCell>(wireCells);

        for (var i = 0; i < wireCell.OutputCount; i++)
        {
            var direction = GetDirection(wireCell.OutputAngles[i], wireCell.Width, wireCell.Height);
            var hit = Physics2D.Raycast((Vector2)wireCell.Position + direction * 1.1f, Vector3.forward);
            var cell = remained.FirstOrDefault(w => hit.transform && hit.transform.position == w.Position);
            if (cell != null && cell.OutputAngles.Any(angle => (-GetDirection(angle, cell.Width, cell.Height) == direction)))
            {
                wireCell.OutputUsedCount++;
                cell.OutputUsedCount++;
                cell.Highlight();
                if (wireCell.State == WireCellState.Source)
                    usedSources.Add(wireCell);

                usedSources.AddRange(CheckConnection(cell, wireCells));
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
