using Cysharp.Threading.Tasks;

public class MenuWindow : BaseWindow
{
    private int _count = 1;

    public override async UniTask OnOpen()
    {
        if (_count == 1)
        {
            await UniTask.Delay(2000);
            WindowManager.Open<InfoWindow>();
            _count = 0;
        }
    }

    public override async UniTask OnClose()
    {
        
    }
}
