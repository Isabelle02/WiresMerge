using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizAnswerButton : BaseButton
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Image _backgroundImage;

    public DialogNode Node { get; set; }

    public void SetText(string title, string text)
    {
        _text.text = text;
        _titleText.text = title;
    }

    public void AnimateTextColor(Color color, float duration = 0.5f)
    {
        _titleText.DOColor(color, duration);
    }

    public void AnimateBackgroundColor(Color color, float duration = 0.5f)
    {
        _backgroundImage.DOColor(color, duration);
    }

    public override void OnClick()
    {
        base.OnClick();
    }
}

