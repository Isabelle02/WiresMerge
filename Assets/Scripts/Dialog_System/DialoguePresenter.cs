using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum DialoguePresenterState
{
    Await,
    Talk,
    Choice,
    Finish
}

public class DialoguePresenter : MonoBehaviour, IClickable
{
    [Header("Clickable")]
    [SerializeField] private Collider2D _collider;
    [SerializeField] private bool _isClickable;

    [Header("Components")]
    [SerializeField] private DialogView _dialogueView;
    [SerializeField] private TextAsset[] _dialogueArray;
    private uint _currentDialogue;
    private DialogNode _currentNode;

    [Header("Events")]
    [SerializeField] private UnityEvent[] OnDialogueFinished;
    public event Action OnDialogStart;

    [field: Header("States")]
    [SerializeField] private DialoguePresenterState _state;
    private bool _messageTyping;

    private byte _start = 0;

    public Collider2D Collider { get => _collider; }


    //private void Awake()
    //{
    //    if (_collider == null) _collider = GetComponent<Collider2D>();
    //    if (_dialogueView == null) _dialogueView = FindObjectOfType<DialogView>();

    //    _state = DialoguePresenterState.Await;
    //}


    private void OnEnable()
    {
        _dialogueView.OnFinishMessage += StopTyping;
    }

    private void OnDisable()
    {
        _dialogueView.OnFinishMessage -= StopTyping;
    }

    public void OnClick()
    {
        Debug.Log("Click handled. State: {_state}, Start: {_start}, Typing: {_messageTyping}");
        Debug.Log($"Click handled. State: {_state}, Start: {_start}, Typing: {_messageTyping}");
        if (_start > 0) NextMessage();
        else StartDialogue();
    }

    private void StopTyping()
    {
        _messageTyping = false;
    }

    public void StartDialogue()
    {
        if (_state == DialoguePresenterState.Finish)
        {
            return;
        }

        _currentNode = ParseDialogFile.GetDialogTree(_dialogueArray[_currentDialogue]);

        _dialogueView.SetPresenter(this);
        OnDialogStart?.Invoke();
        _dialogueView.StartDialogue(_currentNode.Message, _currentNode.Name);
        _state = DialoguePresenterState.Talk;
        _messageTyping = true;
        _start++;
    }

    public void NextMessage()
    {
        Debug.Log($"NextMessage called. State: {_state}, Typing: {_messageTyping}, Children: {_currentNode?.Children.Count}");
        if (_state != DialoguePresenterState.Talk) return;

        if (_messageTyping)
        {
            _dialogueView.StopAnimation(_currentNode.Message);
            return;
        }

        if (_currentNode.Children.Count == 0)
        {
            FinishDialogue();
            return;
        }

        GoToChildMessage();

    }

    private void GoToChildMessage()
    {
        if (_currentNode.Children.Count == 1)
        {
            _currentNode = _currentNode.Children[0];
            _dialogueView.NextMessage(_currentNode.Message, _currentNode.Name);
            _messageTyping = true;
        }
        else
        {
            List<string> answers = new List<string>();
            foreach (var child in _currentNode.Children)
            {
                string answerText = !string.IsNullOrEmpty(child.Answer) ? child.Answer : "Ответ";
                answers.Add(answerText);
                Debug.Log($"Adding answer: {answerText}");
            }
            _dialogueView.ActivateButtons(answers);
            _state = DialoguePresenterState.Choice;
        }
    }

    private void FinishDialogue()
    {
        _dialogueView.StopDialogue();
        if (_currentNode.ActionId != null)
        {
            OnDialogueFinished[(int)_currentNode.ActionId].Invoke();
        }

        _state = DialoguePresenterState.Await;
        _currentDialogue++;
        if (_currentDialogue > _dialogueArray.Length - 1)
        {
            _state = DialoguePresenterState.Finish;
        }
    }

    public void SwitchBranch(int index)
    {
        Debug.Log($"Button click: {index}");
        _currentNode = _currentNode.Children[index];
        _state = DialoguePresenterState.Talk;
        _dialogueView.NextMessage(_currentNode.Message, _currentNode.Name);
        _messageTyping = true;
    }

    public void Action0()
    {
        Debug.Log("Action 0");
    }

    public void Action1()
    {
        Debug.Log("Action 1");
    }

}
