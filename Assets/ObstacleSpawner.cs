using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    // referanslar�n� ve objelerini ekle
    public void ResetAll()
    {
        // sahnedeki aktif engelleri temizle
        // pool varsa pool�u resetle
        foreach (Transform t in transform)
            Destroy(t.gameObject);
    }
}
