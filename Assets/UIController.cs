using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    [Header("Panels")]
    public CanvasGroup mainMenu;
    public CanvasGroup hud;
    public CanvasGroup pausePanel;
    public CanvasGroup gameOver;

    [Header("HUD")]
    public TextMeshProUGUI scoreText;

    [Header("GameOver")]
    public TextMeshProUGUI goScoreText;
    public TextMeshProUGUI goBestText;

    // --- Show/Hide helpers ---
    void SetGroup(CanvasGroup cg, bool show)
    {
        if (!cg) return;
        cg.alpha = show ? 1f : 0f;
        cg.interactable = show;
        cg.blocksRaycasts = show;
        cg.gameObject.SetActive(show);
    }

    public void ShowMainMenu() => SetGroup(mainMenu, true);
    public void HideMainMenu() => SetGroup(mainMenu, false);

    public void ShowHUD() => SetGroup(hud, true);
    public void HideHUD() => SetGroup(hud, false);

    public void ShowPause() => SetGroup(pausePanel, true);
    public void HidePause() => SetGroup(pausePanel, false);

    public void ShowGameOver(int score, int best)
    {
        if (goScoreText) goScoreText.text = score.ToString();
        if (goBestText) goBestText.text = best.ToString();
        SetGroup(gameOver, true);
    }
    public void HideGameOver() => SetGroup(gameOver, false);

    // HUD score update
    public void UpdateScore(int value)
    {
        if (scoreText) scoreText.text = value.ToString();
    }

    // --- Button Events (Inspector’dan baðla) ---
    public void BtnPlay() => GameManager.Instance.StartGame();
    public void BtnPause() => GameManager.Instance.PauseGame();
    public void BtnResume() => GameManager.Instance.ResumeGame();
    public void BtnRetry() => GameManager.Instance.Retry();
    public void BtnMenu() => GameManager.Instance.ReturnToMenu();

    // Reklamlý “Continue” gibi butonlar için hook:
    public void BtnContinueByAd()
    {
        // AdManager.Instance.ShowRewarded(() => { GameManager.Instance.ResumeFromCheckpoint(); });
        // Þimdilik boþ býrakýyoruz; AdMob aþamasýnda dolduracaðýz.
    }
}
