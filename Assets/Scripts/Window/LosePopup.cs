using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LosePopup : BaseWindow
{
    [SerializeField] private BaseButton _returnButton;
    [SerializeField] private BaseButton _exitButton;

    public override async UniTask OnOpen()
    {
        _returnButton.OnButtonClick += OnReturnButton;
        _exitButton.OnButtonClick += OnExitButton;

        MouseManager.AddClickable(_returnButton);
        MouseManager.AddClickable(_exitButton);
    }

    private void OnReturnButton(BaseButton button)
    {
        Gameplay.Finish();
        LevelManager.LoadLevel(LevelManager.LastId);
        LevelManager.ShowLevel();
        WindowManager.ClosePopup();
    }

    private void OnExitButton(BaseButton button)
    {
        SceneHandler.LoadScene(SceneHandler.MainScene);
        WindowManager.Open<MenuWindow>();
    }

    public override async UniTask OnClose()
    {
        _returnButton.OnButtonClick -= OnReturnButton;
        _exitButton.OnButtonClick -= OnExitButton;

        MouseManager.RemoveClickable(_returnButton);
        MouseManager.RemoveClickable(_exitButton);
    }
}

