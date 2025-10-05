using UnityEngine;

public class Scrolling : MonoBehaviour
{
    public float speed = 2.4f;
    public float resetX = -5.7f;  // ekranýn sol dýþý
    public float startX = 5.7f;  // ekranýn sað dýþý

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing)
            return;

        transform.Translate(Vector3.left * speed * Time.deltaTime);
        if (transform.position.x <= resetX)
            transform.position = new Vector3(startX, transform.position.y, transform.position.z);
    }

}
