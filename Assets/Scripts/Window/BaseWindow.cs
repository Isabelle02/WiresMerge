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
        Pool.Release(this);
    }

    public void CloseForce()
    {
        OnClose();
        Pool.Release(this);
    }

    public virtual async UniTask OnOpen()
    {

    }
    public virtual async UniTask OnClose()
    {

    }
}