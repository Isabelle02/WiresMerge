using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> _poolables = new List<MonoBehaviour>();

    protected static Pool _instance;
    protected static List<MonoBehaviour> Poolables => _instance._poolables;

    void Start()
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
            var prefab = Poolables.FirstOrDefault(p => p.GetType() == typeof(T));
            if (prefab)
                obj = (T)Instantiate(prefab, null);
        }
        else
            obj = _poolObjects.Pop();

        obj.gameObject.SetActive(true);
        obj.transform.SetParent(parent);
        obj.transform.localScale = Vector3.one;
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
