using System;
using UnityEngine;

public class BaseButton : MonoBehaviour, IClickable, IDisposable
{
    [SerializeField] private Collider2D _collider;

    public Collider2D Collider => _collider;
    public Action<BaseButton> OnButtonClick { get; set; }

    public virtual void OnClick()
    {
        OnButtonClick?.Invoke(this);
    }

    public virtual void Dispose()
    {
        OnButtonClick = null;
    }
}
