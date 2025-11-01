using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuPopup : BaseWindow
{
    [SerializeField] private BaseButton _backButton;
    [SerializeField] private BaseButton _settingsButton;
    [SerializeField] private BaseButton _exitButton;
    [SerializeField] private BaseButton _closeButton;

    public override async UniTask OnOpen()
    {
        _backButton.OnButtonClick += OnCloseButton;
        _settingsButton.OnButtonClick += OnSettingsClick;
        _exitButton.OnButtonClick += OnExitButton;
        _closeButton.OnButtonClick += OnCloseButton;

        MouseManager.AddClickable(_backButton);
        MouseManager.AddClickable(_settingsButton);
        MouseManager.AddClickable(_exitButton);
        MouseManager.AddClickable(_closeButton);
    }

    private void OnSettingsClick(BaseButton button)
    {
        WindowManager.Open<SettingsPopup>();
    }

    private void OnExitButton(BaseButton button)
    {
        WindowManager.GoToScene("MainMenuScene");
        WindowManager.Open<MenuWindow>();
    }

    private void OnCloseButton(BaseButton button)
    {
        WindowManager.ClosePopup();
    }

    public override async UniTask OnClose()
    {
        _backButton.OnButtonClick -= OnCloseButton;
        _settingsButton.OnButtonClick -= OnSettingsClick;
        _exitButton.OnButtonClick -= OnExitButton;
        _closeButton.OnButtonClick -= OnCloseButton;

        MouseManager.RemoveClickable(_backButton);
        MouseManager.RemoveClickable(_settingsButton);
        MouseManager.RemoveClickable(_exitButton);
        MouseManager.RemoveClickable(_closeButton);
    }
}
