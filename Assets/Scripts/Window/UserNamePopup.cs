using Cysharp.Threading.Tasks;
using UnityEngine;

public class UserNamePopup : BaseWindow
{
    [SerializeField] BaseInputField _userNameInput;
    [SerializeField] BaseButton _enterButton;

    public string UserName;

    public override async UniTask OnOpen()
    {
        _enterButton.OnButtonClick += OnEnterButton;
        _userNameInput.OnValueChanged += OnUserNameValueChanged;

        MouseManager.AddClickable(_enterButton);
        MouseManager.AddClickable(_userNameInput);
    }

    private void OnUserNameValueChanged(string userName)
    {
        _userNameInput.InitText();
        Gameplay.UserName = _userNameInput.text;
    }

    private void OnEnterButton(BaseButton button)
    {
        OnUserNameValueChanged(_userNameInput.text);
        Debug.Log($"User name: {UserName}");
        WindowManager.ClosePopup();
    }

    public override async UniTask OnClose()
    {
        _enterButton.OnButtonClick -= OnEnterButton;
        _userNameInput.OnValueChanged -= OnUserNameValueChanged;

        MouseManager.RemoveClickable(_enterButton);
        MouseManager.RemoveClickable(_userNameInput);
    }

}
