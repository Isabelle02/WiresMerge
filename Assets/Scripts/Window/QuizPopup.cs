using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizPopup : BaseWindow
{
    [SerializeField] private TextMeshProUGUI _questionText;
    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private List<QuizAnswerButton> _userAnswers = new List<QuizAnswerButton>();
    [SerializeField] private BaseButton _closeButton;
    [SerializeField] private Image _closeButtonImage;

    private static int _correctAnswers = 0;

    public static int CorrectAnswers => _correctAnswers;

    public override async UniTask OnOpen()
    {
        _closeButton.gameObject.SetActive(false);

        UpdateUI();

        _closeButton.OnButtonClick += OnCloseButton;
        MouseManager.AddClickable(_closeButton);
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
            button.SetText((i + 1).ToString(), node.FormattedText);
            _userAnswers.Add(button);
            MouseManager.AddClickable(button);
        }
    }

    private async void OnButtonClick(BaseButton button)
    {
        if (button is not QuizAnswerButton quizButton) return;

        var quizSystem = Gameplay.QuizSystem;
        var isCorrect = quizSystem.NextStep(quizButton.Node);

        if (isCorrect)
        {
            _correctAnswers++;
        }

        quizButton.AnimateBackgroundColor(new Color32(0x71, 0x52, 0x3E, 160));

        foreach (var answer in _userAnswers)
        {
            answer.AnimateTextColor(quizSystem.NextStep(answer.Node) ? Color.green : Color.red);
        }

        await UniTask.Delay(1200);
        _closeButton.gameObject.SetActive(true);
        _closeButtonImage.color = new Color(_closeButtonImage.color.r, _closeButtonImage.color.g, _closeButtonImage.color.b, 0f);
        _closeButtonImage.DOFade(1f, 1f);
    }

    private void OnCloseButton(BaseButton button)
    {
        WindowManager.ClosePopup();
    }

    public override async UniTask OnClose()
    {
        _closeButton.OnButtonClick -= OnCloseButton;
        MouseManager.RemoveClickable(_closeButton);
    }
}
