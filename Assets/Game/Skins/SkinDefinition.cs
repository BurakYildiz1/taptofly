using UnityEngine;

[CreateAssetMenu(menuName = "Skins/Skin Definition", fileName = "SkinDefinition")]
public class SkinDefinition : ScriptableObject
{
    [Header("Identity")]
    public string skinId;         // "skin.neon" gibi (oyun içi ID)
    public string displayName;    // UI adý
    public string skuId;          // IAP SKU ("skin.neon"); basic için boþ býrak

    [Header("Visuals")]
    public Sprite preview;        // UI kart görseli
    public Material material;     // Oyuncuya uygulanacak (veya)
    public GameObject prefab;     // Alternatif: tüm prefab deðiþimi

    [Header("Flags")]
    public bool isDefault;        // Varsayýlan sahiplik
}
