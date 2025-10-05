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

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            float y = Random.Range(minY, maxY);

            // bu sat�r� timer if blo�unun ���NDE tan�mla
            GameObject go = Instantiate(pipePairPrefab, new Vector3(4.8f, y, 0f), Quaternion.identity, transform);
            go.AddComponent<PipeMover>().speed = moveSpeed; // �imdi go tan�ml�
        }
    }
}

public class PipeMover : MonoBehaviour
{
    public float speed = 2.4f;

    void Update()
    {
        // E�er oyun Playing de�ilse, hareketi durdur
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.x < -7.5f)
            Destroy(gameObject);
    }
}
