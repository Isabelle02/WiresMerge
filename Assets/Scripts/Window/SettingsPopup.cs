using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class SettingsPopup : BaseWindow
{
    [SerializeField] private BaseButton _exitButton;
    [SerializeField] private BaseButton _closeButton;
    [SerializeField] private BaseInputField _userNameInputField;
    [SerializeField] private BaseSlider _musicVolumeSlider;
    [SerializeField] private BaseSlider _soundUIVolumeSlider;

    public override async UniTask OnOpen()
    {
        _exitButton.OnButtonClick += OnExitButton;
        _closeButton.OnButtonClick += OnExitButton;
        _userNameInputField.OnValueChanged += OnUserNameValueChanged;
        _musicVolumeSlider.OnValueChanged += OnMusicVolumeChanged;
        _soundUIVolumeSlider.OnValueChanged += OnSoundUIVolumeChanged;

        MouseManager.AddClickable(_exitButton);
        MouseManager.AddClickable(_closeButton);
        MouseManager.AddClickable(_userNameInputField);
        MouseManager.AddClickable(_musicVolumeSlider);
        MouseManager.AddClickable(_soundUIVolumeSlider);

    }
    private void OnExitButton(BaseButton button)
    {
        WindowManager.ClosePopup();
    }

    private void OnUserNameValueChanged(string userName)
    {
        Debug.Log($"User name: {userName}");
        //
    }

    private void OnMusicVolumeChanged(float volume)
    {
        Debug.Log($"Music val: {(int)volume}");
        // Здесь можно применить значение, например:
        // AudioListener.volume = volume / 100f;
    }

    private void OnSoundUIVolumeChanged(float volume)
    {
        Debug.Log($"SoundUI val: {(int)volume}");
        // Здесь можно применить значение, например:
        // AudioListener.volume = volume / 100f;
    }

    public override async UniTask OnClose()
    {
        _exitButton.OnButtonClick -= OnExitButton;
        _closeButton.OnButtonClick -= OnExitButton;
        _userNameInputField.OnValueChanged -= OnUserNameValueChanged;
        _musicVolumeSlider.OnValueChanged -= OnMusicVolumeChanged;
        _soundUIVolumeSlider.OnValueChanged -= OnSoundUIVolumeChanged;

        MouseManager.RemoveClickable(_exitButton);
        MouseManager.RemoveClickable(_closeButton);
        MouseManager.RemoveClickable(_userNameInputField);
        MouseManager.RemoveClickable(_musicVolumeSlider);
        MouseManager.RemoveClickable(_soundUIVolumeSlider);

    }
}
