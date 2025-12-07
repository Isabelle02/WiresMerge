using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindow : BaseWindow, IClickable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private TextMeshProUGUI _persText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private GridLayoutGroup _choicesGrid;
    [SerializeField] private List<DialogChoiceButton> _userChoices = new List<DialogChoiceButton>();
    [SerializeField] private BaseButton _pauseButton;

    private DialogSystem _dialogSystem;
    private float _delay = 0.05f;
    private bool _isTyping = false;
    private CancellationTokenSource _cancellationTokenSource;
    private string _textToType = "";

    public Collider2D Collider => _collider;

    public void Start()
    {
        MouseManager.AddClickable(this);
    }

    public override async UniTask OnOpen()
    {
        Debug.Log("On Open Dialog");
        await base.OnOpen();

        Gameplay.DialogSystem.OnNextStep += UpdateUI;
        Gameplay.DialogSystem.Start(LevelManager.LastDialogNodeId);

        _pauseButton.OnButtonClick += OnPauseClick;
        MouseManager.AddClickable(_pauseButton);
    }

    private void OnPauseClick(BaseButton button)
    {
        WindowManager.Open<PausePopup>();
    }

    private void OnButtonClick(BaseButton button)
    {
        if (_isTyping)
            return;

        var success = _dialogSystem.NextStep((button as DialogChoiceButton).Node);
        if (!success)
        {
            LoadGame();
        }
    }

    public void OnClick()
    {
        if (_isTyping)
        {
            StopAnimation();
            return;
        }

        if (_userChoices.Count != 0)
            return;

        var success = _dialogSystem.NextPersNode();
        if (!success)
        {
            LoadGame();
        }
    }

    private void LoadGame()
    {
        Debug.Log("GAME");
        gameObject.SetActive(false);
        LevelManager.ShowLevel();
        WindowManager.Open<GameWindow>();
    }

    private void UpdateUI()
    {
        foreach (var choice in _userChoices)
        {
            if (choice != null)
            {
                choice.OnButtonClick -= OnButtonClick;
                MouseManager.RemoveClickable(choice);
                Pool<DialogChoiceButton>.Release(choice);
            }
        }

        _userChoices.Clear();

        if (!_dialogSystem.CurrentRootNode.IsPlayer)
        {
            _nameText.text = _dialogSystem.CurrentRootNode.Speaker;
            _textToType = _dialogSystem.CurrentRootNode.FormattedText ?? "";
            StartAnimation();
        }
        else
        {
            ShowUserChoices();
        }
    }

    private async UniTask TypeText(string textToType, CancellationToken token)
    {
        _isTyping = true;
        _persText.text = "";

        _choicesGrid.gameObject.SetActive(false);

        try
        {
            if (string.IsNullOrEmpty(textToType))
            {
                _persText.text = "";
                OnAnimationComplete();
                return;
            }

            foreach (char letter in textToType)
            {
                token.ThrowIfCancellationRequested();
                _persText.text += letter;
                await UniTask.Delay((int)(_delay * 1000));
            }

            OnAnimationComplete();
        }
        catch (OperationCanceledException)
        {
            OnAnimationInterrupted();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in TypeText: {ex.Message}");
            OnAnimationInterrupted();
        }
    }

    private void OnAnimationComplete()
    {
        _isTyping = false;
        ShowUserChoices();
    }

    private void OnAnimationInterrupted()
    {
        _isTyping = false;

        if (!string.IsNullOrEmpty(_textToType))
        {
            _persText.text = _textToType;
        }

        ShowUserChoices();
    }

    private void ShowUserChoices()
    {
        if (_dialogSystem?.NextDialogNodes == null)
            return;

        _choicesGrid.gameObject.SetActive(true);

        foreach (var node in _dialogSystem.NextDialogNodes)
        {
            if (!node.IsPlayer)
                continue;

            var button = Pool<DialogChoiceButton>.Get(_choicesGrid.transform);
            button.Node = node;
            button.OnButtonClick += OnButtonClick;
            button.SetText(node.FormattedText);
            _userChoices.Add(button);
            MouseManager.AddClickable(button);
        }
    }

    public void StartAnimation()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        TypeText(_textToType, _cancellationTokenSource.Token).Forget();
    }

    public void StopAnimation()
    {
        _cancellationTokenSource?.Cancel();
        _persText.text = _textToType;
        _isTyping = false;
    }

    public override UniTask OnClose()
    {
        StopAnimation();

        _dialogSystem.OnNextStep -= UpdateUI;
        _pauseButton.OnButtonClick -= OnPauseClick;
        MouseManager.RemoveClickable(_pauseButton);

        foreach (var choice in _userChoices)
        {
            if (choice != null)
            {
                choice.OnButtonClick -= OnButtonClick;
                MouseManager.RemoveClickable(choice);
                Pool<DialogChoiceButton>.Release(choice);
            }
        }
        _userChoices.Clear();

        return base.OnClose();
    }
}
