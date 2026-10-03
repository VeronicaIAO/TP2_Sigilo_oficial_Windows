using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCameraOrbit : MonoBehaviour
{
    public Transform target;


    public float distance = 5f;
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    public float sensitivityX = 3f;
    public float sensitivityY = 2f;
    public float minPitch = -20f;
    public float maxPitch = 70f;


    public LayerMask collisionMask;
    public float collisionBuffer = 0.25f;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 startAngles = transform.eulerAngles;
        yaw = startAngles.y;
        pitch = startAngles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;


        float mouseX = 0f;
        float mouseY = 0f;

        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseX = mouseDelta.x;
            mouseY = mouseDelta.y;
        }

        yaw += mouseX * sensitivityX;
        pitch -= mouseY * sensitivityY; // invertido para que "arriba" en el mouse mire hacia arriba
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + targetOffset;


        Vector3 desiredPosition = pivot - (rotation * Vector3.forward * distance);


        float finalDistance = distance;
        if (Physics.Linecast(pivot, desiredPosition, out RaycastHit hit, collisionMask))
        {
            finalDistance = Mathf.Max(hit.distance - collisionBuffer, 0.2f);
        }

        transform.position = pivot - (rotation * Vector3.forward * finalDistance);
        transform.rotation = rotation;
    }
}
