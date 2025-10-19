//using Prepare;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class DialogView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private GameObject[] _buttons;
    [SerializeField] private TextMeshProUGUI[] _buttonsText;
    [SerializeField] private Animator _animator;

    private DialogueAnimation _dialogueAnimation;
    private DialoguePresenter _dialoguePresenter;

    [Header("Animation")]
    [SerializeField] private float _delay = 0.05f;

    public event Action OnFinishMessage;
    private bool _isDialogueRun;

    public void Awake()
    {
        _dialogueAnimation = new DialogueAnimation(_messageText, _delay);
    }

    public void SetPresenter (DialoguePresenter presenter)
    {
        _dialoguePresenter = presenter;
    }

    public void StartDialogue(string message, string name)
    {
        _isDialogueRun = true;
        _nameText.text = name;
        NewMessage(message);
        _animator.SetTrigger("Appearance");
    }

    public void StopAnimation(string message)
    {
        _dialogueAnimation.StopAnimation(message);
    }

    public void StopDialogue()
    {
        _isDialogueRun = false;
        // чтобы текст не пропадал резко: ожидаание?
        _messageText.text = "";
        // изменить прозрачность (color.a)?
        _nameText.text = "";
        _animator.SetTrigger("Disappearance");
    }

    public void NextMessage(string message, string name)
    {
        _nameText.text = name;
        NewMessage(message);
    }

    public void NewMessage (string message)
    {
        _messageText.text = message;
        _dialogueAnimation.StartAnimation(message);
    }

    public void HideButtons()
    {
        foreach (var button in _buttons)
        {
            button.SetActive(false);
        }
    }

    public void ActivateButtons(List<string> answers)
    {
        for (int i = 0; i < answers.Count; i++)
        {
            if (i < _buttons.Length)
            {
                _buttons[i].SetActive(true);
                _buttonsText[i].text = answers[i];
            }
        }
    }

    private void FinishMessage()
    {
        OnFinishMessage?.Invoke();
    }

    private void OnEnable()
    {
        _dialogueAnimation.OnAnimationFinished += OnFinishMessage;
    }

    private void OnDisable()
    {
        _dialogueAnimation.OnAnimationFinished -= OnFinishMessage;
    }

    public void ClickOnButtonChoice(int indexButton)
    {
        HideButtons();
        _dialoguePresenter.SwitchBranch(indexButton);
    }
}
