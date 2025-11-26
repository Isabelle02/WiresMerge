using Cysharp.Threading.Tasks;
using System.Collections.Generic;
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
        var success = Gameplay.DialogSystem.NextStep((button as DialogChoiceButton).Node);
        if (!success)
        {
            LoadGame();
        }
    }

    public void OnClick()
    {
        if (_userChoices.Count != 0)
            return;
        
        var success = Gameplay.DialogSystem.NextPersNode();
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
        //close dialogs, go to game
    }

    private void UpdateUI()
    {
        foreach (var choice in _userChoices)
            Pool.Release(choice);

        _userChoices.Clear();
        if (!Gameplay.DialogSystem.CurrentRootNode.IsPlayer)
        {
            _persText.text = Gameplay.DialogSystem.CurrentRootNode.FormattedText;
            _nameText.text = Gameplay.DialogSystem.CurrentRootNode.Speaker;
        }

        foreach (var node in Gameplay.DialogSystem.NextDialogNodes)
        {
            if (!node.IsPlayer)
                continue;

            var button = Pool.Get<DialogChoiceButton>(_choicesGrid.transform);
            button.Node = node;
            button.OnButtonClick += OnButtonClick;
            button.SetText(node.FormattedText);
            _userChoices.Add(button);
            MouseManager.AddClickable(button);

        }
    }

    public override UniTask OnClose()
    {
        Gameplay.DialogSystem.OnNextStep -= UpdateUI;
        _pauseButton.OnButtonClick -= OnPauseClick;
        MouseManager.RemoveClickable(_pauseButton);

        return base.OnClose();
    }
}
