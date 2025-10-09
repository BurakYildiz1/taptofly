using UnityEngine;
using TMPro;

public enum GameState { Ready, WaitingTap, Playing, GameOver, MainMenu, Paused }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameState State { get; private set; } = GameState.Ready;

    [Header("UI Refs")]
    public GameObject startPanel;        // = MainMenu
    public GameObject gameOverPanel;
    public GameObject pausePanel;        // NEW
    public GameObject hudPanel;          // NEW (ScoreText burada duruyorsa bunu atayabilirsin)
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestTextTop;  // sol üst “Best: X” yazýsý

    [Header("Gameplay")]
    public PlayerController player;
    public Transform pipesRoot;
    [SerializeField] private PipeSpawner spawner;

    [Header("SFX")]
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip levelUpClip;
    [SerializeField, Range(0f, 1f)] float levelUpVolume = 1f;
    [SerializeField] AudioClip hitClip;

    [Header("Options")]
    [SerializeField] bool freezeTimeOnPause = true; // NEW

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
        Time.timeScale = 1f;
        UpdateScoreUI();
        SetUI();
    }

    void Start() => SetUI();

    // ---------- Public UI API ----------
    public void StartGame()
    {
        score = 0;
        ClearPipes();
        UpdateScoreUI();

        if (Difficulty.Instance) Difficulty.Instance.ResetDiff();

        State = GameState.WaitingTap;
        SetUI();

        if (player) player.Begin();
    }

    public void ActivatePlay()
    {
        if (State != GameState.WaitingTap) return;
        State = GameState.Playing;
        ResumeTime();
        ShowHUDOnly();
    }

    public void PauseGame()
    {
        if (State != GameState.Playing) return;
        State = GameState.Paused;
        if (pausePanel) pausePanel.SetActive(true);
        if (hudPanel) hudPanel.SetActive(false);
        if (freezeTimeOnPause) Time.timeScale = 0f;
        var shake = Camera.main ? Camera.main.GetComponent<CameraShake2D>() : null;
        if (shake) shake.StopShakeAndReset();
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        State = GameState.Playing;
        if (pausePanel) pausePanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(true);
        ResumeTime();
        var shake = Camera.main ? Camera.main.GetComponent<CameraShake2D>() : null;
        if (shake) shake.StopShakeAndReset();
    }

    public void Retry()
    {
        // hýzlý reset (scene reload yok)
        score = 0;
        UpdateScoreUI();

        ClearPipes();
        if (player) player.ResetPlayer();

        if (Difficulty.Instance) Difficulty.Instance.ResetDiff();

        State = GameState.WaitingTap;
        SetUI();
        var shake = Camera.main ? Camera.main.GetComponent<CameraShake2D>() : null;
        if (shake) shake.StopShakeAndReset();
    }

    public void ReturnToMenu()
    {
        score = 0;
        UpdateScoreUI();

        ClearPipes();
        if (player) player.ResetPlayer();

        if (Difficulty.Instance) Difficulty.Instance.ResetDiff();

        State = GameState.Ready;
        SetUI();
        ResumeTime();
    }

    // ---------- Game flow ----------
    public void GameOver()
    {
        State = GameState.GameOver;

        var shake = Camera.main ? Camera.main.GetComponent<CameraShake2D>() : null;
        if (shake) shake.Shake(0.35f, 0.25f);
        if (sfxSource && hitClip) sfxSource.PlayOneShot(hitClip, 1f);

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("best", bestScore);
            PlayerPrefs.Save();
        }
        UpdateScoreUI();
        SetUI();
        StopTime(); // küçük dramatik duruþ
    }

    public void AddScore(int v = 1)
    {
        if (State != GameState.Playing) return;

        score += v;
        UpdateScoreUI();

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("best", bestScore);
            PlayerPrefs.Save();
            UpdateScoreUI();
        }

        // Seviye/artýþ & efekt
        if (score > 0 && score % 25 == 0)
        {
            if (Difficulty.Instance) Difficulty.Instance.LevelUp();
            if (sfxSource && levelUpClip) sfxSource.PlayOneShot(levelUpClip, levelUpVolume);

            var card = FindObjectOfType<LevelUpCard>(true);
            if (card)
            {
                int lvl = (Difficulty.Instance != null) ? Difficulty.Instance.CurrentLevel : (score / 25) + 1;
                card.Show(lvl);
            }

            var shake = Camera.main ? Camera.main.GetComponent<CameraShake2D>() : null;
            if (shake) shake.Shake(0.15f, 0.12f);
        }
    }

    // ---------- Helpers ----------
    void UpdateScoreUI()
    {
        if (scoreText) scoreText.text = score.ToString();
        if (bestTextTop) bestTextTop.text = $"Best: {bestScore}";
    }

    void SetUI()
    {
        bool onMenu = (State == GameState.Ready || State == GameState.MainMenu);
        if (startPanel) startPanel.SetActive(onMenu);
        if (gameOverPanel) gameOverPanel.SetActive(State == GameState.GameOver);
        if (pausePanel) pausePanel.SetActive(State == GameState.Paused);

        if (hudPanel)
            hudPanel.SetActive(State == GameState.WaitingTap || State == GameState.Playing);

        if (onMenu || State == GameState.GameOver || State == GameState.Paused)
            Time.timeScale = (State == GameState.Paused && freezeTimeOnPause) ? 0f : 1f;
    }

    void ShowHUDOnly()
    {
        if (startPanel) startPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (hudPanel) hudPanel.SetActive(true);
    }

    void ClearPipes()
    {
        var root = pipesRoot ? pipesRoot : GameObject.Find("PipesRoot")?.transform;
        if (!root) return;
        for (int i = root.childCount - 1; i >= 0; i--)
            Destroy(root.GetChild(i).gameObject);
    }

    void StopTime() => Time.timeScale = 0f;
    void ResumeTime() => Time.timeScale = 1f;
}
