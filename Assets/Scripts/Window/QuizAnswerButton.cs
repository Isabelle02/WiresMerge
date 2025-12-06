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

    public void SetTextColor(Color color)
    {
        _titleText.color = color;
    }

    public void SetBackgroundColor(Color color)
    {
        _backgroundImage.color = color;
    }

    public override void OnClick()
    {
        base.OnClick();
    }
}

