using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;


public class WireCell : MonoBehaviour, IClickable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private bool _isClickable;

    // fill the list counterclockwise
    [SerializeField] private List<GameObject> _wires = new List<GameObject>();

    private Tween _rotateTween;
    private int _rotateCount = 0;

    private int OutputCount => _wires.Count;

    public Collider2D Collider { get => _collider; }

    public void Start()
    {
        if (_isClickable)
            MouseManager.AddClickable(this);
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
            Debug.Log("complete " + _rotateCount);
            transform.rotation = Quaternion.identity;
            var tempWires = new List<bool>(_wires.Select(w => w.activeSelf));
            for (var i = 0; i < OutputCount; i++)
                _wires[i].SetActive(tempWires[(i + OutputCount - _rotateCount % OutputCount) % OutputCount]);

            _rotateCount = 0;
        });
    }

    private Tween RotateAnimation()
    {
        int maxExtraRotations = 2;
        int effectiveRotateCount = Mathf.Min(_rotateCount, maxExtraRotations * OutputCount + _rotateCount % OutputCount);
        var angle = 360 / OutputCount * effectiveRotateCount;
        var duration = 0.7f + effectiveRotateCount / 10f;
        return transform.DORotate(new Vector3(0f, 0f, angle) - transform.rotation.eulerAngles, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);
    }
}
