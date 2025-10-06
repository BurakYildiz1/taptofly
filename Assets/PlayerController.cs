#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        if (GameManager.Instance == null) return;

        // Tap algısı (yeni + eski input)
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

        // İlk tap: WaitingTap'ten Playing'e geçiş
        if (GameManager.Instance.State == GameState.WaitingTap && tapped)
        {
            waitingFirstTap = false;
            rb.gravityScale = 3f;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);

            GameManager.Instance.ActivatePlay(); // <<< oyun şimdi gerçekten başlar
            return;
        }

        // Normal oyun sırasında zıplama
        if (GameManager.Instance.State == GameState.Playing && tapped)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
        }
    }

    public void Begin()
    {
        isAlive = true;
        waitingFirstTap = true;

        rb.simulated = true;
        rb.gravityScale = 0f;           // ilk tap’e kadar düşmesin
        rb.linearVelocity = Vector2.zero;

        // ORTA BAŞLAT
        transform.position = new Vector3(0f, 0f, 0f);

        // X eksenini kilitle (yana kaymasın)
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isAlive) return;
        isAlive = false;
        GameManager.Instance.GameOver();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ScoreGate"))
            GameManager.Instance.AddScore(1);
    }
    public class PlayerSkinManager : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public Sprite[] skins;
        int currentSkinIndex = 0;

        public void SetSkin(int index)
        {
            currentSkinIndex = Mathf.Clamp(index, 0, skins.Length - 1);
            spriteRenderer.sprite = skins[currentSkinIndex];
            PlayerPrefs.SetInt("playerSkin", currentSkinIndex);
        }

        void Start()
        {
            SetSkin(PlayerPrefs.GetInt("playerSkin", 0));
        }
    }

}
