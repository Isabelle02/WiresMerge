using Cysharp.Threading.Tasks;
using UnityEngine;

public class SettingsPopup : BaseWindow
{
    [SerializeField] private BaseButton _exitButton;

    public override async UniTask OnOpen()
    {
        _exitButton.OnButtonClick += OnExitButton;
        MouseManager.AddClickable(_exitButton);
    }
    private void OnExitButton(BaseButton button)
    {
        this.CloseForce();
    }

    public override async UniTask OnClose()
    {
        _exitButton.OnButtonClick -= OnExitButton;
        MouseManager.RemoveClickable(_exitButton);
    }
}
