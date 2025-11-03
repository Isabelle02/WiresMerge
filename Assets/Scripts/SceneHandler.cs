using UnityEngine;

public class SceneHandler : MonoBehaviour
{
    public static SceneHandler Instance { get; private set; }

    public static string MainScene => "MainScene";
    public static string GameScene => "GameScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
