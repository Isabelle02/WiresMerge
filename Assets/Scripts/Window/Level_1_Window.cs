using Cysharp.Threading.Tasks;
using UnityEngine;

public class Level_1_Window : BaseWindow
{
    [SerializeField] private BaseButton _menuButton;

    public override async UniTask OnOpen()
    {
        _menuButton.OnButtonClick += OnMenuClick;
        MouseManager.AddClickable(_menuButton);
    }

    private void OnMenuClick(BaseButton button)
    {
        WindowManager.Open<MenuPopup>();
    }

    public override async UniTask OnClose()
    {
        _menuButton.OnButtonClick -= OnMenuClick;
        MouseManager.RemoveClickable(_menuButton);
    }
}
