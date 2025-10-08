using UnityEngine;

public class Difficulty : MonoBehaviour
{
    public static Difficulty Instance { get; private set; }
    void Awake() => Instance = this;

    [Header("Spawn")]
    public float baseInterval = 1.6f;
    public float intervalStep = 0.10f;
    public float minInterval = 0.9f;

    [Header("Gap")]
    public float baseGap = 3.1f;
    public float gapStep = 0.18f;
    public float minGap = 1.6f;

    [Header("Scroll Speed")]                 // <<< YENİ
    public float baseSpeed = 2.6f;
    public float speedStep = 0.25f;          // her level artışı
    public float maxSpeed = 5.0f;

    int level = 0;

    public void ResetDiff() => level = 0;
    public void LevelUp() => level++;
    public int CurrentLevel => level;
    public float CurrentInterval() =>
        Mathf.Max(minInterval, baseInterval - level * intervalStep);

    public float CurrentGap() =>
        Mathf.Max(minGap, baseGap - level * gapStep);

    public float CurrentSpeed() =>           // <<< YENİ
        Mathf.Min(maxSpeed, baseSpeed + level * speedStep);

}
