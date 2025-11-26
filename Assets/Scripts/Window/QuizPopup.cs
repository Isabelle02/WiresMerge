using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizPopup : BaseWindow
{
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private List<QuizAnswerButton> _userAnswers = new List<QuizAnswerButton>();

    private readonly string _question = "Question";
    private static int _correctAnswers = 0;

    public static int CorrectAnswers => _correctAnswers;

    public override async UniTask OnOpen()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        foreach (var answer in _userAnswers)
            Pool.Release(answer);

        _userAnswers.Clear();
        _questionText.text = Gameplay.QuizSystem.CurrentRootNode.FormattedText;
        for (var i = 0; i < Gameplay.QuizSystem.NextDialogNodes.Count; i++)
        {
            var node = Gameplay.QuizSystem.NextDialogNodes[i];
            var button = Pool.Get<QuizAnswerButton>(_grid.transform);
            button.Node = node;
            button.OnButtonClick += OnButtonClick;
            button.SetText((i + 1).ToString(),node.FormattedText);
            _userAnswers.Add(button);
            MouseManager.AddClickable(button);
        }
    }

    private void OnButtonClick(BaseButton button)
    {
        var success = Gameplay.QuizSystem.NextStep((button as QuizAnswerButton).Node);
        if (success)
        {
            _correctAnswers++;
        }

        foreach (var answer in _userAnswers)
        {
            answer.SetColor(Gameplay.QuizSystem.NextStep(answer.Node) ? Color.green : Color.red);
        }
    }

    public override async UniTask OnClose()
    {
        
    }
}
