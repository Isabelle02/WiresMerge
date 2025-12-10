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

public class WireCell : MonoBehaviour, IClickable, IWireCell, IDisposable
{
    [SerializeField] private ShapeType _shapeType;
    [SerializeField] private Collider2D _rectCollider;
    [SerializeField] private Collider2D _hexCollider;
    [SerializeField] private GameObject _sourceObj;
    [SerializeField] private GameObject _bulbObj;
    [SerializeField] private LineRenderer _lineLight;

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
    public Dictionary<int, bool> OutputUsedAngles { get; set; } = new Dictionary<int, bool>();
    public bool IsHighlighted { get; private set; } = false;
    public List<int> OutputAngles => _outputAngles;
    public Action Rotated { get; set; }
    public Action<IWireCell> BulbTurnedOn { get; set; }
    public Action<IWireCell> BulbTurnedOff { get; set; }
    public Collider2D Collider => _shapeType == ShapeType.Rect ? _rectCollider : _hexCollider;

    public void OnValidate()
    {
        if (_lineRenderer)
            DrawLines(_lineRenderer);

        if (_lineLight)
            DrawLines(_lineLight);
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

        OutputUsedAngles = new Dictionary<int, bool>();
        foreach (var angle in _outputAngles)
        {
            OutputUsedAngles.Add(angle, false);
        }
    }

    public void Init()
    {
        IsHighlighted = _state == WireCellState.Source;
        _rectCollider.gameObject.SetActive(_shapeType == ShapeType.Rect);
        _hexCollider.gameObject.SetActive(_shapeType == ShapeType.Hex);
        _lineRenderer.gameObject.SetActive(true);
        _lineLight.gameObject.SetActive(IsHighlighted);
        _sourceObj.SetActive(_state == WireCellState.Source);
        _bulbObj.SetActive(_state == WireCellState.Bulb);
        DrawLines(_lineRenderer);
        DrawLines(_lineLight);

        if (_isClickable)
            MouseManager.AddClickable(this);

        Gameplay.WireSystem.AddWireCell(this);

        Gameplay.Finished += OnFinished;
    }


    private void DrawLines(LineRenderer line)
    {
        if (_outputAngles == null || _outputAngles.Count == 0)
            return;

        var allPoints = new List<Vector3>();
        var segmentsPerSegment = 15;

        var directionPoints = new List<Vector3>();
        foreach (var angle in _outputAngles)
        {
            directionPoints.Add(WireSystem.GetDirection(angle, (this as IWireCell).Width, (this as IWireCell).Height));
        }

        if (directionPoints.Count == 1)
        {
            allPoints.Add(Vector2.zero);
            allPoints.Add(directionPoints[0]);
        }
        else if (directionPoints.Count == 2 && -directionPoints[0] == directionPoints[1])
        {
            allPoints.Add(directionPoints[0]);
            allPoints.Add(directionPoints[1]);
        }
        else
        {
            DrawMultiAngleCurve(allPoints, segmentsPerSegment, directionPoints);
        }

        line.positionCount = allPoints.Count;
        line.SetPositions(allPoints.ToArray());
    }

    private void DrawSingleCurve(List<Vector3> points, Vector3 start, Vector3 end, int segments)
    {
        var control = Vector3.Lerp(start, end, 0.5f) + GetPerpendicularOffset(start, end, _intensity);

        for (var i = 0; i <= segments; i++)
        {
            points.Add(CalculateQuadraticBezierPoint((float)i / segments, start, control, end));
        }
    }

    private void DrawMultiAngleCurve(List<Vector3> points, int segments, List<Vector3> directionPoints)
    {
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
        _lineLight.gameObject.SetActive(true);
        if (_state == WireCellState.Bulb)
            BulbTurnedOn?.Invoke(this);
    }

    public void Unhighlight()
    {
        if (_state == WireCellState.Source || !IsHighlighted)
            return;

        IsHighlighted = false;
        _lineLight.gameObject.SetActive(false);
        if (_state == WireCellState.Bulb)
            BulbTurnedOff?.Invoke(this);
    }

    public void OnClick()
    {
        RotateToLeft();
        if (_quizNodeId > -1)
        {
            Gameplay.QuizSystem.Start(QuizNodeId);
            WindowManager.Open<QuizPopup>();
            _quizNodeId = -1;
        }
    }

    public void RotateToLeft()
    {
        AudioManager.PlayOneShot(Sound.WireClick);

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

    private void OnFinished()
    {
        Pool.Release(this);
    }

    public void Dispose()
    {
        _baseAngle = 0f;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        Gameplay.WireSystem.RemoveWireCell(this);
        MouseManager.RemoveClickable(this);
        Gameplay.Finished -= OnFinished;
    }
}
