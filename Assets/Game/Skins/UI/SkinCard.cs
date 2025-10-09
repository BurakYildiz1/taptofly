using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinCard : MonoBehaviour
{
    [SerializeField] Image previewImg;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI priceText;
    [SerializeField] Button primaryButton;   // Satýn Al / Kullan
    [SerializeField] TextMeshProUGUI primaryBtnLabel;
    [SerializeField] GameObject lockIcon;

    SkinDefinition def;

    public void Bind(SkinDefinition def)
    {
        this.def = def;
        titleText.text = def.displayName;
        previewImg.sprite = def.preview;

        Refresh();
    }

    void OnEnable()
    {
        if (SkinService.Instance != null)
            SkinService.Instance.OnSkinChanged += OnSkinChanged;
    }
    void OnDisable()
    {
        if (SkinService.Instance != null)
            SkinService.Instance.OnSkinChanged -= OnSkinChanged;
    }

    void OnSkinChanged(SkinDefinition _) => Refresh();

    void Refresh()
    {
        bool owns = SkinService.Instance.Owns(def);
        bool isCurrent = SkinService.Instance.Current == def;

        lockIcon.SetActive(!owns);

        if (!owns)
        {
            primaryBtnLabel.text = "Satýn Al";
            priceText.text = GetPrice(def);
            primaryButton.onClick.RemoveAllListeners();
            primaryButton.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(def.skuId))
                    IapManager.Instance.Buy(def.skuId);
            });
        }
        else
        {
            priceText.text = isCurrent ? "Seçili" : "";
            primaryBtnLabel.text = isCurrent ? "Kullanýlýyor" : "Kullan";
            primaryButton.onClick.RemoveAllListeners();
            primaryButton.onClick.AddListener(() =>
            {
                if (!isCurrent)
                    SkinService.Instance.Apply(def);
            });
        }
    }

    string GetPrice(SkinDefinition d)
    {
        if (string.IsNullOrEmpty(d.skuId)) return "";
        if (IapManager.Instance && IapManager.Instance.IsInitialized)
            return IapManager.Instance.GetLocalizedPrice(d.skuId);
        return "—";
    }
}
