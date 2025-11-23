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

    private string _lockedInitialText = Gameplay.DefaultUserName;
    private bool _isSelected = false;

    private string _lastText;
    private bool caretVisible = true;
    private float caretBlinkRate = 0.5f;
    private float caretTimer = 0f;

    public Collider2D Collider => _collider;
    public string InitialTextValue => _lockedInitialText;
    public string DisplayTextValue
    {
        get => _displayText.text;
        set
        {
            _displayText.text = value;
            UpdateVisuals();
        }
    }
    public bool IsSelected
    {
        get => _isSelected;
        set => _isSelected = value;
    }
    public Action<string> OnValueChanged { get; set; }

    private void Awake()
    {
        UpdateVisuals();
    }

    public virtual void OnClick()
    {
        _isSelected = true;
        caretText.gameObject.SetActive(true);
        UpdateVisuals();
    }

    private void Update()
    {
        if (_isSelected && Input.anyKeyDown)
        {
            foreach (var c in Input.inputString)
            {
                if (c == '\b') // Backspace
                {
                    if (_displayText.text.Length > 0)
                    {
                        _displayText.text = _displayText.text.Substring(0, _displayText.text.Length - 1);
                        OnValueChanged?.Invoke(_displayText.text);
                    }
                }
                else if (c == '\n' || c == '\r') // Enter
                {
                    _isSelected = false;
                    OnValueChanged?.Invoke(_displayText.text);
                }
                else
                {
                    _displayText.text += c;
                    _displayText.text = _displayText.text;
                    OnValueChanged?.Invoke(_displayText.text);
                }
            }
            UpdateVisuals();
        }
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
        if (_initialText.text != _lockedInitialText)
        {
            Debug.LogWarning("Attempt to change _initialText.text");
            _initialText.text = _lockedInitialText;
        }
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
