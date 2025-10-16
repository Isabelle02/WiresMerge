using UnityEngine;

public sealed class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    private static CameraManager _instance;

    public static Camera MainCamera => _instance._mainCamera;

    void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
