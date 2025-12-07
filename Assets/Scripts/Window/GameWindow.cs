using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : BaseWindow
{
    [SerializeField] private BaseButton _pauseButton;
    [SerializeField] private Text _timerValue;

    public void Update()
    {
        if (WireSystem.IsWin)
        {
            Gameplay.TimerSystem.IsRunning = false;
            WindowManager.Open<WinPopup>();
        }
    }

    public override async UniTask OnOpen()
    {
        _pauseButton.OnButtonClick += OnPauseClick;
        MouseManager.AddClickable(_pauseButton);

        Gameplay.TimerSystem.IntervalElapsed += OnIntervalElapsed;
        Gameplay.TimerSystem.TimerElapsed += OnTimerElapsed;
    }

    private void OnPauseClick(BaseButton button)
    {
        WindowManager.Open<PausePopup>();
    }

    private void OnIntervalElapsed(float tick)
    {
        _timerValue.text = (LevelManager.LastTimerDuration--).ToString();
        Debug.Log("LastTimerDuration " + LevelManager.LastTimerDuration);
    }

    private void OnTimerElapsed()
    {
        Debug.Log("LOSE");
        WindowManager.Open<LosePopup>();
    }

    public override async UniTask OnClose()
    {
        Gameplay.Finish();

        _pauseButton.OnButtonClick -= OnPauseClick;
        MouseManager.RemoveClickable(_pauseButton);

        Gameplay.TimerSystem.IntervalElapsed -= OnIntervalElapsed;
        Gameplay.TimerSystem.TimerElapsed -= OnTimerElapsed;
    }
}
