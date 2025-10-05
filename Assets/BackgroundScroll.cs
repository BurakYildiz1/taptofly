using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public Transform pieceA;   // BackgroundA
    public Transform pieceB;   // BackgroundB
    public float speed = 0.4f;

    float width;

    void Start()
    {
        width = GetWorldWidth(pieceA);
        // B'yi A'n�n sa��na garanti hizala
        pieceB.position = pieceA.position + Vector3.right * width;
        // debug g�rmek istersen:
        // Debug.Log($"BG width = {width}");
    }

    float GetWorldWidth(Transform t)
    {
        var sr = t.GetComponent<SpriteRenderer>();
        if (sr != null) return sr.bounds.size.x;

        var rt = t as RectTransform; // UI k�kenliyse de hesapla
        if (rt != null)
        {
            Vector3[] c = new Vector3[4];
            rt.GetWorldCorners(c);
            return (c[2] - c[0]).x;
        }

        return 10f;
    }

    void Update()
    {
        // Oyun Playing de�ilken dur
        if (GameManager.Instance != null && GameManager.Instance.State != GameState.Playing)
            return;

        Vector3 d = Vector3.left * speed * Time.deltaTime;
        pieceA.position += d;
        pieceB.position += d;

        if (pieceA.position.x <= -width) pieceA.position += Vector3.right * (width * 2f);
        if (pieceB.position.x <= -width) pieceB.position += Vector3.right * (width * 2f);
    }
}
