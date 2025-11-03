using UnityEngine;

public class Gameplay : MonoBehaviour
{
    private static Gameplay _instance;

    public static WireSystem WireSystem { get; private set; }
    public static TimerSystem TimerSystem { get; private set; }

    private static string _userName;
    public static string UserName
    {
        get
        {
            if (string.IsNullOrEmpty(_userName))
                _userName = PlayerPrefs.GetString("UserName", "User");

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
