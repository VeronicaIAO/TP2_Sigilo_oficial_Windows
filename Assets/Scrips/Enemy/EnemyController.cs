using UnityEngine;
 
public enum EnemyState { Patrol, Suspicious, Investigating, Chase }
[RequireComponent(typeof(EnemyDetection))]
public class EnemyController : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;
    public float rotationSpeed = 6f;
    public float waypointTolerance = 0.3f;
    public float waitTimeAtPoint = 1.5f;

    public float suspiciousToChaseTime = 1.2f;
    public float loseTargetTime = 3f;
    public float investigateWaitTime = 2f;

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
     // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;
 
        if (GameManager.Instance != null && GameManager.Instance.loseOnDetectionOnly)
        {
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
                if (HorizontalDistance(transform.position, investigatePoint) <= waypointTolerance)
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
                    if (HorizontalDistance(transform.position, detection.PlayerPosition) <= catchDistance)
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
 
    private void HandlePlayerHidden()
    {
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
        if (HorizontalDistance(transform.position, point.position) <= waypointTolerance)
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

    private float HorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }
}
