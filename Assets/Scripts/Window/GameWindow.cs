using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameWindow : BaseWindow
{
    [SerializeField] private BaseButton _menuButton;

    public override async UniTask OnOpen()
    {
        _menuButton.OnButtonClick += OnMenuClick;
        MouseManager.AddClickable(_menuButton);
    }

    private void OnMenuClick(BaseButton button)
    {
        WindowManager.Open<PausePopup>();
    }

    public override async UniTask OnClose()
    {
        _menuButton.OnButtonClick -= OnMenuClick;
        MouseManager.RemoveClickable(_menuButton);
    }
}
