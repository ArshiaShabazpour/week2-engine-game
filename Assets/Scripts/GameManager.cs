using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Won, Lost, Timeout }
    public GameState State { get; private set; } = GameState.Playing;

    [Header("Gameplay")]
    public int totalPickups = 5;
    public int playerHealth = 5;
    public float timeLimit = 20f; 
    [Header("refs for UI")]
    public PlayerMovement playerRef; 
    public EnemyController enemyRef;

    int collected = 0;
    float timeLeft;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        timeLeft = timeLimit;
        Time.timeScale = 1f; 
    }

    void Update()
    {
        var kb = Keyboard.current;
        bool pausePressed =
            (kb != null && kb.escapeKey.wasPressedThisFrame);
        bool resetPressed =
            (kb != null && kb.rKey.wasPressedThisFrame);


        if (State == GameState.Playing)
        {
            timeLeft -= Time.unscaledDeltaTime * Time.timeScale; 
            if (timeLeft <= 0f) SetState(GameState.Timeout);
        }

        if ((State == GameState.Won || State == GameState.Lost || State == GameState.Timeout) && resetPressed)
            Reload();
    }

    public void OnPickup(GameObject pickup)
    {
        if (State != GameState.Playing) return;
        collected++;
        Destroy(pickup);
    }

    public void OnHitEnemy()
    {
        if (State != GameState.Playing) return;
        playerHealth--;
        if (playerHealth <= 0) SetState(GameState.Lost);
    }

    public void OnReachGoal()
    {
        if (State != GameState.Playing) return;
        if (collected >= totalPickups) SetState(GameState.Won);
    }

    public void OnFallOut()
    {
        if (State == GameState.Playing) SetState(GameState.Lost);
    }

    void SetState(GameState newState)
    {
        State = newState;
        switch (State)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
            case GameState.Won:
            case GameState.Lost:
            case GameState.Timeout:
                Time.timeScale = 0f;
                break;
        }
    }
    void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 500, 240));
        GUILayout.Label($"Pickups: {collected}/{totalPickups}");
        GUILayout.Label($"Health: {playerHealth}");
        GUILayout.Label($"Time Left: {Mathf.Max(0f, timeLeft):0.0}s");


        if (State == GameState.Won) GUILayout.Label("You WON! Press R/Select to Restart");
        if (State == GameState.Lost) GUILayout.Label("You LOST! Press R/Select to Restart");
        if (State == GameState.Timeout) GUILayout.Label("TIME OUT! Press R/Select to Restart");
        GUILayout.EndArea();
    }
}
