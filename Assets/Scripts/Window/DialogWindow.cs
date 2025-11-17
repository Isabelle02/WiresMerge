using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindow : BaseWindow, IClickable
{
    [SerializeField] private DialogGraph _graph;
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
    private string _textToType;

    public Collider2D Collider => _collider;

    public void Start()
    {
        MouseManager.AddClickable(this);
    }

    public override async UniTask OnOpen()
    {
        Debug.Log("On Open Dialog");
        await base.OnOpen();

        _dialogSystem = new DialogSystem(_graph);
        _dialogSystem.OnNextStep += UpdateUI;
        _dialogSystem.Start();

        _pauseButton.OnButtonClick += OnPauseClick;
        MouseManager.AddClickable(_pauseButton);
    }

    private void OnPauseClick(BaseButton button)
    {
        WindowManager.Open<PausePopup>();
    }

    private void OnButtonClick(BaseButton button)
    {
        OnClickInternal((button as DialogChoiceButton).Node);
    }

    public void OnClick()
    {
        if (_userChoices.Count == 0)
            OnClickInternal(_dialogSystem.NextDialogNodes.FirstOrDefault());
    }

    private void OnClickInternal(DialogNode node)
    {
        var success = _dialogSystem.NextStep(node);
        if (!success)
        {
            Debug.Log("GAME");
            //close dialogs, go to game
            WindowManager.Open<GameWindow>();
        }
    }

    private void UpdateUI()
    {
        foreach (var choice in _userChoices)
            Pool<DialogChoiceButton>.Release(choice);

        _userChoices.Clear();
        if (!_dialogSystem.CurrentRootNode.IsPlayer)
        {
            _persText.text = _dialogSystem.CurrentRootNode.FormattedText;
            //_textToType = _dialogSystem.CurrentRootNode.FormattedText;
            //if (!_isTyping)
            //{
            //    StartAnimation();
            //}
            //else
            //{
            //    StopAnimation();
            //}
            _nameText.text = _dialogSystem.CurrentRootNode.Speaker;
        }

        foreach (var node in _dialogSystem.NextDialogNodes)
        {
            if (!node.IsPlayer)
                continue;

            //if (!_isTyping)
            //    continue;

            var button = Pool<DialogChoiceButton>.Get(_choicesGrid.transform);
            button.Node = node;
            button.OnButtonClick += OnButtonClick;
            button.SetText(node.FormattedText);
            _userChoices.Add(button);
            MouseManager.AddClickable(button);

        }
    }

    private async UniTask TypeText(string textToType, CancellationToken token)
    {
        _isTyping = true;
        _persText.text = "";

        foreach (char letter in textToType)
        {
            token.ThrowIfCancellationRequested();
            _persText.text += letter;
            await UniTask.Delay((int)(_delay * 1000));
        }

        _isTyping = false;
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
        _dialogSystem.OnNextStep -= UpdateUI;
        _pauseButton.OnButtonClick -= OnPauseClick;
        MouseManager.RemoveClickable(_pauseButton);

        return base.OnClose();
    }
}
