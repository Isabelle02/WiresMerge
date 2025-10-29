using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

        _dialogSystem = new DialogSystem(_graph);
        _dialogSystem.OnNextStep += UpdateUI;
        _dialogSystem.Start();
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
            _nameText.text = _dialogSystem.CurrentRootNode.Speaker;
        }

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
