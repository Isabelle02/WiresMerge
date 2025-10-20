using Cysharp.Threading.Tasks;

public class GiftPopup : BaseWindow
{
    private int _count = 1;

    public override async UniTask OnOpen()
    {
        if (_count == 1)
        {
            await UniTask.Delay(2000);
            WindowManager.Open<ErrorPopup>();
            _count = 0;
        }
        else if (_count == 0)
        {
            await UniTask.Delay(2000);
            WindowManager.ClosePopup();
        }
    }

    public override async UniTask OnClose()
    {
    }
}
