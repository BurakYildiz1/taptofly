#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float flapForce = 7.5f;
    public float startY = 0.6f;

    public SpriteRenderer flameSprite;
    public float flameDuration = 0.12f;

    Rigidbody2D rb;
    bool isAlive = false;
    bool waitingFirstTap = false;


    [Header("Tilt")]
    [SerializeField] float tiltMaxUp = 30f;     // yukarı bakış limiti
    [SerializeField] float tiltMaxDown = -60f;  // aşağı bakış limiti
    [SerializeField] float tiltLerp = 10f;      // dönüş hızı
    [SerializeField] float baseAngle = 0f;      // sprite’ın doğal duruşu (gerekirse 90/ -90 ver)

    void Awake() => rb = GetComponent<Rigidbody2D>();

    void Start()
    {
        rb.simulated = false;
        rb.gravityScale = 0f;
    }
    void Update()
    {
        if (GameManager.Instance == null) return;

        // --- Tap algısı (Yeni Input System + Eski) ---
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

        // --- İlk tap: WaitingTap -> Playing ---
        if (GameManager.Instance.State == GameState.WaitingTap && tapped)
        {
            rb.gravityScale = 3f;
            rb.linearVelocity = Vector2.zero; // linearVelocity değil!
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
            if (flameSprite) StartCoroutine(FlameBurst());

            GameManager.Instance.ActivatePlay();
            return;
        }

        // --- Oyun sırasında zıplama ---
        if (GameManager.Instance.State == GameState.Playing && tapped)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
            if (flameSprite) StartCoroutine(FlameBurst());
        }

        // --- Tilt (burun açısı) ---
        float targetZ = 0f; // WaitingTap'ta düz
        if (GameManager.Instance.State == GameState.Playing)
        {
            float vy = rb.linearVelocity.y;

            if (vy >= 0f)
                targetZ = Mathf.Lerp(0f, tiltMaxUp, Mathf.InverseLerp(0f, 8f, vy));
            else
                targetZ = Mathf.Lerp(0f, tiltMaxDown, Mathf.InverseLerp(0f, -8f, vy));
        }

        float currentZ = transform.eulerAngles.z;
        float desiredZ = baseAngle + targetZ; // sprite yönüne göre 0/90 ayarla
        float z = Mathf.MoveTowardsAngle(currentZ, desiredZ, tiltLerp * 60f * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, z);
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
    IEnumerator FlameBurst()
    {
        if (!flameSprite) yield break;

        // görünür yap + hafif scale
        flameSprite.enabled = true;
        var t = 0f;
        var startScale = Vector3.one * 0.9f;
        var endScale = Vector3.one * 1.2f;
        flameSprite.transform.localScale = startScale;

        // kısa “parlama”
        while (t < flameDuration)
        {
            t += Time.deltaTime;
            flameSprite.transform.localScale = Vector3.Lerp(startScale, endScale, t / flameDuration);
            yield return null;
        }
        flameSprite.enabled = false;
    }

}
