using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemPrefab : MonoBehaviour
{
    [SerializeField] private ULongValue scoreValue;
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private ObserverSo scoreObserver;
    [SerializeField] private UpgradeItems_SO upgradeItems;
    
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text levelText;
    
    [SerializeField] private UpgradeMenuUI upgradeMenuUIParent;

    private Button _button;

    private UpgradeField _data;

     public void Initialize(UpgradeField data)
     {
         _data = data;
    
         titleText.text = data.Title;
         priceText.text = data.PriceTitle;
         descriptionText.text = data.Description;
         powerText.text = data.Power.ToString();
         levelText.text = data.CurrentLevel.ToString();
         
         UpdateStateButton();
         Debug.Log($"{name}Прикрепил событие к scoreObserver");
         scoreObserver.OnValueChanged += UpdateStateButton;
     }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void UpdateStateButton()
    { 
        _button.enabled = _data.MaxLevel != _data.CurrentLevel && scoreValue.Value >= _data.Price;
        Debug.Log($"Проверяю хватает ли у игрока денег. {scoreValue.Value} а стоит {_data.Price} ну крч {_button.enabled}");
    }

    // private void OnEnable()
    // {
    //     UpdateStateButton();
    //     Debug.Log($"{name}Прикрепил событие к scoreObserver");
    //     scoreObserver.OnValueChanged += UpdateStateButton;
    // }

    private void OnDestroy()
    {
        Debug.Log($"{name}: Открепил событие к scoreObserver");
        scoreObserver.OnValueChanged -= UpdateStateButton;
    }
    private void OnDisable()
    {
        Debug.Log($"{name}: Откерпил событие к scoreObserver");
        scoreObserver.OnValueChanged -= UpdateStateButton;
    }

    public void OnBuyEvent()
    {
        if (scoreValue.Value < _data.Price) return;
        
        scoreValue.Value -= _data.Price;
        powerValue.Value = _data.Power;
        upgradeItems.Upgrades.First(u => u.Id == _data.Id).CurrentLevel++;
        UpdateData();
    }

    private void UpdateData()
    {
        _data = upgradeItems.Upgrades.First(u => u.Id == _data.Id);
        
        titleText.text = _data.Title;
        priceText.text = _data.PriceTitle;
        descriptionText.text = _data.Description;
        powerText.text = _data.Power.ToString();
        levelText.text = _data.CurrentLevel.ToString();
    }
}
