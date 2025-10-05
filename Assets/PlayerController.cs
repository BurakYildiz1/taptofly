#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // Yeni sistem
#endif
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Tune")]
    public float flapForce = 7.5f;
    public float startY = 0.6f;

    Rigidbody2D rb;
    bool isAlive = false;
    bool waitingFirstTap = false;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Start()
    {
        rb.simulated = false;
        rb.gravityScale = 0f;
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;

        bool tapped = false;

        // --- Yeni Input System (varsa)
#if ENABLE_INPUT_SYSTEM
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) tapped = true;
        if (!tapped && Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) tapped = true;
        if (!tapped && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) tapped = true;
#endif

        // --- Eski Input System (fallback) - TouchPhase'i fully-qualified kullan
        if (!tapped)
        {
            tapped = Input.GetMouseButtonDown(0)
                     || (Input.touchCount > 0 && Input.GetTouch(0).phase == UnityEngine.TouchPhase.Began)
                     || Input.GetKeyDown(KeyCode.Space);
        }

        if (!tapped) return;

        if (waitingFirstTap)
        {
            waitingFirstTap = false;
            rb.gravityScale = 3.0f;
        }

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
    }

    public void Begin()
    {
        isAlive = true;
        waitingFirstTap = true;

        rb.simulated = true;
        rb.gravityScale = 0f;   // ilk tap’e kadar düşmesin
        rb.linearVelocity = Vector2.zero;

        // tam ortadan başla
        transform.position = new Vector3(0f, startY, 0f);

        // yana kaymayı tamamen kilitle
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ScoreGate"))
            GameManager.Instance.AddScore(1);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isAlive) return;
        isAlive = false;
        GameManager.Instance.GameOver();
    }

}
