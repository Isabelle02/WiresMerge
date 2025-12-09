using System;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    private static Gameplay _instance;
    public static readonly string DefaultUserName = "Лев";

    public static Action Started;
    public static Action Finished;

    public static Transform Transform => _instance.transform;

    public static WireSystem WireSystem { get; private set; }
    public static TimerSystem TimerSystem { get; private set; }
    public static DialogSystem DialogSystem { get; private set; }
    public static QuizSystem QuizSystem { get; private set; }

    private static string _userName;
    public static string UserName
    {
        get
        {
            if (string.IsNullOrEmpty(_userName))
                _userName = PlayerPrefs.GetString("UserName", DefaultUserName);

            return _userName;
        }
        set
        {
            if (_userName == value)
                return;

            _userName = value;
            PlayerPrefs.SetString("UserName", value);
        }
    }

    public static bool IsAllWin
    {
        get
        {
            return true;
        }
    }

    void Awake()
    {
        if (!_instance)
        {
            _instance = this;

            WireSystem = new WireSystem();
            DialogSystem = new DialogSystem();
            QuizSystem = new QuizSystem();
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
    }

    public static void Play()
    {
        TimerSystem = new TimerSystem();

        _instance.gameObject.SetActive(true);
        TimerSystem.IsRunning = true;
        TimerSystem.Duration = LevelManager.LastTimerDuration;

        Started?.Invoke();
    }

    public static void Finish()
    {
        TimerSystem.IntervalElapsed = null;
        TimerSystem.TimerElapsed = null;
        TimerSystem.IsRunning = false;
        TimerSystem = null;
        Started = null;

        Finished?.Invoke();
    }
}
