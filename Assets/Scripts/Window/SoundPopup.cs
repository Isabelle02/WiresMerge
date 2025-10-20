using Cysharp.Threading.Tasks;

public class SoundPopup : BaseWindow
{
    private int _count = 1;

    public override async UniTask OnOpen()
    {
        if (_count == 1)
        {
            await UniTask.Delay(2000);
            WindowManager.Open<GiftPopup>();
            _count = 0;
        }
        else if (_count == 0)
        {
            await UniTask.Delay(2000);
            WindowManager.ClosePopup();
            await UniTask.Delay(2000);
            WindowManager.Close();
        }
    }

    public override async UniTask OnClose()
    {
    }
}
