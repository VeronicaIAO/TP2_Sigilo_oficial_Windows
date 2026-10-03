using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
    }

    public void Defeat(string reason)
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log($"Derrota: {reason}");
    }

    public void RestartLevel()
    {
        IsGameOver = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
