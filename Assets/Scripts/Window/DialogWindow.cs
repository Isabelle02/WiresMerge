using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogWindow : BaseWindow, IClickable
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private TextMeshProUGUI _persText;
    [SerializeField] private GridLayoutGroup _choicesGrid;
    [SerializeField] private List<DialogChoiceButton> _userChoices = new List<DialogChoiceButton>();

    private DialogSystem _dialogSystem;

    public Collider2D Collider => _collider;

    public void Start()
    {
        MouseManager.AddClickable(this);
    }

    public override async UniTask OnOpen()
    {
        Debug.Log("On Open Dialog");
        await base.OnOpen();

        _dialogSystem = new DialogSystem();
        _dialogSystem.OnNextStep += UpdateUI;
        _dialogSystem.Start(LevelManager.LastDialogNodeId);
    }

    private void OnButtonClick(BaseButton button)
    {
        var success = _dialogSystem.NextStep((button as DialogChoiceButton).Node);
        if (!success)
        {
            LoadGame();
        }
    }

    public void OnClick()
    {
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
        //close dialogs, go to game
    }

    private void UpdateUI()
    {
        foreach (var choice in _userChoices)
            Pool<DialogChoiceButton>.Release(choice);

        _userChoices.Clear();
        if (!_dialogSystem.CurrentRootNode.IsPlayer)
            _persText.text = _dialogSystem.CurrentRootNode.Speaker + " " + _dialogSystem.CurrentRootNode.FormattedText;

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

    public override UniTask OnClose()
    {
        _dialogSystem.OnNextStep -= UpdateUI;
        return base.OnClose();
    }
}
