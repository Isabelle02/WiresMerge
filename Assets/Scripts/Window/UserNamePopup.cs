using Cysharp.Threading.Tasks;
using UnityEngine;

public class UserNamePopup : BaseWindow
{
    [SerializeField] BaseInputField _userNameInput;
    [SerializeField] BaseButton _closeButton;

    private string _updateUserName;

    public override async UniTask OnOpen()
    {
        _userNameInput.Init(Gameplay.DefaultUserName, "");

        _closeButton.OnButtonClick += OnCloseButton;
        _userNameInput.OnValueChanged += OnUserNameValueChanged;

        MouseManager.AddClickable(_closeButton);
        MouseManager.AddClickable(_userNameInput);
    }

    private void OnUserNameValueChanged(string userName)
    {
        _updateUserName = _userNameInput.DisplayTextValue;
    }

    private void OnCloseButton(BaseButton button)
    {
        if (!string.IsNullOrEmpty(_userNameInput.DisplayTextValue))
            Gameplay.UserName = _updateUserName;
        else
            Gameplay.UserName = _userNameInput.InitialTextValue;

        Debug.Log($"Gameplay User name: {Gameplay.UserName}");
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
