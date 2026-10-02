using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Maneja victoria/derrota/reinicio del nivel. Un solo punto central para que
/// GoalZone y EnemyController no tengan que saber nada de UI ni de escenas.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Condición de derrota")]
    [Tooltip("Nota 4 (sin ocultamiento): ser detectado ya es derrota. " +
             "Desactivalo cuando implementes zonas de ocultamiento (Nota 7): ahí la derrota " +
             "pasa a ser que el enemigo te atrape.")]
    public bool loseOnDetectionOnly = false;

    public bool IsGameOver { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Victory()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log("Victoria: llegaste a la meta.");
        // Enganchar acá la UI / pantalla de victoria.
    }

    public void Defeat(string reason)
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log($"Derrota: {reason}");
        // Enganchar acá la UI / pantalla de derrota.
    }

    public void RestartLevel()
    {
        IsGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
