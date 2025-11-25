using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public enum ShapeType
{
    Rect,
    Hex
}

[Serializable]
public enum WireCellState
{
    Source,
    Wire,
    Bulb
}

public class WireCell : MonoBehaviour, IClickable, IWireCell
{
    [SerializeField] private ShapeType _shapeType;
    [SerializeField] private Collider2D _rectCollider;
    [SerializeField] private Collider2D _hexCollider;
    [SerializeField] private GameObject _rectLight;
    [SerializeField] private GameObject _hexLight;

    [SerializeField] private bool _isClickable;
    [SerializeField] private WireCellState _state;
    [SerializeField] private List<int> _outputAngles; // relative to z    right - 0    up - 90    left - 180    down - 270
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private float _intensity = 0.2f;
    [SerializeField] private int _quizNodeId = -1;

    private float _baseAngle = 0f;
    private Tween _rotateTween;
    private int _rotateCount = 0;

    private int SideCountInternal => (this as IWireCell).SideCount;

    public Vector3 Position => transform.position;
    public ShapeType ShapeType => _shapeType;
    public WireCellState State => _state;
    public bool IsClickable => _isClickable;
    public float Intensity => _intensity;
    public int QuizNodeId => _quizNodeId;
    public int OutputCount => _outputAngles.Count;
    public int OutputUsedCount { get; set; } = 0;
    public bool IsHighlighted { get; private set; } = false;
    public List<int> OutputAngles => _outputAngles;
    public Action Rotated { get; set; }
    public Action<IWireCell> BulbTurnedOn { get; set; }
    public Action<IWireCell> BulbTurnedOff { get; set; }

    public Collider2D Collider => _shapeType == ShapeType.Rect ? _rectCollider : _hexCollider;
    public GameObject Light => _shapeType == ShapeType.Rect ? _rectLight : _hexLight;

    public void OnValidate()
    {
        if (_lineRenderer)
            DrawLines();
    }

    public void Set(WireCellData data)
    {
        if (data == null)
            return;

        transform.position = data.Position;
        _shapeType = data.ShapeType;
        _state = data.State;
        _isClickable = data.IsClickable;
        _outputAngles = new List<int>(data.OutputAngles);
        _intensity = data.CurveIntensity;
        _quizNodeId = data.QuizNodeId;
    }

    public void Init()
    {
        _rectCollider.gameObject.SetActive(_shapeType == ShapeType.Rect);
        _hexCollider.gameObject.SetActive(_shapeType == ShapeType.Hex);
        IsHighlighted = _state == WireCellState.Source;
        Light.SetActive(IsHighlighted);
        DrawLines();

        if (_isClickable)
            MouseManager.AddClickable(this);

        Gameplay.WireSystem.AddWireCell(this);
    }

    private void DrawLines()
    {
        if (_outputAngles == null || _outputAngles.Count == 0) 
            return;

        var allPoints = new List<Vector3>();
        var segmentsPerSegment = 15;

        if (_outputAngles.Count == 1)
        {
            var direction = WireSystem.GetDirection(_outputAngles[0], (this as IWireCell).Width, (this as IWireCell).Height);
            allPoints.Add(Vector2.zero);
            allPoints.Add(direction);
        }
        else
        {
            DrawMultiAngleCurve(allPoints, segmentsPerSegment);
        }

        _lineRenderer.positionCount = allPoints.Count;
        _lineRenderer.SetPositions(allPoints.ToArray());
    }

    private void DrawSingleCurve(List<Vector3> points, Vector3 start, Vector3 end, int segments)
    {
        var control = Vector3.Lerp(start, end, 0.5f) + GetPerpendicularOffset(start, end, _intensity);

        for (var i = 0; i <= segments; i++)
        {
            points.Add(CalculateQuadraticBezierPoint((float)i / segments, start, control, end));
        }
    }

    private void DrawMultiAngleCurve(List<Vector3> points, int segments)
    {
        // Points for all directions
        var directionPoints = new List<Vector3>();
        foreach (var angle in _outputAngles)
        {
            directionPoints.Add(WireSystem.GetDirection(angle, (this as IWireCell).Width, (this as IWireCell).Height));
        }

        // Connecting points by curve
        for (var i = 0; i < directionPoints.Count - 1; i++)
        {
            var currentDir = directionPoints[i];
            var nextDir = directionPoints[i + 1];

            DrawSingleCurve(points, currentDir, nextDir, segments);
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        var u = 1 - t;
        var uu = u * u;
        var tt = t * t;

        var p = uu * p0;         // (1-t)^2 * p0
        p += 2 * u * t * p1;         // 2(1-t)t * p1
        p += tt * p2;               // t^2 * p2

        return p;
    }

    private Vector3 GetPerpendicularOffset(Vector3 start, Vector3 end, float intensity)
    {
        var direction = (end - start).normalized;
        var perpendicular = new Vector3(-direction.y, direction.x, 0);
        return perpendicular * intensity;
    }

    public void Highlight()
    {
        if (_state == WireCellState.Source || IsHighlighted)
            return;

        IsHighlighted = true;
        Light.SetActive(true);
        if (_state == WireCellState.Bulb)
            BulbTurnedOn?.Invoke(this);
    }

    public void Unhighlight()
    {
        if (_state == WireCellState.Source || !IsHighlighted)
            return;

        IsHighlighted = false;
        Light.SetActive(false);
        if (_state == WireCellState.Bulb)
            BulbTurnedOff?.Invoke(this);
    }

    public void OnClick()
    {
        RotateToLeft();
    }

    public void RotateToLeft()
    {
        _rotateCount++;
        _rotateTween?.Kill();
        _rotateTween = RotateAnimation();
        _rotateTween.OnComplete(() =>
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * (transform.rotation.eulerAngles.z % 360));
            var angle = _rotateCount % SideCountInternal * 360 / SideCountInternal;
            for (var i = 0; i < _outputAngles.Count; i++)
                _outputAngles[i] = (_outputAngles[i] + angle) % 360;

            _baseAngle = transform.rotation.eulerAngles.z;
            _rotateCount = 0;
            Rotated?.Invoke();
        });
    }

    private Tween RotateAnimation()
    {
        int maxExtraRotations = 2;
        int effectiveRotateCount = Mathf.Min(_rotateCount, maxExtraRotations * SideCountInternal + _rotateCount % SideCountInternal);
        var angle = _baseAngle + 360 / SideCountInternal * effectiveRotateCount;
        var duration = 0.1f + effectiveRotateCount / 100f;
        return transform.DORotate(new Vector3(0f, 0f, angle) - transform.rotation.eulerAngles, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }
}
