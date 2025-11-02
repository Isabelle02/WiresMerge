using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BaseInputField : MonoBehaviour, IClickable, IDisposable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private Text _initialText;
    [SerializeField] private Text _displayText;
    [SerializeField] private int _characterLimit = 20;
    [SerializeField] private Image _background;
    [SerializeField] private Color _selectedColor = Color.white;

    private string _text;
    private bool _isSelected = false;

    public Collider2D Collider => _collider;
    public Action<string> OnValueChanged { get; set; }
    //public Action<string> EndEdit { get; set; }

    public virtual void OnClick()
    {
        _isSelected = true;
        UpdateVisuals();
    }

    private void Update()
    {
        if (_isSelected && Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (_text.Length > 0)
                        _text = _text.Substring(0, _text.Length - 1);
                }
                else if (c == '\n' || c == '\r') // Enter
                {
                    _isSelected = false;
                    OnValueChanged?.Invoke(_text);
                }
                else
                {
                    _text += c;
                }
            }
            UpdateVisuals();
        }
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (_isSelected &&  hit.collider != _collider)
            {
                _isSelected = false;
                UpdateVisuals();
                OnValueChanged?.Invoke(_text);
            }
        }
        
    }

    private void UpdateVisuals()
    {
        _displayText.text = _text;
        _background.color = _isSelected ? _selectedColor : Color.white;
        _initialText.gameObject.SetActive(!_isSelected && string.IsNullOrEmpty(_text));
    }

    public virtual void Dispose()
    {
        OnValueChanged = null;
        //EndEdit = null;
    }
}
