using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameWindow : BaseWindow
{
    [SerializeField] private BaseButton _pauseButton;

    //private TimerSystem timerSystem;

    public override async UniTask OnOpen()
    {
        _pauseButton.OnButtonClick += OnPauseClick;
        MouseManager.AddClickable(_pauseButton);

        Gameplay.TimerSystem.IntervalElapsed += OnIntervalElapsed;
    }

    private void OnPauseClick(BaseButton button)
    {
        WindowManager.Open<PausePopup>();
    }

    private void OnIntervalElapsed(float tick)
    {
        
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
