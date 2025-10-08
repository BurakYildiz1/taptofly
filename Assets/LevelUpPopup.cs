using UnityEngine;
using TMPro;

public class LevelUpPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] float showTime = 0.6f;
    [SerializeField] float scaleFrom = 0.6f;
    [SerializeField] float scaleTo = 1.0f;

    CanvasGroup cg;
    void Awake()
    {
        if (!label) label = GetComponent<TextMeshProUGUI>();
        cg = gameObject.GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        transform.localScale = Vector3.one * scaleFrom;
    }

    public void Show(int level)
    {
        StopAllCoroutines();

        label.text = $"ENTERING SECTOR {level}";

        StartCoroutine(Animate());
    }


    System.Collections.IEnumerator Animate()
    {
        float t = 0f;
        cg.alpha = 1f;
        transform.localScale = Vector3.one * scaleFrom;

        // scale-in sýrasýnda renk geçiþi:
        var startColor = Color.cyan;
        var endColor = Color.white;
        label.color = startColor;

        // scale-in
        while (t < showTime * 0.45f)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / (showTime * 0.45f));
            transform.localScale = Vector3.one * Mathf.Lerp(scaleFrom, scaleTo, k);
            label.color = Color.Lerp(startColor, endColor, k);
            yield return null;
        }
        // hold + fade out
        float remain = showTime - t;
        float f = 0f;
        while (f < remain)
        {
            f += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, f / remain);
            yield return null;
        }
        cg.alpha = 0f;
    }
}
