using UnityEngine;

public interface IClickable
{
    public Collider2D Collider { get; }
    public void OnClick();
}