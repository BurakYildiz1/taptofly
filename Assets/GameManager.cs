using UnityEngine;
using TMPro;

public enum GameState { Ready, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Ready;

    [Header("UI Refs")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;

    [Header("Gameplay")]
    public PlayerController player;

    private int score = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Application.targetFrameRate = 120;
    }

    void Start() => SetUI();

    public void StartGame()
    {
        score = 0;
        UpdateScoreUI();
        State = GameState.Playing;
        player.Begin();
        ClearPipes();     // varsa borularý temizler (yoksa sorun deðil)
        SetUI();
    }

    public void GameOver()
    {
        State = GameState.GameOver;
        SetUI();
        // Reklamý sonra ekleyeceðiz
    }

    public void Retry() => StartGame();

    // ?? PlayerController burayý çaðýrýyor
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = score.ToString();
    }

    private void SetUI()
    {
        if (startPanel) startPanel.SetActive(State == GameState.Ready);
        if (gameOverPanel) gameOverPanel.SetActive(State == GameState.GameOver);
    }

    private void ClearPipes()
    {
        var root = GameObject.Find("PipesRoot");
        if (!root) return;
        for (int i = root.transform.childCount - 1; i >= 0; i--)
            Destroy(root.transform.GetChild(i).gameObject);
    }
}
