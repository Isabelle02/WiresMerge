using TMPro;
using UnityEngine;

public class QuizAnswerButton : BaseButton
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private TextMeshProUGUI _titleText;

    public DialogNode Node { get; set; }

    public void SetText(string title, string text)
    {
        _text.text = text;
        _titleText.text = title;
    }

    public void SetColor(Color color)
    {
        _text.color = color;
    }

    public override void OnClick()
    {
        base.OnClick();
    }
}

