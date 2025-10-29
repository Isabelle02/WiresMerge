using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuWindow : BaseWindow
{
    [SerializeField] private BaseButton _playButton;

    public override async UniTask OnOpen()
    {
        _playButton.OnButtonClick += OnPlayClick;
        MouseManager.AddClickable(_playButton);
    }

    private void OnPlayClick(BaseButton button)
    {
        WindowManager.Open<DialogWindow>();
    }

    public override async UniTask OnClose()
    {
        _playButton.OnButtonClick -= OnPlayClick;
        MouseManager.RemoveClickable(_playButton);

    }

}
