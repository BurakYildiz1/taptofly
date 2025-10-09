using UnityEngine;
using System.Collections;

public class CameraShake2D : MonoBehaviour
{
    [SerializeField] Transform target;   // Boþ býrakýlýrsa kendi transform’u kullanýr
    Vector3 _originalPos;
    Coroutine _running;

    void Awake()
    {
        if (!target) target = transform;
        _originalPos = target.localPosition;
    }

    void OnDisable()
    {
        // Olasý donuk offset’i sýfýrla
        if (target) target.localPosition = _originalPos;
        _running = null;
    }

    public void Shake(float amplitude, float duration, bool useUnscaledTime = true)
    {
        if (_running != null) StopCoroutine(_running);
        _running = StartCoroutine(DoShake(amplitude, duration, useUnscaledTime));
    }

    IEnumerator DoShake(float amp, float dur, bool unscaled)
    {
        float elapsed = 0f;

        while (elapsed < dur)
        {
            float dt = unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            elapsed += dt;

            // 2D için küçük bir rastgele sapma
            Vector2 offset2D = Random.insideUnitCircle * amp;
            target.localPosition = _originalPos + new Vector3(offset2D.x, offset2D.y, 0f);

            yield return null; // unscaled bekleyiþ gerekmez; dt zaten unscaled
        }

        // Temizle
        target.localPosition = _originalPos;
        _running = null;
    }

    public void StopShakeAndReset()
    {
        if (_running != null) StopCoroutine(_running);
        if (target) target.localPosition = _originalPos;
        _running = null;
    }
}
