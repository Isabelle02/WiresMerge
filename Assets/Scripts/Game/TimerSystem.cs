using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class TimerSystem
{
    private readonly float _interval = 1f;
    private float _elapsed = 0f;
    private float _duration = 10f;
    private bool _isRunning = false;

    /// <summary>
    /// Parameter is remained seconds
    /// </summary>
    public Action<float> IntervalElapsed;
    public Action TimerElapsed;

    public bool IsRunning 
    {
        get => _isRunning;
        set
        {
            if (_isRunning == value)
                return;

            _isRunning = value;
            Run();
        }
    }

    private async void Run()
    {
        while (IsRunning)
        {
            await UniTask.Delay((int)(1000 * _interval));
            Elapse();
        }
    }

    private void Elapse()
    {
        _elapsed += _interval;
        IntervalElapsed?.Invoke(_duration - _elapsed);
        Debug.Log("interval " + (_duration - _elapsed));

        if (_elapsed >= _duration)
        {
            TimerElapsed?.Invoke();
            IsRunning = false;
            _elapsed = 0f;
            Debug.Log("timer elapsed");
        }
    }
}
