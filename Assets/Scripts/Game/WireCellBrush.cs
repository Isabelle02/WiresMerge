#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;

[CreateAssetMenu(fileName = "WireCellBrush", menuName = "Brushes/WireCell Brush")]
[CustomGridBrush(false, true, false, "WireCell Brush")]
public class WireCellBrush : GridBrush
{
    [Header("Wire Cell Brush Settings")]
    public GameObject wireCellPrefab;
    public ShapeType brushShape = ShapeType.Rect;

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget == null || wireCellPrefab == null)
            return;

        Erase(grid, brushTarget, position);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(wireCellPrefab);
        if (instance != null)
        {
            instance.transform.SetParent(brushTarget.transform);
            instance.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f, 0f)));
            if (brushShape == ShapeType.Hex)
                instance.transform.position = grid.CellToWorld(position) + grid.cellSize * 0.5f - new Vector3(0.435f, 0.5f, 0f);

            Undo.RegisterCreatedObjectUndo(instance, "Paint WireCell");
        }
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget == null)
            return;

        var erased = false;
        foreach (Transform child in brushTarget.transform)
        {
            var cellPos = grid.WorldToCell(child.position);
            if (cellPos == position)
            {
                Undo.DestroyObjectImmediate(child.gameObject);
                erased = true;
            }
        }

        if (!erased)
        {
            var worldPos = grid.LocalToWorld(grid.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f, 0f)));
            if (brushShape == ShapeType.Hex)
                worldPos = grid.CellToWorld(position) + grid.cellSize * 0.5f - new Vector3(0.435f, 0.5f, 0f);

            foreach (Transform child in brushTarget.transform)
            {
                if (Vector3.Distance(child.position, worldPos) < 0.1f)
                {
                    Undo.DestroyObjectImmediate(child.gameObject);
                    break;
                }
            }
        }
    }
}
#endif