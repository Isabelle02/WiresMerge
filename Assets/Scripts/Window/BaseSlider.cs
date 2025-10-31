using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BaseSlider : MonoBehaviour, IClickable, IDisposable
{
    [SerializeField] private Collider2D _sliderCollider;
    [SerializeField] private Collider2D _handleCollider;
    [SerializeField] private Image _handlerImage;
    [SerializeField] private Transform _handle;
    [SerializeField] private float _minValue = 0f;
    [SerializeField] private float _maxValue = 100f;
    [SerializeField] private float _value = 100f;

    [Header("Animation Settings")]
    [SerializeField] private float _moveDuration = 0.2f;
    
    private bool _isDragging = false;
    private Ease _moveEase = Ease.Linear;
    private Tweener _currentTween;

    public Collider2D Collider => _handleCollider;
    public float Value => _value;

    public Action<float> OnValueChanged { get; set; }

    void OnValidate()
    {
        _value = Mathf.Clamp(_value, _minValue, _maxValue);

        if (!Application.isPlaying)
        {
            UpdateHandlePosition(false);
        }
    }

    void Start()
    {
        UpdateHandlePosition(false);
    }

    public void OnClick()
    {
        _isDragging = true;
    }

    void Update()
    {
        if (_isDragging && Input.GetMouseButton(0))
        {
            UpdateSliderValue();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
    }

    public void SetValue(float newValue)
    {
        //_value = Mathf.Clamp(newValue, _minValue, _maxValue);
        UpdateHandlePosition();
        OnValueChanged?.Invoke(_value);
    }

    private void UpdateSliderValue()
    {
        Vector2 mousePosition = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
        Bounds sliderBounds = _sliderCollider.bounds;

        float relativePosition = Mathf.InverseLerp(
            sliderBounds.min.x,
            sliderBounds.max.x,
            mousePosition.x
        );

        float newValue = Mathf.Lerp(_minValue, _maxValue, relativePosition);

        SetValue(newValue);
    }

    private void UpdateHandlePosition(bool animate = true)
    {
        if (_handle != null && _sliderCollider != null)
        {
            Bounds sliderBounds = _sliderCollider.bounds;
            float normalizedValue = Mathf.InverseLerp(_minValue, _maxValue, _value);

            Vector3 targetPosition = _handle.position;
            targetPosition.x = Mathf.Lerp(sliderBounds.min.x, sliderBounds.max.x, normalizedValue);

            _currentTween?.Kill();
            if (animate)
            {
                _currentTween = _handle.DOMoveX(targetPosition.x, _moveDuration)
                    .SetEase(_moveEase)
                    .OnUpdate(UpdateColliderPosition);
            }
            else
            {
                _handle.position = targetPosition;
                UpdateColliderPosition();
            }
        }
    }

    private void UpdateColliderPosition()
    {
        if (_handleCollider != null && _handle != null)
        {
            _handleCollider.transform.position = _handle.position;
        }
    }

    public virtual void Dispose()
    {
        _currentTween?.Kill();
        OnValueChanged = null;
    }

    void OnDestroy()
    {
        Dispose();
    }

}
