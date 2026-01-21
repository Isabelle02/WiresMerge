using Cysharp.Threading.Tasks;
using UnityEngine;

public class WinPopup : BaseWindow
{
    [SerializeField] private BaseButton _nextButton;
    [SerializeField] private BaseButton _exitButton;

    public override async UniTask OnOpen()
    {
        AudioManager.PlayOneShot(Sound.WinPopup);

        _nextButton.OnButtonClick += OnNextButton;
        _exitButton.OnButtonClick += OnExitButton;

        MouseManager.AddClickable(_nextButton);
        MouseManager.AddClickable(_exitButton);
    }

    private void OnNextButton(BaseButton button)
    {
        WindowManager.Open<DialogWindow>();
    }

    private void OnExitButton(BaseButton button)
    {
        SceneHandler.LoadScene(SceneHandler.MainScene);
        WindowManager.Open<MenuWindow>();
    }

    public override async UniTask OnClose()
    {
        _nextButton.OnButtonClick -= OnNextButton;
        _exitButton.OnButtonClick -= OnExitButton;

        MouseManager.RemoveClickable(_nextButton);
        MouseManager.RemoveClickable(_exitButton);
    }

}

