using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    private static WindowManager _instance;
    private BaseWindow _currentWindow;
    private Stack<BaseWindow> _windowsStack = new Stack<BaseWindow>();

    public void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void Start()
    {
        Open<MenuWindow>();
    }

    public static void Open<T>() where T : BaseWindow
    {
        _instance.OpenInternal<T>();
    }

    public static async UniTask ClosePopup()
    {
        await _instance.ClosePopupInternal();
    }

    public static async UniTask Close()
    {
        await _instance.CloseInternal();
    }

    private async void OpenInternal<T>() where T : BaseWindow
    {
        var window = Pool<T>.Get(transform);
        if (window.IsPopup)
            await ClosePopupToOpenInternal();
        else
            await CloseToOpenInternal();

        window.Open();
        _windowsStack.Push(window);
        _currentWindow = window;
    }

    private async UniTask ClosePopupToOpenInternal()
    {
        if (_windowsStack.Count > 1 && _currentWindow.IsPopup)
            await _currentWindow.Close();
    }

    private async UniTask CloseToOpenInternal()
    {
        if (_windowsStack.Count > 1)
             await _currentWindow.Close();
    }

    private async UniTask ClosePopupInternal()
    {
        if (_windowsStack.Count < 2)
            return;

        if (_currentWindow.IsPopup)
        {
            await _windowsStack.Pop().Close();
            _currentWindow = _windowsStack.Peek();
        }

        if (_currentWindow.IsPopup)
            _currentWindow.Open();
    }

    private async UniTask CloseInternal()
    {
        while (_windowsStack.Count > 0 && _currentWindow.IsPopup)
        {
            _windowsStack.Pop().CloseForce();
            _currentWindow = _windowsStack.Peek();
        }

        if (_windowsStack.Count < 2)
            return;

        await _windowsStack.Pop().Close();
        _currentWindow = _windowsStack.Peek();
        _currentWindow.Open();
    }
}
