using UnityEngine;
using System.Collections;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;  // içinde top & bottom parçalarý olacak
    public float yLimit = 2.0f;    // merkez oynama limiti

    Coroutine loop;

    public void StartSpawning()
    {
        StopSpawning();
        loop = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (loop != null) StopCoroutine(loop);
        loop = null;
    }

    IEnumerator SpawnLoop()
    {
        while (GameManager.Instance.State == GameState.Playing)
        {
            SpawnOne();
            yield return new WaitForSeconds(Difficulty.Instance.CurrentInterval());
        }
    }

    void SpawnOne()
    {
        float gap = Difficulty.Instance.CurrentGap();

        float centerY = Random.Range(-yLimit, yLimit);
        Vector3 pos = new Vector3(transform.position.x, centerY, 0f);

        var go = Instantiate(pipePrefab, pos, Quaternion.identity);
        var p = go.GetComponent<Pipe>();
        p.Setup(gap); // üst-alt ayrýmý burada yapýlacak
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


public class PipeMover : MonoBehaviour
{
    public float speed = 2.4f;

    void Update()
    {
        // Eðer oyun Playing deðilse, hareketi durdur
        if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive)
            return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -7.5f)
            Destroy(gameObject);
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
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        if (transform.position.x < despawnX)
            Destroy(gameObject);
    }
}