using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePairPrefab;
    public float spawnInterval = 1.35f;
    public float moveSpeed = 2.4f;
    public float minY = -1.2f;
    public float maxY = 1.2f;

    float timer;

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;

        // Zorluk sistemi varsa interval ve speed'i oradan al
        float interval = Difficulty.Instance ? Difficulty.Instance.CurrentInterval() : spawnInterval;
        float speedNow = Difficulty.Instance ? Difficulty.Instance.CurrentSpeed() : moveSpeed;

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            float y = Random.Range(minY, maxY);
            GameObject go = Instantiate(pipePairPrefab, new Vector3(4.8f, y, 0f), Quaternion.identity, transform);
            go.AddComponent<PipeMover>().speed = speedNow;
        }
    }
}

public class PipeMover : MonoBehaviour
{
    public float speed = 2.4f;

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
            return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -7.5f)
            Destroy(gameObject);
    }
}


public class PipeSkinManager : MonoBehaviour
{
    public SpriteRenderer[] pipeParts;
    public Sprite[] skins;
    int skinIndex;

    void Start()
    {
        skinIndex = PlayerPrefs.GetInt("pipeSkin", 0);
        foreach (var part in pipeParts)
            part.sprite = skins[skinIndex];
    }
}

public class Pipe : MonoBehaviour
{
    public Transform topPart;
    public Transform bottomPart;
    public float moveSpeed = 2.6f;
    public float despawnX = -10f;

    public void Setup(float gap)
    {
        // pivot merkezdeyse:
        topPart.localPosition = new Vector3(0, gap * 0.5f, 0);
        bottomPart.localPosition = new Vector3(0, -gap * 0.5f, 0);
    }

    void Update()
    {
        if (GameManager.Instance.State != GameState.Playing) return;

        float speed = Difficulty.Instance.CurrentSpeed();
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < despawnX)
            Destroy(gameObject);
    }
}