using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseWindow : MonoBehaviour
{
    [SerializeField] private bool _isPopup;

    public bool IsPopup => _isPopup;

    public void Open()
    {
        gameObject.SetActive(true);
        OnOpen();
    }

    public async UniTask Close()
    {
        await OnClose();
        gameObject.SetActive(false);
    }

    public void CloseForce()
    {
        OnClose();
        gameObject.SetActive(false);
    }

    public virtual async UniTask OnOpen()
    {

    }
    public virtual async UniTask OnClose()
    {

    }
}