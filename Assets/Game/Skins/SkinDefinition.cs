using UnityEngine;

[CreateAssetMenu(menuName = "Skins/Skin Definition", fileName = "SkinDefinition")]
public class SkinDefinition : ScriptableObject
{
    [Header("Identity")]
    public string skinId;         // "skin.neon" gibi (oyun i�i ID)
    public string displayName;    // UI ad�
    public string skuId;          // IAP SKU ("skin.neon"); basic i�in bo� b�rak

    [Header("Visuals")]
    public Sprite preview;        // UI kart g�rseli
    public Material material;     // Oyuncuya uygulanacak (veya)
    public GameObject prefab;     // Alternatif: t�m prefab de�i�imi

    [Header("Flags")]
    public bool isDefault;        // Varsay�lan sahiplik
}
