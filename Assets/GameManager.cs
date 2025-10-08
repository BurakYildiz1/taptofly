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
    public Transform pipesRoot;
    [SerializeField] private PipeSpawner spawner;

    [Header("SFX")]
    [SerializeField] AudioSource sfxSource;     // Canvas / GameManager üstünde tek kaynak
    [SerializeField] AudioClip levelUpClip;     // 25'te çalýnacak
    [SerializeField, Range(0f, 1f)] float levelUpVolume = 1f;
    [SerializeField] AudioClip hitClip;


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
        score = 0;
        ClearPipes();
        UpdateScoreUI();

        // <<< ÖNEMLÝ: her run baþýnda level sýfýrlansýn
        if (Difficulty.Instance) Difficulty.Instance.ResetDiff();

        State = GameState.WaitingTap;
        SetUI();

        if (player) player.Begin();
    }

    public void ActivatePlay()
    {
        if (State != GameState.WaitingTap) return;
        State = GameState.Playing;        // <<< Artýk spawner/scroll çalýþýr
    }

    public void GameOver()
    {
        State = GameState.GameOver;
        var shake = Camera.main ? Camera.main.GetComponent<CameraShake2D>() : null;
        if (shake) shake.Shake(0.35f, 0.25f);
        if (sfxSource && hitClip) sfxSource.PlayOneShot(hitClip, 1f);
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
        if (State != GameState.Playing) return;

        score += v;

        if (scoreText != null)
            scoreText.text = score.ToString();

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("best", bestScore);
            if (bestTextTop != null)
                bestTextTop.text = bestScore.ToString();
        }

        // 25'te seviye artýr (Difficulty varsa)
        if (score > 0 && score % 25 == 0 && Difficulty.Instance != null)
            Difficulty.Instance.LevelUp();
        if (score > 0 && score % 5 == 0)
        {

            if (Difficulty.Instance) Difficulty.Instance.LevelUp();
            if (sfxSource && levelUpClip) sfxSource.PlayOneShot(levelUpClip, levelUpVolume);
            // Card popup
            var card = FindObjectOfType<LevelUpCard>(true);
            if (card)
            {
                int lvl = (Difficulty.Instance != null) ? Difficulty.Instance.CurrentLevel : (score / 25) + 1;
                card.Show(lvl);
            }
            // Shake (aþaðýdaki script)
            var shake = Camera.main ? Camera.main.GetComponent<CameraShake2D>() : null;
            if (shake) shake.Shake(0.15f, 0.12f);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = score.ToString();
        if (bestTextTop != null) bestTextTop.text = $"Best: {bestScore}";
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
