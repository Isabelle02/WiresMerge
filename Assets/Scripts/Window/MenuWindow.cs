using Cysharp.Threading.Tasks;

public class MenuWindow : BaseWindow
{
    public override async UniTask OnOpen()
    {
        await UniTask.Delay(2000);
        //LevelManager.LoadLevel(LevelManager.LastId + 1);
        LevelManager.LoadLevel(0);
        WindowManager.Open<DialogWindow>();
    }

    public override async UniTask OnClose()
    {
        
    }
}
