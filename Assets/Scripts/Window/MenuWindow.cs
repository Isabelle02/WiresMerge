using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuWindow : BaseWindow
{
    [SerializeField] private BaseButton _playButton;
    [SerializeField] private BaseButton _settingsButton;
    [SerializeField] private BaseButton _quitButton;

    public override async UniTask OnOpen()
    {
        _playButton.OnButtonClick += OnPlayClick;
        _settingsButton.OnButtonClick += OnSettingsClick;
        _quitButton.OnButtonClick += OnQuitButton;

        MouseManager.AddClickable(_playButton);
        MouseManager.AddClickable(_settingsButton);
        MouseManager.AddClickable(_quitButton);
        
        //LevelManager.LoadLevel(LevelManager.LastId + 1);
    }

    private async void OnPlayClick(BaseButton button)
    {
        await SceneHandler.LoadScene(SceneHandler.GameScene);
        LevelManager.LoadLevel(0);
        WindowManager.Open<DialogWindow>();
    }

    private void OnSettingsClick(BaseButton button)
    {
        WindowManager.Open<SettingsPopup>();
    }

    private void OnQuitButton(BaseButton button)
    {
        SceneHandler.QuitProgram();
    }

    public override async UniTask OnClose()
    {
        _playButton.OnButtonClick -= OnPlayClick;
        _settingsButton.OnButtonClick -= OnSettingsClick;
        _quitButton.OnButtonClick -= OnQuitButton;

        MouseManager.RemoveClickable(_playButton);
        MouseManager.RemoveClickable(_settingsButton);
        MouseManager.RemoveClickable(_quitButton);
    }

}
