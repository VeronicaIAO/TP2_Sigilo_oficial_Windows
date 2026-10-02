using UnityEngine;

/// <summary>
/// Cámara en tercera persona que orbita alrededor de un punto de referencia (el jugador)
/// usando el cursor del mouse. No está emparentada al jugador: cada frame recalcula
/// su posición en base al target, así que un giro constante del jugador nunca la afecta,
/// y ella tampoco fuerza ninguna rotación sobre el jugador.
/// Requiere una capa de colisión (por ejemplo "Level") para el chequeo de obstáculos.
/// </summary>
public class ThirdPersonCameraOrbit : MonoBehaviour
{
    [Header("Referencia")]
    [Tooltip("Punto alrededor del cual orbita la cámara (ej: un Empty a la altura del pecho del jugador)")]
    public Transform target;

    [Header("Distancia y altura")]
    public float distance = 5f;
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Sensibilidad del cursor")]
    public float sensitivityX = 3f;
    public float sensitivityY = 2f;
    public float minPitch = -20f;
    public float maxPitch = 70f;

    [Header("Colisión con el escenario")]
    public LayerMask collisionMask;
    public float collisionBuffer = 0.25f;

    private float yaw;
    private float pitch = 15f;

    void Start()
    {
        // Cursor bloqueado y oculto para poder usarlo como "joystick" de la cámara
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 startAngles = transform.eulerAngles;
        yaw = startAngles.y;
        pitch = startAngles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- Rotación con el cursor ---
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * sensitivityX;
        pitch -= mouseY * sensitivityY; // invertido para que "arriba" en el mouse mire hacia arriba
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + targetOffset;

        // --- Posición deseada según la distancia configurada ---
        Vector3 desiredPosition = pivot - (rotation * Vector3.forward * distance);

        // --- Evitar que la cámara atraviese paredes/obstáculos ---
        float finalDistance = distance;
        if (Physics.Linecast(pivot, desiredPosition, out RaycastHit hit, collisionMask))
        {
            finalDistance = Mathf.Max(hit.distance - collisionBuffer, 0.2f);
        }

        transform.position = pivot - (rotation * Vector3.forward * finalDistance);
        transform.rotation = rotation;
    }
}
