using UnityEngine;

public class SkinApplier : MonoBehaviour
{
    [SerializeField] Renderer targetRenderer; // SpriteRenderer/SkinnedMeshRenderer olabilir
    [SerializeField] GameObject replaceRoot;  // Prefab’la komple deðiþim istiyorsan

    void OnEnable()
    {
        SkinService.Instance.OnSkinChanged += Apply;
        Apply(SkinService.Instance.Current);
    }

    void OnDisable()
    {
        if (SkinService.Instance != null)
            SkinService.Instance.OnSkinChanged -= Apply;
    }

    void Apply(SkinDefinition def)
    {
        if (def.material && targetRenderer)
        {
            targetRenderer.sharedMaterial = def.material;
        }

        if (def.prefab && replaceRoot)
        {
            foreach (Transform c in replaceRoot.transform) Destroy(c.gameObject);
            Instantiate(def.prefab, replaceRoot.transform);
        }
    }
}
