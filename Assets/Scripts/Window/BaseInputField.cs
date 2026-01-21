using System;
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
    [SerializeField] private Color _unselectedColor = Color.white;
    [SerializeField] private Text caretText;

    private bool _isSelected = false;

    private string _lastText;
    private bool caretVisible = true;
    private float caretBlinkRate = 0.5f;
    private float caretTimer = 0f;

    private TouchScreenKeyboard _keyboard;

    public Collider2D Collider => _collider;
    public string InitialTextValue => _initialText.text;
    public string DisplayTextValue => _displayText.text;
    public Action<string> OnValueChanged { get; set; }

    public void Init(string initialText, string displayText)
    {
        _isSelected = false;
        _initialText.text = initialText;
        _displayText.text = displayText;
        UpdateVisuals();
    }

    public virtual void OnClick()
    {
        _isSelected = true;
        caretText.gameObject.SetActive(true);
        UpdateVisuals();
#if UNITY_ANDROID
        _keyboard = TouchScreenKeyboard.Open(DisplayTextValue, TouchScreenKeyboardType.Default);
#endif
    }

    private void Update()
    {
#if UNITY_ANDROID
        if (_keyboard != null)
        {
            if (_keyboard.active)
            {
                if (_keyboard.text != DisplayTextValue)
                {
                    _displayText.text = _keyboard.text;
                    OnValueChanged?.Invoke(_displayText.text);
                    UpdateVisuals();
                }
            }
            else if (_keyboard.done || _keyboard.wasCanceled)
            {
                _keyboard = null;
                _isSelected = false;
                UpdateVisuals();
            }
        }
#else
        if (_isSelected && Input.anyKeyDown)
        {
            foreach (var c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (_displayText.text.Length > 0)
                    {
                        _displayText.text = _displayText.text.Substring(0, _displayText.text.Length - 1);
                    }
                }
                else if (c == '\n' || c == '\r') // Enter
                {
                    _isSelected = false;
                }
                else
                {
                    _displayText.text += c;
                    _displayText.text = _displayText.text;
                }
            }
            UpdateVisuals();
            OnValueChanged?.Invoke(_displayText.text);
        }
#endif
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (_isSelected && hit.collider != _collider)
            {
                _isSelected = false;
                UpdateVisuals();
            }
        }
        UpdateCaret();
    }

    private void UpdateVisuals()
    {
        _background.color = _isSelected ? _selectedColor : _unselectedColor;
        _initialText.gameObject.SetActive(string.IsNullOrEmpty(_displayText.text));
    }

    private void UpdateCaret()
    {
        if (_isSelected)
        {
            caretTimer += Time.deltaTime;
            if (caretTimer >= caretBlinkRate)
            {
                caretVisible = !caretVisible;
                caretText.gameObject.SetActive(caretVisible);
                caretTimer = 0f;
            }

            if (_lastText != _displayText.text)
            {
                UpdateCaretPosition();
                _lastText = _displayText.text;
            }
        }
        else
        {
            caretText.gameObject.SetActive(false);
        }
    }

    private void UpdateCaretPosition()
    {
        if (string.IsNullOrEmpty(_displayText.text))
        {
            caretText.rectTransform.anchoredPosition = new Vector2(0, 0);
            return;
        }

        float textWidth = _displayText.preferredWidth;
        float xOffset = 0f;

        switch (_displayText.alignment)
        {
            case TextAnchor.MiddleCenter:
                xOffset = -textWidth * 0.5f;
                break;
            default:
                xOffset = 0f;
                break;
        }

        float caretOffset = 2f;
        float caretX = xOffset + textWidth + caretOffset;
        caretText.rectTransform.anchoredPosition = new Vector2(caretX, 0);
    }

    public virtual void Dispose()
    {
        OnValueChanged = null;
    }
}
