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

    public string text;
    private bool _isSelected = false;

    public Collider2D Collider => _collider;
    public Action<string> OnValueChanged { get; set; }
    //public Action<string> EndEdit { get; set; }

    public virtual void OnClick()
    {
        _isSelected = true;
        VoidTextToInitial();
        UpdateVisuals();
    }

    public void VoidTextToInitial()
    {
        if (text == "")
            text = _initialText.text;
    }

    private void Update()
    {
        if (_isSelected && Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (text.Length > 0)
                        text = text.Substring(0, text.Length - 1);
                }
                else if (c == '\n' || c == '\r') // Enter
                {
                    _isSelected = false;
                    VoidTextToInitial();
                    OnValueChanged?.Invoke(text);
                }
                else
                {
                    text += c;
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
                VoidTextToInitial();
                UpdateVisuals();
                OnValueChanged?.Invoke(text);
            }
            //if (hit.collider != _collider)
            //{
            //    VoidTextToInitial();
            //}
        }
        
    }

    private void UpdateVisuals()
    {
        _displayText.text = text;
        _background.color = _isSelected ? _selectedColor : Color.white;
        _initialText.gameObject.SetActive(!_isSelected && string.IsNullOrEmpty(text));
    }

    public virtual void Dispose()
    {
        OnValueChanged = null;
        //EndEdit = null;
    }
}
