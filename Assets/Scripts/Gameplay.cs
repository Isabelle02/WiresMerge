using UnityEngine;

public class Gameplay : MonoBehaviour
{
    private static Gameplay _instance;

    public static WireSystem WireSystem { get; private set; }
    public static string UserName { get; private set; } = "User";

    void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            WireSystem = new WireSystem();
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
