using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BaseSlider : MonoBehaviour, IClickable, IDisposable
{
    [SerializeField] private Collider2D _sliderCollider;
    [SerializeField] private Collider2D _handleCollider;
    [SerializeField] private float _minValue = 0f;
    [SerializeField] private float _maxValue = 100f;
    [SerializeField] private float _value = 100f;
    
    private bool _isDragging = false;

    public Collider2D Collider => _handleCollider;
    public Transform Handle => _handleCollider.transform;
    public float Value => _value;

    public Action<float> OnValueChanged { get; set; }

    void OnValidate()
    {
        _value = Mathf.Clamp(_value, _minValue, _maxValue);

        if (!Application.isPlaying)
        {
            UpdateHandlePosition();
        }
    }

    void Start()
    {
        UpdateHandlePosition();
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
        _value = Mathf.Clamp(newValue, _minValue, _maxValue);
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

    private void UpdateHandlePosition()
    {
        if (Handle != null && _sliderCollider != null)
        {
            Bounds sliderBounds = _sliderCollider.bounds;
            float normalizedValue = Mathf.InverseLerp(_minValue, _maxValue, _value);
            Vector3 targetPosition = Handle.position;
            targetPosition.x = Mathf.Lerp(sliderBounds.min.x, sliderBounds.max.x, normalizedValue);
            Handle.position = targetPosition;
        }
    }

    public void Dispose()
    {
        OnValueChanged = null;
    }
}
