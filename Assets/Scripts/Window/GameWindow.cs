using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameWindow : BaseWindow
{
    [SerializeField] private BaseButton _pauseButton;

    public override async UniTask OnOpen()
    {
        _pauseButton.OnButtonClick += OnPauseClick;
        MouseManager.AddClickable(_pauseButton);
    }

    private void OnPauseClick(BaseButton button)
    {
        WindowManager.Open<PausePopup>();
    }

    public override async UniTask OnClose()
    {
        _pauseButton.OnButtonClick -= OnPauseClick;
        MouseManager.RemoveClickable(_pauseButton);
    }
}
