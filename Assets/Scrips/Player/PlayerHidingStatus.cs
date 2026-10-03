using System;
using UnityEngine;

public class PlayerHidingStatus : MonoBehaviour
{
    public static event Action OnPlayerHidden;

    public bool IsHidden { get; private set; }

    public void SetHidden(bool hidden)
    {
        if (IsHidden == hidden) return;
        IsHidden = hidden;
        if (hidden) OnPlayerHidden?.Invoke();
    }
}
