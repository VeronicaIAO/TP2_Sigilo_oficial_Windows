using UnityEngine;

public enum EnemyState { Patrol, Suspicious, Investigating, Chase }

/// <summary>
/// Decide qué hacer con lo que EnemyDetection percibe. No calcula raycasts ni
/// conos de visión acá — eso es responsabilidad de EnemyDetection (Nota 10).
///
/// Estados (Nota 7): Patrol -> Suspicious (lo vio, todavía no está seguro) -> Chase
/// (confirmado, lo persigue). Investigating es el estado extra para reaccionar a
/// distracciones/ruido (Nota 10) sin confundirlo con haber visto al jugador.
///
/// Nota 4 (modo simple, sin ocultamiento): activar loseOnDetectionOnly en el
/// GameManager para que ser detectado sea derrota inmediata, sin estados intermedios.
/// Nota 7 (con ocultamiento): dejarlo en false — ahí la derrota es ser atrapado, y
/// esconderse corta la persecución y vuelve a patrullar.
/// </summary>
[RequireComponent(typeof(EnemyDetection))]
public class EnemyController : MonoBehaviour
{
    [Header("Patrullaje")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float rotationSpeed = 6f;
    public float waypointTolerance = 0.3f;
    public float waitTimeAtPoint = 1.5f;

    [Header("Tiempos de estado")]
    public float suspiciousToChaseTime = 1.2f;
    public float loseTargetTime = 3f;
    public float investigateWaitTime = 2f;

    [Header("Captura")]
    public float catchDistance = 1f;

    public EnemyState CurrentState { get; private set; } = EnemyState.Patrol;

    private EnemyDetection detection;
    private int patrolIndex;
    private float waitTimer;
    private float stateTimer;
    private Vector3 investigatePoint;

    void Awake()
    {
        detection = GetComponent<EnemyDetection>();
    }

    void OnEnable() => PlayerHidingStatus.OnPlayerHidden += HandlePlayerHidden;
    void OnDisable() => PlayerHidingStatus.OnPlayerHidden -= HandlePlayerHidden;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        if (GameManager.Instance != null && GameManager.Instance.loseOnDetectionOnly)
        {
            // Modo Nota 4: sin estados, ver al jugador ya es game over.
            Patrol();
            if (detection.PlayerDetected)
            {
                GameManager.Instance.Defeat("detectado por el enemigo");
            }
            return;
        }

        switch (CurrentState)
        {
            case EnemyState.Patrol:
                Patrol();
                if (detection.PlayerDetected) EnterState(EnemyState.Suspicious);
                break;

            case EnemyState.Suspicious:
                FaceTarget(detection.PlayerPosition);
                stateTimer += Time.deltaTime;
                if (!detection.PlayerDetected) { EnterState(EnemyState.Patrol); break; }
                if (stateTimer >= suspiciousToChaseTime) EnterState(EnemyState.Chase);
                break;

            case EnemyState.Investigating:
                MoveTowards(investigatePoint, patrolSpeed);
                if (detection.PlayerDetected) { EnterState(EnemyState.Suspicious); break; }
                if (Vector3.Distance(transform.position, investigatePoint) <= waypointTolerance)
                {
                    stateTimer += Time.deltaTime;
                    if (stateTimer >= investigateWaitTime) EnterState(EnemyState.Patrol);
                }
                break;

            case EnemyState.Chase:
                if (detection.PlayerDetected)
                {
                    stateTimer = 0f;
                    MoveTowards(detection.PlayerPosition, chaseSpeed);
                    if (Vector3.Distance(transform.position, detection.PlayerPosition) <= catchDistance)
                    {
                        GameManager.Instance.Defeat("atrapado por el enemigo");
                    }
                }
                else
                {
                    stateTimer += Time.deltaTime;
                    if (stateTimer >= loseTargetTime) EnterState(EnemyState.Patrol);
                }
                break;
        }
    }

    /// Llamado por un NoiseEmitter cuando algo suena cerca (distracción, Nota 10).
    public void Distract(Vector3 noisePosition)
    {
        if (CurrentState == EnemyState.Chase) return; // ya está seguro de dónde está el jugador
        investigatePoint = noisePosition;
        EnterState(EnemyState.Investigating);
    }

    private void HandlePlayerHidden()
    {
        // Requisito Nota 7: al esconderse, corta la persecución y vuelve a patrullar.
        if (CurrentState == EnemyState.Chase || CurrentState == EnemyState.Suspicious)
        {
            EnterState(EnemyState.Patrol);
        }
    }

    private void EnterState(EnemyState newState)
    {
        CurrentState = newState;
        stateTimer = 0f;
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform point = patrolPoints[patrolIndex];
        if (Vector3.Distance(transform.position, point.position) <= waypointTolerance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waitTimer = 0f;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            }
            return;
        }

        MoveTowards(point.position, patrolSpeed);
    }

    private void MoveTowards(Vector3 destination, float speed)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        transform.position += direction.normalized * speed * Time.deltaTime;
        FaceTarget(destination);
    }

    private void FaceTarget(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
