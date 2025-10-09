using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using static NUnit.Framework.Internal.OSPlatform;

public class IapManager : MonoBehaviour, IStoreListener
{
    public static IapManager Instance { get; private set; }

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensions;

    // SKU'lar (Play Console ile birebir ayn� olmal�)
    public const string SKU_SKIN_DRAGON = "skin.dragon";
    public const string SKU_SKIN_NEON = "skin.neon";
    public const string SKU_SKIN_GOLD_BUNDLE = "skin.goldbundle";

    // Sahiplik anahtarlar� (yerel kay�t)
    const string PP_SKIN_DRAGON = "own_skin.dragon";
    const string PP_SKIN_NEON = "own_skin.neon";
    const string PP_SKIN_GOLD = "own_skin.goldbundle";

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    void Start() => InitializeIAP();

    public bool IsInitialized => storeController != null && storeExtensions != null;

    public void InitializeIAP()
    {
        if (IsInitialized) return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Non-consumable �r�nler
        builder.AddProduct(SKU_SKIN_DRAGON, ProductType.NonConsumable);
        builder.AddProduct(SKU_SKIN_NEON, ProductType.NonConsumable);
        builder.AddProduct(SKU_SKIN_GOLD_BUNDLE, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    // --- UI'dan �a��r: fiyat g�sterimi i�in
    public string GetLocalizedPrice(string sku)
    {
        if (!IsInitialized) return "";
        var p = storeController.products.WithID(sku);
        return p != null && p.hasReceipt || p != null ? p.metadata.localizedPriceString : "";
    }

    // --- UI'dan �a��r: sat�n alma
    public void Buy(string sku)
    {
        if (!IsInitialized) { Debug.LogWarning("IAP not initialized"); return; }
        var product = storeController.products.WithID(sku);
        if (product == null || !product.availableToPurchase)
        {
            Debug.LogWarning($"Product {sku} not available");
            return;
        }
        storeController.InitiatePurchase(product);
    }

    // --- UI'dan �a��r: sahiplik kontrol (yerel)
    public bool Owns(string sku)
    {
        return PlayerPrefs.GetInt("own_" + sku, 0) == 1;
    }

    // --- UI'dan �a��r: manuel restore (Android�de �art de�il ama iyi pratik)
    public void RestorePurchases()
    {
#if UNITY_ANDROID
        // Android�de Google otomatik restore eder; yine de initialize sonras� sahiplikleri senkronlamak iyi.
        SyncOwnershipFromReceipts();
#elif UNITY_IOS
        var apple = storeExtensions.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions(result => Debug.Log("Restore result: " + result));
#endif
    }

    // IStoreListener
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        storeExtensions = extensions;

        // Uygulama a��l���nda sahiplikleri kvk et (�zellikle yeniden y�kleme/cihaz de�i�imi i�in)
        SyncOwnershipFromReceipts();
    }

    public void OnInitializeFailed(InitializationFailureReason error) =>
        Debug.LogError("IAP init failed: " + error);

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        var sku = args.purchasedProduct.definition.id;

        // (Opsiyonel) receipt do�rulama � minimal yerel do�rulama veya server-side token check �nerilir
        GrantEntitlement(sku);

        // Non-consumable: sat�n alma tamamland� bilgisini hemen d�nelim
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogWarning($"Purchase failed: {product.definition.id} => {reason}");
        // UI'ya hata mesaj� g�ster
    }

    private void GrantEntitlement(string sku)
    {
        PlayerPrefs.SetInt("own_" + sku, 1);
        PlayerPrefs.Save();

        // SkinService'e bildir (SKU SkinDefinition e�leme)
        var db = FindObjectOfType<SkinService>();
        if (db)
        {
            // skuId ile e�le
            var def = db.GetComponent<SkinService>()?.GetType()
                .GetField("database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .GetValue(db) as SkinDatabase;

            if (def != null)
            {
                foreach (var s in def.All)
                    if (s.skuId == sku) { db.MarkOwned(s); break; }
            }
        }

        Debug.Log($"Entitlement granted: {sku}");
    }

    public void MarkOwnedBySku(string sku)
    {
        var def = database.All.FirstOrDefault(x => x.skuId == sku);
        if (def != null) MarkOwned(def);
    }

    private void SyncOwnershipFromReceipts()
    {
        if (!IsInitialized) return;
        foreach (var p in storeController.products.all)
        {
            if (p != null && p.hasReceipt && p.definition.type == ProductType.NonConsumable)
            {
                GrantEntitlement(p.definition.id);
            }
        }
    }
}
