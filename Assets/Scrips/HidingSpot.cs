using UnityEngine;

/// <summary>
/// Poner en un Collider marcado como Trigger (un arbusto, un placard, una sombra, etc.).
/// Al entrar, marca al jugador como escondido; al salir, deja de estarlo.
/// </summary>
[RequireComponent(typeof(Collider))]
public class HidingSpot : MonoBehaviour
{
    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerHidingStatus hiding))
        {
            hiding.SetHidden(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerHidingStatus hiding))
        {
            hiding.SetHidden(false);
        }
    }
}
