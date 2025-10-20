using Cysharp.Threading.Tasks;

public class MenuWindow : BaseWindow
{
    public override async UniTask OnOpen()
    {
        await UniTask.Delay(2000);
        WindowManager.Open<DialogWindow>();
    }

    public override async UniTask OnClose()
    {
        
    }
}
