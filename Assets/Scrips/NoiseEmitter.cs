using UnityEngine;

/// <summary>
/// Poner en el prefab que se tira con DistractionThrower. Al chocar con algo,
/// avisa a todos los EnemyController dentro del radio de ruido para que vayan
/// a investigar ese punto.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class NoiseEmitter : MonoBehaviour
{
    public float noiseRadius = 6f;
    public LayerMask enemyMask;

    private bool hasEmitted;

    void OnCollisionEnter(Collision collision)
    {
        if (hasEmitted) return;
        hasEmitted = true;
        EmitNoise();
    }

    private void EmitNoise()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, noiseRadius, enemyMask);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out EnemyController enemy))
            {
                enemy.Distract(transform.position);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, noiseRadius);
    }
}
