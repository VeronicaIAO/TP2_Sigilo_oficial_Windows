using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyVisualFeedback : MonoBehaviour
{
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

        indicatorRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", target);
        propertyBlock.SetColor("_BaseColor", target);
        indicatorRenderer.SetPropertyBlock(propertyBlock);
    }
}