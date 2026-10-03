using UnityEngine;
using UnityEngine.InputSystem;

public class DistractionThrower : MonoBehaviour
{
    public GameObject distractionPrefab;
    public Transform throwPoint;
    public float throwForce = 8f;
    public KeyCode throwKey = KeyCode.F;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
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
