using UnityEngine;

public class Gameplay : MonoBehaviour
{
    private static Gameplay _instance;
    public static readonly string DefaultUserName = "Лев";

    public static WireSystem WireSystem { get; private set; }
    public static TimerSystem TimerSystem { get; private set; }

    private static string _userName;
    public static string UserName
    {
        get
        {
            var stored = PlayerPrefs.GetString("UserName", null);
            if (string.IsNullOrEmpty(stored))
            {
                _userName = DefaultUserName;
                PlayerPrefs.SetString("UserName", _userName);
            }
            else
            {
                _userName = stored;
            }

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

    void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            WireSystem = new WireSystem();
            TimerSystem = new TimerSystem();
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        TimerSystem.IsRunning = true;
    }

    void Update()
    {
    }
}
