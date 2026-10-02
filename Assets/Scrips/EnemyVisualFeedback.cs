using UnityEngine;

/// <summary>
/// Cambia el color de un indicador (por ejemplo una esfera/ícono flotando sobre el
/// enemigo) según su estado actual, para que el jugador pueda leer qué tan en peligro
/// está de un vistazo.
/// </summary>
[RequireComponent(typeof(EnemyController))]
public class EnemyVisualFeedback : MonoBehaviour
{
    [Tooltip("Renderer del indicador (ej: una esfera chica flotando sobre la cabeza)")]
    public Renderer indicatorRenderer;

    public Color patrolColor = Color.green;
    public Color investigateColor = new Color(1f, 0.6f, 0f);
    public Color suspiciousColor = Color.yellow;
    public Color chaseColor = Color.red;

    private EnemyController controller;
    private MaterialPropertyBlock propertyBlock;

    void Awake()
    {
        controller = GetComponent<EnemyController>();
        propertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (indicatorRenderer == null) return;

        Color target = controller.CurrentState switch
        {
            EnemyState.Patrol => patrolColor,
            EnemyState.Investigating => investigateColor,
            EnemyState.Suspicious => suspiciousColor,
            EnemyState.Chase => chaseColor,
            _ => patrolColor
        };

        // MaterialPropertyBlock evita crear una instancia de material por enemigo.
        indicatorRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", target);
        indicatorRenderer.SetPropertyBlock(propertyBlock);
    }
}
