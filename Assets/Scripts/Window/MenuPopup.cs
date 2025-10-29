using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuPopup : BaseWindow
{
    [SerializeField] private BaseButton _playButton;
    [SerializeField] private BaseButton _settingsButton;
    [SerializeField] private BaseButton _exitButton;

    public override async UniTask OnOpen()
    {
        _playButton.OnButtonClick += OnPlayClick;
        _settingsButton.OnButtonClick += OnSettingsClick;
        _exitButton.OnButtonClick += OnExitButton;

        MouseManager.AddClickable(_playButton);
        MouseManager.AddClickable(_settingsButton);
        MouseManager.AddClickable(_exitButton);
    }

    private void OnPlayClick(BaseButton button)
    {
        this.CloseForce();
        Debug.Log("Click");
    }

    private void OnSettingsClick(BaseButton button)
    {
        WindowManager.Open<SettingsPopup>();
    }

    private void OnExitButton(BaseButton button)
    {
        WindowManager.Open<MenuWindow>();
        ///
        WindowManager.Close<DialogWindow>();
        ///
    }

    public override async UniTask OnClose()
    {
        _playButton.OnButtonClick -= OnPlayClick;
        _settingsButton.OnButtonClick -= OnSettingsClick;
        _exitButton.OnButtonClick -= OnExitButton;

        MouseManager.RemoveClickable(_playButton);
        MouseManager.RemoveClickable(_settingsButton);
        MouseManager.RemoveClickable(_exitButton);
    }
}
