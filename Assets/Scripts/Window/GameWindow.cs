using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameWindow : BaseWindow
{
    [SerializeField] private BaseButton _pauseButton;
    [SerializeField] private Text _timerValue;

    public override async UniTask OnOpen()
    {
        Gameplay.Started += OnStarted;
        Gameplay.Finished += OnFinished;
        Gameplay.WireSystem.Win += OnWIn;
        _pauseButton.OnButtonClick += OnPauseClick;
        MouseManager.AddClickable(_pauseButton);
    }

    private void OnWIn()
    {
        Gameplay.TimerSystem.IsRunning = false;
        LevelManager.LoadLevel(LevelManager.LastId + 1);
        Gameplay.Win();
        Debug.Log("Score: " + Gameplay.Score);
        WindowManager.Open<WinPopup>();
    }

    private void OnFinished()
    {
        Gameplay.Started += OnStarted;
    }

    private void OnStarted()
    {
        Debug.Log("on started game window");
        _timerValue.text = LevelManager.LastTimerDuration.ToString();
        Gameplay.TimerSystem.IntervalElapsed += OnIntervalElapsed;
        Gameplay.TimerSystem.TimerElapsed += OnTimerElapsed;
    }

    private void OnPauseClick(BaseButton button)
    {
        WindowManager.Open<PausePopup>();
    }

    private void OnIntervalElapsed(float remained)
    {
        _timerValue.text = ((int)remained).ToString();
    }

    private void OnTimerElapsed()
    {
        Debug.Log("LOSE");
        WindowManager.Open<LosePopup>();
    }

    public override async UniTask OnClose()
    {
        _pauseButton.OnButtonClick -= OnPauseClick;
        MouseManager.RemoveClickable(_pauseButton);

        Gameplay.Finished -= OnFinished;
        Gameplay.WireSystem.Win -= OnWIn;
        Gameplay.Finish();
    }
}
