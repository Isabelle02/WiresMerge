using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class BaseWindow : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private bool _isPopup;

    public bool IsPopup => _isPopup;

    private void Start()
    {
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.worldCamera = CameraManager.MainCamera;
    }

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
        gameObject.SetActive(false);
    }

    public virtual async UniTask OnOpen()
    {

    }
    public virtual async UniTask OnClose()
    {

    }
}