using UnityEngine;

/// <summary>
/// Responsabilidad única: percibir al jugador. No decide qué hacer con esa información
/// (eso lo maneja EnemyController) — separación de responsabilidades pedida en Nota 10.
///
/// - Detección primaria: Raycast dentro de un cono de visión, respetando distancia y
///   obstáculos entre enemigo y jugador (Nota 4).
/// - Detección secundaria: OverlapSphere (equivalente a un SphereCollider de detección
///   cercana), que ignora el ángulo de visión — el jugador "se choca" con el radar del
///   enemigo (Nota 4).
/// - Detección progresiva: en vez de detectar de golpe, acumula un nivel de 0 a 1. Por
///   debajo de alertThreshold el enemigo no reacciona, entre alertThreshold y
///   detectionThreshold está en duda, y al llegar a 1 lo detecta del todo (Nota 10).
/// - Respeta zonas de ocultamiento: si el jugador tiene PlayerHidingStatus.IsHidden en
///   true, no lo detecta aunque esté en el cono de visión (Nota 7).
/// </summary>
public class EnemyDetection : MonoBehaviour
{
    [Header("Detección primaria (Raycast)")]
    public Transform eyePoint;
    public float viewDistance = 10f;
    [Range(0, 180)] public float viewAngle = 90f;
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [Header("Detección secundaria (zona cercana)")]
    public float closeRangeRadius = 2.5f;

    [Header("Detección progresiva")]
    public float detectionSpeed = 1f;
    public float detectionDecaySpeed = 0.6f;
    [Range(0, 1)] public float alertThreshold = 0.4f;
    [Range(0, 1)] public float detectionThreshold = 1f;

    public float DetectionLevel { get; private set; }
    public bool PlayerSuspicious => DetectionLevel >= alertThreshold;
    public bool PlayerDetected => DetectionLevel >= detectionThreshold;
    public Vector3 PlayerPosition { get; private set; }

    private Transform player;
    private PlayerHidingStatus hidingStatus;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            hidingStatus = playerObj.GetComponent<PlayerHidingStatus>();
        }
    }

    void Update()
    {
        if (player == null) return;

        bool playerHidden = hidingStatus != null && hidingStatus.IsHidden;
        bool sees = !playerHidden && (CheckRaycastVision() || CheckCloseRange());

        DetectionLevel += (sees ? detectionSpeed : -detectionDecaySpeed) * Time.deltaTime;
        DetectionLevel = Mathf.Clamp01(DetectionLevel);

        if (sees)
        {
            PlayerPosition = player.position;
        }
    }

    private bool CheckRaycastVision()
    {
        Vector3 origin = eyePoint != null ? eyePoint.position : transform.position;
        Vector3 toPlayer = player.position - origin;
        float distance = toPlayer.magnitude;

        if (distance > viewDistance) return false;

        float angle = Vector3.Angle(transform.forward, toPlayer);
        if (angle > viewAngle * 0.5f) return false;

        if (Physics.Raycast(origin, toPlayer.normalized, out RaycastHit hit, distance, obstacleMask | playerMask))
        {
            return ((1 << hit.collider.gameObject.layer) & playerMask) != 0;
        }
        return false;
    }

    private bool CheckCloseRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, closeRangeRadius, playerMask);
        return hits.Length > 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, closeRangeRadius);

        Gizmos.color = Color.cyan;
        Vector3 origin = eyePoint != null ? eyePoint.position : transform.position;
        Vector3 forwardLeft = Quaternion.Euler(0, -viewAngle * 0.5f, 0) * transform.forward;
        Vector3 forwardRight = Quaternion.Euler(0, viewAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(origin, forwardLeft * viewDistance);
        Gizmos.DrawRay(origin, forwardRight * viewDistance);
    }
}
