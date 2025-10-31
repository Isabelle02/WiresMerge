using Cysharp.Threading.Tasks;
using UnityEngine;

public class SettingsPopup : BaseWindow
{
    [SerializeField] private BaseButton _exitButton;
    [SerializeField] private BaseSlider _volumeSlider;

    public override async UniTask OnOpen()
    {
        _exitButton.OnButtonClick += OnExitButton;
        _volumeSlider.OnValueChanged += OnVolumeChanged;
        MouseManager.AddClickable(_exitButton);
        MouseManager.AddClickable(_volumeSlider);

    }
    private void OnExitButton(BaseButton button)
    {
        Debug.Log("exit Click");

        WindowManager.ClosePopup();
    }

    private void OnVolumeChanged(float volume)
    {
        Debug.Log($"Sound val: {(int)volume}");
        // Здесь можно применить значение, например:
        // AudioListener.volume = volume / 100f;
    }

    public override async UniTask OnClose()
    {
        _exitButton.OnButtonClick -= OnExitButton;
        _volumeSlider.OnValueChanged -= OnVolumeChanged;
        MouseManager.RemoveClickable(_exitButton);
        MouseManager.RemoveClickable(_volumeSlider);

    }
}
