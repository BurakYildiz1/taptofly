using UnityEngine;
using TMPro;

public class LevelUpCard : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] CanvasGroup cg;
    [SerializeField] RectTransform card;
    [SerializeField] TextMeshProUGUI label;

    [Header("Timings")]
    [SerializeField] float fadeIn = 0.20f;
    [SerializeField] float hold = 1.00f;
    [SerializeField] float fadeOut = 0.40f;

    [Header("Scale")]
    [SerializeField] float scaleFrom = 0.85f;
    [SerializeField] float scaleTo = 1.00f;

    void Awake()
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        if (!card) card = GetComponent<RectTransform>();
        if (!label) label = GetComponentInChildren<TextMeshProUGUI>(true);
        if (!label)
        {
            // Label yoksa otomatik oluþtur
            var go = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(transform, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(24, 16); rt.offsetMax = new Vector2(-24, -16);
            label = go.GetComponent<TextMeshProUGUI>();
            label.alignment = TextAlignmentOptions.Center;
            label.fontSize = 44;
            label.color = new Color32(233, 251, 255, 255);
            label.raycastTarget = false;
        }

        // Týklamayý asla bloklama
        cg.blocksRaycasts = false;
        cg.interactable = false;

        HideImmediate(); // <<< baþta tamamen gizle + deactive
    }

    public void HideImmediate()
    {
        cg.alpha = 0f;
        if (card) card.localScale = Vector3.one * scaleFrom;
        gameObject.SetActive(false); // <<< paneli tamamen kapat
    }

    public void Show(int level, string prefix = "ENTERING SECTOR ")
    {
        // her çaðrýda aktif et ve görünür ayarlarý yap
        gameObject.SetActive(true);
        transform.SetAsLastSibling();           // kartý canvasta en üste al
        if (label) label.transform.SetAsLastSibling(); // label da kart içinde en üstte
        StopAllCoroutines();

        if (label)
        {
            label.text = $"{prefix}{level}";
            var c = label.color; c.a = 1f; label.color = c;
        }

        StartCoroutine(Animate());
    }

    System.Collections.IEnumerator Animate()
    {
        // alpha=0'dan baþla
        cg.alpha = 0f;
        if (card) card.localScale = Vector3.one * scaleFrom;

        // Fade In + Scale In
        float t = 0f;
        while (t < fadeIn)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / fadeIn);
            cg.alpha = k;
            if (card) card.localScale = Vector3.one * Mathf.Lerp(scaleFrom, scaleTo, k);
            yield return null;
        }
        cg.alpha = 1f; if (card) card.localScale = Vector3.one * scaleTo;

        // Hold
        yield return new WaitForSeconds(hold);

        // Fade Out
        t = 0f;
        while (t < fadeOut)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / fadeOut);
            cg.alpha = Mathf.Lerp(1f, 0f, k);
            yield return null;
        }

        HideImmediate(); // <<< animasyon bitince tamamen kapat
    }
}
