using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public enum WireCellState
{
    Source,
    Wire,
    Bulb
}

public class WireCell : MonoBehaviour, IClickable, IWireCell
{
    [SerializeField] private GameObject _light;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private bool _isClickable;
    [SerializeField] private WireCellState _state;
    [SerializeField] private int _wireCount;
    [SerializeField] private List<int> _outputAngles; // relative to z    right - 0    up - 90    left - 180    down - 270

    private float _baseAngle = 0f;
    private Tween _rotateTween;
    private int _rotateCount = 0;

    public Vector3 Position => transform.position;
    public WireCellState State => _state;
    public int WireCount => _wireCount;
    public int OutputCount { get; private set; }
    public List<int> OutputAngles => _outputAngles;
    public Action Rotated { get; set; }
    public Action<IWireCell> BulbTurnedOn { get; set; }
    public Action<IWireCell> BulbTurnedOff { get; set; }

    public int OutputUsedCount { get; set; } = 0;
    public bool IsHighlighted { get; private set; } = false;

    public Collider2D Collider { get => _collider; }

    public void Awake()
    {
        OutputCount = _outputAngles.Count;
        IsHighlighted = _state == WireCellState.Source;
        _light.SetActive(IsHighlighted);
    }

    public void Start()
    {
        if (_isClickable)
            MouseManager.AddClickable(this);

        Gameplay.WireSystem.AddWireCell(this);
    }

    public void Highlight()
    {
        if (_state == WireCellState.Source || IsHighlighted)
            return;

        IsHighlighted = true;
        _light.SetActive(true);
        if (_state == WireCellState.Bulb)
            BulbTurnedOn?.Invoke(this);
    }

    public void Unhighlight()
    {
        if (_state == WireCellState.Source || !IsHighlighted)
            return;

        IsHighlighted = false;
        _light.SetActive(false);
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
            var angle = _rotateCount % WireCount * 360 / WireCount;
            for (var i = 0; i < _outputAngles.Count; i++)
            {
                _outputAngles[i] = (_outputAngles[i] + angle) % 360;
            }

            _baseAngle = transform.rotation.eulerAngles.z;
            _rotateCount = 0;

            Rotated?.Invoke();
        });
    }

    private Tween RotateAnimation()
    {
        int maxExtraRotations = 2;
        int effectiveRotateCount = Mathf.Min(_rotateCount, maxExtraRotations * WireCount + _rotateCount % WireCount);
        var angle = _baseAngle + 360 / WireCount * effectiveRotateCount;
        var duration = 0.1f + effectiveRotateCount / 100f;
        return transform.DORotate(new Vector3(0f, 0f, angle) - transform.rotation.eulerAngles, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }
}
