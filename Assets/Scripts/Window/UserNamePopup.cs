using Cysharp.Threading.Tasks;
using UnityEngine;

public class UserNamePopup : BaseWindow
{
    [SerializeField] BaseInputField _userNameInput;
    [SerializeField] BaseButton _closeButton;

    public override async UniTask OnOpen()
    {
        _closeButton.OnButtonClick += OnCloseButton;
        _userNameInput.OnValueChanged += OnUserNameValueChanged;

        MouseManager.AddClickable(_closeButton);
        MouseManager.AddClickable(_userNameInput);
    }

    private void OnUserNameValueChanged(string userName)
    {
        if (string.IsNullOrEmpty(_userNameInput.DisplayTextValue))
            Gameplay.UserName = _userNameInput.InitialTextValue;
        else
            Gameplay.UserName = _userNameInput.DisplayTextValue;
    }

    private void OnCloseButton(BaseButton button)
    {
        Debug.Log($"User name: {Gameplay.UserName}");
        WindowManager.ClosePopup();
    }

    public override async UniTask OnClose()
    {
        _closeButton.OnButtonClick -= OnCloseButton;
        _userNameInput.OnValueChanged -= OnUserNameValueChanged;

        MouseManager.RemoveClickable(_closeButton);
        MouseManager.RemoveClickable(_userNameInput);
    }

}
