using UnityEngine;

/// <summary>
/// Va en el jugador. Al apretar la tecla configurada, instancia un objeto (con
/// Rigidbody + NoiseEmitter) y lo tira hacia adelante para generar una distracción.
/// </summary>
public class DistractionThrower : MonoBehaviour
{
    public GameObject distractionPrefab;
    public Transform throwPoint;
    public float throwForce = 8f;
    public KeyCode throwKey = KeyCode.F;

    void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            Throw();
        }
    }

    private void Throw()
    {
        if (distractionPrefab == null) return;

        Vector3 spawnPos = throwPoint != null ? throwPoint.position : transform.position + transform.forward;
        GameObject obj = Instantiate(distractionPrefab, spawnPos, Quaternion.identity);

        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.AddForce(transform.forward * throwForce + transform.up * 2f, ForceMode.VelocityChange);
        }
    }
}
