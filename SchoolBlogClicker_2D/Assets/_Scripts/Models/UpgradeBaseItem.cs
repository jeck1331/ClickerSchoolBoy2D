using TMPro;
using UnityEngine;
using UnityEngine.UI;
public abstract class UpgradeBaseItem<TData>: MonoBehaviour where TData : class
{
    [SerializeField] protected ULongValue scoreValue;
    [SerializeField] protected ObserverSo scoreObserver;
    
    [SerializeField] protected TMP_Text titleText;
    [SerializeField] protected TMP_Text priceText;
    [SerializeField] protected TMP_Text descriptionText;

    protected Button _button;
    protected Image _image;

    protected TData _data;

    public abstract void Initialize(TData data);

    private void Awake()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
    }

    protected abstract void UpdateStateButton();

    public abstract void OnBuyEvent();
}