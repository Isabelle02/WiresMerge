using UnityEngine;

public class Gameplay : MonoBehaviour
{
    private static Gameplay _instance;

    public static WireSystem WireSystem { get; private set; }
    public static TimerSystem TimerSystem { get; private set; }
    public static string UserName { get; private set; } = "User";

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
