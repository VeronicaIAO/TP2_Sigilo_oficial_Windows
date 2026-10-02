using System;
using UnityEngine;

/// <summary>
/// Solo guarda si el jugador está escondido o no. Va en el mismo GameObject que
/// PlayerMovement. Se separa en su propio script porque es un dato que necesitan
/// consultar otros sistemas (EnemyDetection, UI) sin acoplarse al script de movimiento.
/// </summary>
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
