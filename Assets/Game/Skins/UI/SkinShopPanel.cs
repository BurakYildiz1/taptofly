using UnityEngine;

public class SkinShopPanel : MonoBehaviour
{
    [SerializeField] SkinDatabase database;
    [SerializeField] Transform gridParent;
    [SerializeField] SkinCard cardPrefab;

    void OnEnable()
    {
        Build();
    }

    void Build()
    {
        foreach (Transform c in gridParent) Destroy(c.gameObject);
        foreach (var def in database.All)
        {
            var card = Instantiate(cardPrefab, gridParent);
            card.Bind(def);
        }
    }

    // UI Button
    public void OnRestore()
    {
        IapManager.Instance.RestorePurchases();
    }
}
