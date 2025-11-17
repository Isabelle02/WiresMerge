using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    private static SceneHandler _instance { get; set; }

    public static string MainScene => "MainScene";
    public static string GameScene => "GameScene";

    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public static void LoadScene(string sceneName)
    {
        _instance.InternalLoadScene(sceneName);
    }

    public static void QuitProgram()
    {
        _instance.InternalQuitProgram();
    }

    private void InternalLoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void InternalQuitProgram()
    {
        Application.Quit();
    }
}
