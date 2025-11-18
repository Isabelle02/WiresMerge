using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPopup : BaseWindow
{
    //[SerializeField] private DialogGraph _graph;
    [SerializeField] private Collider2D _collider;
    //[SerializeField] private TextMeshProUGUI _questionText;
    //[SerializeField] private GridLayoutGroup _answersGrid;
    //[SerializeField] private List<DialogChoiceButton> _userChoices = new List<DialogChoiceButton>();

    public override async UniTask OnOpen()
    {
    }

    public override async UniTask OnClose()
    {
    }
}
