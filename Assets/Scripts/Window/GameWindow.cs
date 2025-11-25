using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : BaseWindow
{
    [SerializeField] private BaseButton _pauseButton;
    [SerializeField] private Text _timerValue;

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
    }

    public override async UniTask OnClose()
    {
        _pauseButton.OnButtonClick -= OnPauseClick;
        MouseManager.RemoveClickable(_pauseButton);

        Gameplay.TimerSystem.IntervalElapsed -= OnIntervalElapsed;
        Gameplay.TimerSystem.TimerElapsed -= OnTimerElapsed;
    }
}
