using Cysharp.Threading.Tasks;
using UnityEngine;

public class SettingsPopup : BaseWindow
{
    [SerializeField] private BaseButton _exitButton;
    [SerializeField] private BaseButton _closeButton;
    [SerializeField] private BaseInputField _userNameInputField;
    [SerializeField] private BaseSlider _musicVolumeSlider;
    [SerializeField] private BaseSlider _soundVolumeSlider;

    public override async UniTask OnOpen()
    {
        _userNameInputField.DisplayTextValue = Gameplay.UserName;

        _exitButton.OnButtonClick += OnCloseButton;
        _closeButton.OnButtonClick += OnCloseButton;
        _userNameInputField.OnValueChanged += OnUserNameValueChanged;
        _musicVolumeSlider.OnValueChanged += OnMusicVolumeChanged;
        _soundVolumeSlider.OnValueChanged += OnSoundVolumeChanged;

        MouseManager.AddClickable(_exitButton);
        MouseManager.AddClickable(_closeButton);
        MouseManager.AddClickable(_userNameInputField);
        MouseManager.AddClickable(_musicVolumeSlider);
        MouseManager.AddClickable(_soundVolumeSlider);

    }

    private void OnCloseButton(BaseButton button)
    {
        OnUserNameValueChanged(_userNameInputField.DisplayTextValue);
        WindowManager.ClosePopup();
    }

    private void OnUserNameValueChanged(string userName)
    {
        if (!string.IsNullOrEmpty(_userNameInputField.DisplayTextValue))
        {
            Gameplay.UserName = _userNameInputField.DisplayTextValue;
        }
        Debug.Log($"User name: {Gameplay.UserName}");
    }

    private void OnMusicVolumeChanged(float volume)
    {
        Debug.Log($"Music val: {(int)volume}");
        // Here you can apply the value, for example:
        // AudioManager.SetMusicVolume(volume / 100f); || ((int)volume)
    }

    private void OnSoundVolumeChanged(float volume)
    {
        Debug.Log($"SoundUI val: {(int)volume}");
        // Here you can apply the value, for example:
        // AudioManager.SetSoundVolume(volume / 100f); || ((int)volume)
    }

    public override async UniTask OnClose()
    {
        _exitButton.OnButtonClick -= OnCloseButton;
        _closeButton.OnButtonClick -= OnCloseButton;
        _userNameInputField.OnValueChanged -= OnUserNameValueChanged;
        _musicVolumeSlider.OnValueChanged -= OnMusicVolumeChanged;
        _soundVolumeSlider.OnValueChanged -= OnSoundVolumeChanged;

        MouseManager.RemoveClickable(_exitButton);
        MouseManager.RemoveClickable(_closeButton);
        MouseManager.RemoveClickable(_userNameInputField);
        MouseManager.RemoveClickable(_musicVolumeSlider);
        MouseManager.RemoveClickable(_soundVolumeSlider);

    }
}
