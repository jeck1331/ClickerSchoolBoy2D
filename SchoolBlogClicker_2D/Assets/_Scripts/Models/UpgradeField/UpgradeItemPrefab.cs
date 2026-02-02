using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemPrefab : UpgradeBaseItem<UpgradeField>
{
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private UpgradeItems_SO upgradeItems;
    [SerializeField] private ObserverSo powerObserver;
    
    [SerializeField] private TMP_Text powerText;
    
     public override void Initialize(UpgradeField data)
     {
         _data = data;
    
         titleText.text = data.Title;
         priceText.text = data.Price.ToString();
         descriptionText.text = data.Description;
         powerText.text = data.Power.ToString();
         
         CheckSecretState();
         UpdateStateButton();
         Debug.Log($"{name}Прикрепил событие к scoreObserver");
         scoreObserver.OnValueChanged += UpdateStateButton;
         powerObserver.OnValueChanged += UpdateStateButton;
         powerObserver.OnValueChanged += CheckSecretState;
     }
    

    protected override void UpdateStateButton()
    {
        bool t = !_data.IsBought && !_data.IsSecret && scoreValue.Value >= _data.Price;
        Debug.Log($"Проверяю хватает ли у игрока денег. {scoreValue.Value} а стоит {_data.Price} ну крч {t}");
        _button.enabled = !_data.IsBought && !_data.IsSecret && scoreValue.Value >= _data.Price;
    }

    private void CheckSecretState()
    {
        if (_data.IsBought)
        {
            _button.enabled = false;
            _image.color = Color.white;
            priceText.text = _data.Price.ToString();
            powerText.text = _data.Power.ToString();
            return;
        }

        if (_data.IsSecret)
        {
            if (upgradeItems.Upgrades.Any(u => u.Id == _data.Id - 1))
            {
                var prevItem = upgradeItems.Upgrades.Single(u => u.Id == _data.Id - 1);
                if (prevItem.IsBought)
                {
                    _image.color = Color.white;
                    priceText.text = _data.Price.ToString();
                    powerText.text = _data.Power.ToString();
                    _data.Unsecret();
                }
                else
                {
                    _image.color = Color.black;
                    priceText.text = "???";
                    powerText.text = "???";
                    _button.enabled = false;
                }
            }
        }
    }

    // private void OnDestroy()
    // {
    //     Debug.Log($"{name}: Открепил события");
    //     scoreObserver.OnValueChanged -= UpdateStateButton;
    //     powerObserver.OnValueChanged -= UpdateStateButton;
    // }
    private void OnDisable()
    {
        Debug.Log($"{name}: Откерпил события");
        scoreObserver.OnValueChanged -= UpdateStateButton;
        powerObserver.OnValueChanged -= UpdateStateButton;
        powerObserver.OnValueChanged -= CheckSecretState;
    }

    public override void OnBuyEvent()
    {
        if (scoreValue.Value < _data.Price) return;
        
        scoreValue.Value -= _data.Price;
        upgradeItems.Upgrades.Single(u => u.Id == _data.Id).Buy();
        powerValue.Value = _data.Power;
    }
}
