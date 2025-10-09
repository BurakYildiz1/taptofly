using UnityEngine;
using System.Collections;

public class UIFx : MonoBehaviour
{
    public CanvasGroup cg;
    public RectTransform rt;
    public float fade = 0.18f;
    public float pop = 0.18f;
    public float fromScale = 0.9f;

    void Reset()
    {
        cg = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(CoShow());
    }

    public void Hide()
    {
        StopAllCoroutines();
        StartCoroutine(CoHide());
    }

    IEnumerator CoShow()
    {
        if (cg) cg.alpha = 0f;
        if (rt) rt.localScale = Vector3.one * fromScale;

        float t = 0;
        while (t < fade) { t += Time.unscaledDeltaTime; if (cg) cg.alpha = Mathf.Lerp(0, 1, t / fade); yield return null; }

        t = 0;
        while (t < pop) { t += Time.unscaledDeltaTime; if (rt) rt.localScale = Vector3.Lerp(Vector3.one * fromScale, Vector3.one, t / pop); yield return null; }
    }

    IEnumerator CoHide()
    {
        float start = cg ? cg.alpha : 1f;
        float t = 0;
        while (t < fade) { t += Time.unscaledDeltaTime; if (cg) cg.alpha = Mathf.Lerp(start, 0, t / fade); yield return null; }
        gameObject.SetActive(false);
    }
}
