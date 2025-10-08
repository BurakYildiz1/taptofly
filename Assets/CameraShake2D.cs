using UnityEngine;

public class CameraShake2D : MonoBehaviour
{
    Vector3 originalPos;

    void Awake() => originalPos = transform.localPosition;

    public void Shake(float intensity = 0.2f, float duration = 0.15f)
    {
        StopAllCoroutines();
        StartCoroutine(DoShake(intensity, duration));
    }

    System.Collections.IEnumerator DoShake(float intensity, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float damper = 1f - (t / duration);          // yavaþça sönsün
            Vector2 offset = Random.insideUnitCircle * intensity * damper;
            transform.localPosition = originalPos + (Vector3)offset;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}
