using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameWindow : BaseWindow
{
    [SerializeField] private BaseButton _pauseButton;

    [SerializeField] private BaseButton _Button;
    [SerializeField] private BaseButton _ButtonLose;
    [SerializeField] private BaseButton _ButtonWin;

    public override async UniTask OnOpen()
    {
        _pauseButton.OnButtonClick += OnPauseClick;
        MouseManager.AddClickable(_pauseButton);


        _Button.OnButtonClick += OndClick;
        _ButtonLose.OnButtonClick += OnLoseClick;
        _ButtonWin.OnButtonClick += OnWinClick;
        MouseManager.AddClickable(_Button);
        MouseManager.AddClickable(_ButtonLose);
        MouseManager.AddClickable(_ButtonWin);
    }

    private void OnPauseClick(BaseButton button)
    {
        WindowManager.Open<PausePopup>();
    }

    private void OndClick(BaseButton button)
    {
        WindowManager.Open<QuestionPopup>();
    }

    private void OnLoseClick(BaseButton button)
    {
        WindowManager.Open<LosePopup>();
    }

    private void OnWinClick(BaseButton button)
    {
        WindowManager.Open<WinPopup>();
    }

    public override async UniTask OnClose()
    {
        _pauseButton.OnButtonClick -= OnPauseClick;
        MouseManager.RemoveClickable(_pauseButton);


        _Button.OnButtonClick -= OndClick;
        _ButtonLose.OnButtonClick -= OnLoseClick;
        _ButtonWin.OnButtonClick -= OnWinClick;
        MouseManager.RemoveClickable(_Button);
        MouseManager.RemoveClickable(_ButtonLose);
        MouseManager.RemoveClickable(_ButtonWin);
    }
}
