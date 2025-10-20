using Cysharp.Threading.Tasks;

public class ErrorPopup : BaseWindow
{
    public override async UniTask OnOpen()
    {
        await UniTask.Delay(2000);
        WindowManager.ClosePopup();
    }

    public override async UniTask OnClose()
    {
    }
}
