#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float flapForce = 7.5f;

    public SpriteRenderer flameSprite;
    public float flameDuration = 0.12f;

    Rigidbody2D rb;
    bool isAlive = false;

    [Header("SFX")]
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioClip tapClip;
    [SerializeField, Range(0f, 1f)] float tapVolume = 0.9f;

    [Header("Tilt")]
    [SerializeField] float tiltMaxUp = 30f;
    [SerializeField] float tiltMaxDown = -60f;
    [SerializeField] float tiltLerp = 10f;
    [SerializeField] float baseAngle = 0f;

    Vector3 startPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    void Start()
    {
        rb.simulated = false;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;

        // Input
        bool tapped = false;
#if ENABLE_INPUT_SYSTEM
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) tapped = true;
        if (!tapped && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) tapped = true;
        if (!tapped && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) tapped = true;
#endif
        if (!tapped)
        {
            tapped = Input.GetMouseButtonDown(0)
                  || (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
                  || Input.GetKeyDown(KeyCode.Space);
        }

        // İlk tap → Playing
        if (GameManager.Instance.State == GameState.WaitingTap && tapped)
        {
            rb.gravityScale = 3f;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
            if (flameSprite) StartCoroutine(FlameBurst());
            if (sfxSource && tapClip) sfxSource.PlayOneShot(tapClip, tapVolume);

            GameManager.Instance.ActivatePlay();
            return;
        }

        // Oyun sırasında tap
        if (GameManager.Instance.State == GameState.Playing && tapped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
            if (flameSprite) StartCoroutine(FlameBurst());
            if (sfxSource && tapClip) sfxSource.PlayOneShot(tapClip, tapVolume);
        }

        // Paused/GameOver → input yok
        if (GameManager.Instance.State == GameState.Paused || GameManager.Instance.State == GameState.GameOver)
            return;

        // Tilt
        float targetZ = 0f;
        if (GameManager.Instance.State == GameState.Playing)
        {
            float vy = rb.linearVelocity.y;
            if (vy >= 0f)
                targetZ = Mathf.Lerp(0f, tiltMaxUp, Mathf.InverseLerp(0f, 8f, vy));
            else
                targetZ = Mathf.Lerp(0f, tiltMaxDown, Mathf.InverseLerp(0f, -8f, vy));
        }

        float currentZ = transform.eulerAngles.z;
        float desiredZ = baseAngle + targetZ;
        float z = Mathf.MoveTowardsAngle(currentZ, desiredZ, tiltLerp * 60f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, z);
    }

    public void Begin()
    {
        isAlive = true;

        rb.simulated = true;
        rb.gravityScale = 0f;      // ilk tap'a kadar düşmesin
        rb.linearVelocity = Vector2.zero;

        // ortada başlat
        transform.position = new Vector3(0f, 0f, 0f);

        // X eksenini kilitle
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    public void ResetPlayer()
    {
        isAlive = true;
        rb.simulated = true;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        transform.position = startPos;   // istersen (0,0,0) da kullanabilirsin
        gameObject.SetActive(true);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isAlive) return;
        isAlive = false;
        GameManager.Instance.GameOver();
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ScoreGate"))
            GameManager.Instance.AddScore(1);
    }

    IEnumerator FlameBurst()
    {
        if (!flameSprite) yield break;

        flameSprite.enabled = true;
        var t = 0f;
        var startScale = Vector3.one * 0.9f;
        var endScale = Vector3.one * 1.2f;
        flameSprite.transform.localScale = startScale;

        while (t < flameDuration)
        {
            t += Time.deltaTime;
            flameSprite.transform.localScale = Vector3.Lerp(startScale, endScale, t / flameDuration);
            yield return null;
        }
        flameSprite.enabled = false;
    }
}
