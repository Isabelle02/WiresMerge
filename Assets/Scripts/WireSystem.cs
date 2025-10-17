using System.Collections.Generic;
using UnityEngine;

public class WireSystem
{
    private List<IWireCell> _wireCells = new List<IWireCell>();

    public void AddWireCell(IWireCell wireCell)
    {
        wireCell.ChangedRotation += CheckConnections;
        _wireCells.Add(wireCell);
    }

    public void RemoveWireCell(IWireCell wireCell)
    {
        wireCell.ChangedRotation -= CheckConnections;
        _wireCells.Remove(wireCell);
    }

    public void CheckConnections(IWireCell wireCell)
    {
        Debug.Log("CheckConnections");

        //var neighborCells = _wireCells;

        //var mainWireStates = wireCell.ActiveStates;

        //var outputConnectedCount = 0;

        //for (var i = 0; i < wireCell.WireCount; i++) 
        //{
        //    foreach (var neighborCell in neighborCells)
        //    {
        //        var neighborWireStates = neighborCell.ActiveStates;
        //        if (mainWireStates[i] == neighborWireStates[i + wireCell.WireCount / 2] == true)
        //        {
        //            outputConnectedCount++;
        //            break;
        //        }

        //    }
        //}
    }
}
