using UnityEngine;
using TMPro;

public enum GameState { Ready, WaitingTap, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Ready;

    [Header("UI Refs")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestTextTop;   // sol üstte sürekli görünen

    [Header("Gameplay")]
    public PlayerController player;
    [SerializeField] Transform pipesRoot; // opsiyonel Inspector'dan baðla

    int score = 0;
    int bestScore;

    public bool IsGameplayActive => State == GameState.Playing;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        bestScore = PlayerPrefs.GetInt("best", 0);
        score = 0;

        State = GameState.Ready;
        UpdateScoreUI();
        SetUI();
    }

    void Start() => SetUI();

    public void StartGame()
    {
        // Start butonuna basýldý ? sahne hazýrlanýr ama gerçek oyun "ilk tap" ile baþlar
        score = 0;
        ClearPipes();
        UpdateScoreUI();

        State = GameState.WaitingTap;     // <<< ÖNEMLÝ: pipelar henüz baþlamaz
        SetUI();

        if (player) player.Begin();       // player ekranda bekler (gravity kapalý)
    }

    // Player ilk dokunuþu algýlayýnca burayý çaðýrýr
    public void ActivatePlay()
    {
        if (State != GameState.WaitingTap) return;
        State = GameState.Playing;        // <<< Artýk spawner/scroll çalýþýr
    }

    public void GameOver()
    {
        State = GameState.GameOver;

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("best", bestScore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();                  // sol üst "Best" yazýsý anýnda güncellensin
        SetUI();
    }

    public void Retry() => StartGame();

    public void AddScore(int v = 1)
    {
        score += v;
        scoreText.text = score.ToString();

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("best", bestScore);
            bestTextTop.text = bestScore.ToString();
        }

        if (score > 0 && score % 25 == 0)
        {
            Difficulty.Instance.LevelUp();
        }
    }


    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = score.ToString();
        if (bestTextTop) bestTextTop.text = $"Best: {bestScore}";
    }

    void SetUI()
    {
        if (startPanel) startPanel.SetActive(State == GameState.Ready);
        if (gameOverPanel) gameOverPanel.SetActive(State == GameState.GameOver);
    }

    void ClearPipes()
    {
        var root = pipesRoot ? pipesRoot : GameObject.Find("PipesRoot")?.transform;
        if (!root) return;
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }
}
