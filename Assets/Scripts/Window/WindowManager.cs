using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WindowManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Transform _windowParent;
    [SerializeField] private Transform _popupParent;
    [SerializeField] private List<BaseWindow> _windows = new List<BaseWindow>();

    private static WindowManager _instance;
    private BaseWindow _currentWindow;
    private List<BaseWindow> _initedWindows = new List<BaseWindow>();
    private Stack<BaseWindow> _windowsStack = new Stack<BaseWindow>();

    private bool _isNamed;

    public void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        Open<MenuWindow>();

        //if (_isNamed = false)
        //{
        Open<UserNameInputPopup>(); // Why don't open?????
        //_isNamed = true; // Writing to a save file
        //}
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
        var window = _initedWindows.FirstOrDefault(w => w.GetType() == typeof(T));
        if (!window)
        {
            window = Init(typeof(T));
            if (!window)
                return;
        }

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
        CloseAllPopups();
        if (_windowsStack.Count > 0)
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

    private void CloseAllPopups()
    {
        while (_windowsStack.Count > 0 && _currentWindow.IsPopup)
        {
            _windowsStack.Pop().CloseForce();
            _currentWindow = _windowsStack.Peek();
        }
    }

    private async UniTask CloseInternal()
    {
        CloseAllPopups();

        if (_windowsStack.Count < 2)
            return;

        await _windowsStack.Pop().Close();
        _currentWindow = _windowsStack.Peek();
        _currentWindow.Open();
    }

    private BaseWindow Init(System.Type t)
    {
        var windowPrefab = _windows.FirstOrDefault(w => w.GetType() == t);
        if (!windowPrefab)
            return null;

        var window = Instantiate(windowPrefab, windowPrefab.IsPopup ? _popupParent : _windowParent);
        _initedWindows.Add(window);
        return window;
    }
}
