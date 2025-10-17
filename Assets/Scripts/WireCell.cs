using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

public class WireCell : MonoBehaviour, IClickable, IWireCell
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private bool _isClickable;
    [SerializeField] private bool _isSource;

    // fill the list counterclockwise
    [SerializeField] private List<GameObject> _wires = new List<GameObject>();

    private Tween _rotateTween;
    private int _rotateCount = 0;

    private Action<IWireCell> _changedRotation;

    public int WireCount => _wires.Count;
    public int OutputCount { get; private set; }
    public List<bool> ActiveStates => new List<bool>(_wires.Select(w => w.activeSelf));
    public Action<IWireCell> ChangedRotation { get => _changedRotation; set => _changedRotation = value; }

    public Collider2D Collider { get => _collider; }

    public void Awake()
    {
        OutputCount = _wires.Count(w => w.activeSelf);
    }

    public void Start()
    {
        if (_isClickable)
            MouseManager.AddClickable(this);

        Gameplay.WireSystem.AddWireCell(this);
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
            transform.rotation = Quaternion.identity;
            var tempWires = ActiveStates;
            for (var i = 0; i < WireCount; i++)
                _wires[i].SetActive(tempWires[(i + WireCount - _rotateCount % WireCount) % WireCount]);

            _rotateCount = 0;

            _changedRotation?.Invoke(this);
        });
    }

    private Tween RotateAnimation()
    {
        int maxExtraRotations = 2;
        int effectiveRotateCount = Mathf.Min(_rotateCount, maxExtraRotations * WireCount + _rotateCount % WireCount);
        var angle = 360 / WireCount * effectiveRotateCount;
        var duration = 0.7f + effectiveRotateCount / 10f;
        return transform.DORotate(new Vector3(0f, 0f, angle) - transform.rotation.eulerAngles, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }
}
