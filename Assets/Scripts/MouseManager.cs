using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private static MouseManager _instance;
    private List<IClickable> _clickables = new List<IClickable>();

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

    public static void AddClickable(IClickable clickable)
    {
        if (!_instance._clickables.Contains(clickable))
             _instance._clickables.Add(clickable);
    }

    public static void RemoveClickable(IClickable clickable)
    {
        _instance._clickables.Remove(clickable);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePosition = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            var clickables = new List<IClickable>(_clickables);
            foreach (var clickable in clickables) 
            {
                if (hit.collider == clickable.Collider)
                    clickable.OnClick();
            }
        }
    }
}
