using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;


public class WireCell : MonoBehaviour
{
    // fill the list counterclockwise
    [SerializeField] private List<GameObject> _wires = new List<GameObject>();

    private Tween _rotateTween;
    private int _rotateCount = 0;

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
            for (var i = 0; i < tempWires.Count; i++)
                _wires[i].SetActive(tempWires[(i + 4 - _rotateCount % 4) % 4]);

            _rotateCount = 0;
        });
    }

    private Tween RotateAnimation()
    {
        var angle = 360 / _wires.Count * _rotateCount;
        var duration = 1f + _rotateCount / 8;
        return transform.DORotate(new Vector3(0f, 0f, angle) - transform.rotation.eulerAngles, duration, RotateMode.FastBeyond360).SetRelative();
    }
}
