using UnityEngine;

/// <summary>
/// Movimiento del jugador relativo a la cámara, independiente de hacia dónde mire la cámara
/// en el momento (o sea: "adelante" siempre es "adelante en pantalla", sin importar el yaw/pitch
/// de la cámara). Usa CharacterController.
///
/// Fixes respecto a la versión anterior:
/// - Se aplanan (y = 0) los vectores forward/right de la cámara antes de usarlos. Si no se
///   aplanan, cuando la cámara está inclinada hacia abajo (típico offset de tercera persona)
///   su forward tiene una componente Y negativa fuerte, y eso es lo que generaba el movimiento
///   diagonal/invertido al apretar W.
/// - El jugador solo rota cuando hay input de movimiento real, y lo hace con Slerp hacia el
///   ángulo objetivo (no rotación directa por mouse ni por frame), por eso ya no gira solo.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Si se deja vacío, usa Camera.main automáticamente")]
    public Transform cameraTransform;

    [Header("Movimiento")]
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;

    [Header("Salto y gravedad")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        HandleGravityGroundCheck();
        HandleMovement();
        HandleJump();

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void HandleGravityGroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity.y < 0f)
        {
            verticalVelocity.y = -2f; // lo pega al piso, evita acumular gravedad de más
        }
    }

    private void HandleMovement()
    {
        if (cameraTransform == null) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Vectores de la cámara "aplanados": sin esto, si la cámara mira hacia abajo,
        // W termina moviendo al jugador en diagonal/hacia atrás.
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = camForward * vertical + camRight * horizontal;

        if (moveDir.sqrMagnitude > 0.001f)
        {
            moveDir.Normalize();
            controller.Move(moveDir * moveSpeed * Time.deltaTime);

            // Solo rota el jugador cuando realmente se está moviendo, y de forma suave.
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
