using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public int Current { get; private set; }
    public int Best { get; private set; }

    UIController ui;

    void Awake()
    {
        Best = PlayerPrefs.GetInt("best", 0);
        ui = FindObjectOfType<UIController>();
        ui?.UpdateScore(Current);
    }

    public void Add(int amount = 1)
    {
        Current += amount;
        ui?.UpdateScore(Current);
    }

    public void ResetScore()
    {
        Current = 0;
        ui?.UpdateScore(Current);
    }

    public void CommitBest()
    {
        if (Current > Best)
        {
            Best = Current;
            PlayerPrefs.SetInt("best", Best);
            PlayerPrefs.Save();
        }
    }
}
