using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    protected static Pool _instance;
    private static PoolConfig _config;

    protected static PoolConfig Config
    {
        get
        {
            if (_config == null)
                _config = Resources.Load<PoolConfig>("PoolConfig");

            return _config;
        }
    }

    void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}

public class Pool<T> : Pool where T : MonoBehaviour
{
    private static Stack<T> _poolObjects = new Stack<T>();

    public static T Get(Transform parent = null)
    {
        var obj = default(T);
        if (_poolObjects.Count == 0)
        {
            var prefab = Config.Get<T>();
            if (prefab)
                obj = (T)Instantiate(prefab, null);
        }
        else
            obj = _poolObjects.Pop();

        obj.gameObject.SetActive(true);
        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = Vector3.zero;
        return (T)obj;
    }

    public static void Release(T obj)
    {
        if (obj is IDisposable disposable)
            disposable.Dispose();

        obj.gameObject.SetActive(false);
        obj.transform.SetParent(_instance.transform);
        _poolObjects.Push(obj);
    }
}
