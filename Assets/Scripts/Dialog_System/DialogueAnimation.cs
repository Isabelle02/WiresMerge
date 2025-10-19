using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

public class DialogueAnimation
{
    [SerializeField] private TextMeshProUGUI _dialogueText;
    [SerializeField] private float _delay = 0.05f;

    private bool _isTyping = false;
    private CancellationTokenSource _cancellationTokenSource;

    private string dText;

    public event Action OnAnimationFinished;

    public DialogueAnimation(TextMeshProUGUI dialogueText, float delay = 0.05f)
    {
        _dialogueText = dialogueText;
        _delay = delay;
    }

    public void SetTextAlpha(float alpha)
    {
        Color color = _dialogueText.color;
        color.a = alpha;
        _dialogueText.color = color;
    }


    private async UniTask TypeText(string dText, CancellationToken token)
    {
        _isTyping = true;
        SetTextAlpha(1f);
        _dialogueText.text = "";

        foreach (char letter in dText)
        {
            token.ThrowIfCancellationRequested(); // проверка отмены
            _dialogueText.text += letter;
            await UniTask.Delay((int)(_delay * 1000));
        }

        _isTyping = false;
        OnAnimationFinished?.Invoke();
    }

    public void StartAnimation(string message)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        TypeText(message, _cancellationTokenSource.Token).Forget();
    }

    public void StopAnimation(string message)
    {
        _cancellationTokenSource?.Cancel();
        _dialogueText.text = message;
        //SetTextAlpha(0f);
        _isTyping = false;
        OnAnimationFinished?.Invoke();
    }
}












//using Cysharp.Threading.Tasks;
//using System;
//using System.Threading;
//using TMPro;
//using UnityEngine;

//public class DialogueAnimation : MonoBehaviour, IClickable
//{
//    [SerializeField] private Collider2D _collider;
//    [SerializeField] private bool _isClickable;
//    [SerializeField] private TextMeshProUGUI _dialogueText;
//    [SerializeField] private float _delay = 0.05f;

//    private bool _isTyping = false;
//    private CancellationTokenSource _cancellationTokenSource;

//    public Collider2D Collider { get => _collider; }

//    private string dText;

//    public event Action OnAnimationFinished;

//    public DialogueAnimation(TextMeshProUGUI dialogueText, float delay = 0.05f)
//    {
//        _dialogueText = dialogueText;
//        _delay = delay;
//    }

//    public void Start()
//    {
//        if (_isClickable)
//            MouseManager.AddClickable(this);
//        dText = _dialogueText.text;
//        SetTextAlpha(0f);
//    }

//    public void SetTextAlpha(float alpha)
//    {
//        Color color = _dialogueText.color;
//        color.a = alpha;
//        _dialogueText.color = color;
//    }


//    private async UniTask TypeText(string dText, CancellationToken token)
//    {
//        _isTyping = true;
//        SetTextAlpha(1f);
//        _dialogueText.text = "";

//        foreach (char letter in dText)
//        {
//            token.ThrowIfCancellationRequested(); // проверка отмены
//            _dialogueText.text += letter;
//            await UniTask.Delay((int)(_delay * 1000));
//        }

//        _isTyping = false;
//    }

//    public void StartAnimation()
//    {
//        _cancellationTokenSource = new CancellationTokenSource();
//        TypeText(dText, _cancellationTokenSource.Token).Forget();
//    }

//    public void StopAnimation()
//    {
//        _cancellationTokenSource?.Cancel();
//        _dialogueText.text = dText;
//        _isTyping = false;
//    }

//    public void OnClick()
//    {
//        if (!_isTyping)
//        {
//            //if (_cancellationTokenSource != null)
//            //{
//            //    _cancellationTokenSource.Cancel(); // отменяем предыдущую задачу
//            //    _cancellationTokenSource.Dispose();
//            //}
//            _cancellationTokenSource = new CancellationTokenSource();
//            TypeText(dText, _cancellationTokenSource.Token).Forget();
//        }
//        else
//        {
//            _cancellationTokenSource?.Cancel();
//            _dialogueText.text = dText;
//            _isTyping = false;
//        }
//    }
//}
