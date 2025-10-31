using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuPopup : BaseWindow
{
    [SerializeField] private BaseButton _playButton;
    [SerializeField] private BaseButton _settingsButton;
    [SerializeField] private BaseButton _exitButton;
    [SerializeField] private BaseButton _closeButton;

    public override async UniTask OnOpen()
    {
        _playButton.OnButtonClick += OnPlayClick;
        _settingsButton.OnButtonClick += OnSettingsClick;
        _exitButton.OnButtonClick += OnExitButton;
        _closeButton.OnButtonClick += OnCloseButton;

        MouseManager.AddClickable(_playButton);
        MouseManager.AddClickable(_settingsButton);
        MouseManager.AddClickable(_exitButton);
        MouseManager.AddClickable(_closeButton);
    }

    private void OnPlayClick(BaseButton button)
    {
        Debug.Log("play Click");
    }

    private void OnSettingsClick(BaseButton button)
    {
        Debug.Log("set Click");

        WindowManager.Open<SettingsPopup>();
    }

    private void OnExitButton(BaseButton button)
    {
        Debug.Log("exit Click");

        WindowManager.Open<MenuWindow>();
    }

    private void OnCloseButton(BaseButton button)
    {
        Debug.Log("close Click");

        WindowManager.ClosePopup();
    }

    public override async UniTask OnClose()
    {
        _playButton.OnButtonClick -= OnPlayClick;
        _settingsButton.OnButtonClick -= OnSettingsClick;
        _exitButton.OnButtonClick -= OnExitButton;
        _closeButton.OnButtonClick -= OnCloseButton;

        MouseManager.RemoveClickable(_playButton);
        MouseManager.RemoveClickable(_settingsButton);
        MouseManager.RemoveClickable(_exitButton);
        MouseManager.RemoveClickable(_closeButton);
    }
}
