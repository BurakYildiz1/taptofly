using System;
using System.Linq;
using UnityEngine;

public class SkinService : MonoBehaviour
{
    public static SkinService Instance { get; private set; }
    const string PP_CURRENT = "current_skin";
    const string PP_OWN_PREFIX = "own_";

    [SerializeField] SkinDatabase database;
    [SerializeField] string defaultSkinId = "skin.basic";

    public event Action<SkinDefinition> OnSkinChanged;
    public SkinDefinition Current { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        // Varsayýlan sahiplikleri iþaretle
        foreach (var s in database.All.Where(x => x.isDefault))
            PlayerPrefs.SetInt(PP_OWN_PREFIX + s.skinId, 1);

        var saved = PlayerPrefs.GetString(PP_CURRENT, defaultSkinId);
        var def = database.GetById(saved) ?? database.GetById(defaultSkinId);
        Apply(def);
    }

    public bool Owns(SkinDefinition def)
    {
        if (def.isDefault) return true;
        if (string.IsNullOrEmpty(def.skuId)) return true;
        // IAP kaydý varsa onu, yoksa PlayerPrefs’i referans al
        if (IapManager.Instance && IapManager.Instance.IsInitialized)
            return IapManager.Instance.Owns(def.skuId) || PlayerPrefs.GetInt(PP_OWN_PREFIX + def.skinId, 0) == 1;

        return PlayerPrefs.GetInt(PP_OWN_PREFIX + def.skinId, 0) == 1;
    }

    public void MarkOwned(SkinDefinition def)
    {
        PlayerPrefs.SetInt(PP_OWN_PREFIX + def.skinId, 1);
        PlayerPrefs.Save();
    }

    public void Apply(SkinDefinition def)
    {
        Current = def;
        PlayerPrefs.SetString(PP_CURRENT, def.skinId);
        PlayerPrefs.Save();
        OnSkinChanged?.Invoke(def);
    }
}
