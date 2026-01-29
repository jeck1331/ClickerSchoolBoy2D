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
         
         UpdateStateButton();
         Debug.Log($"{name}Прикрепил событие к scoreObserver");
         scoreObserver.OnValueChanged += UpdateStateButton;
         powerObserver.OnValueChanged += UpdateStateButton;
     }
    

    protected override void UpdateStateButton()
    {
        if (_data.IsSecret || _data.IsBought)
        {
            _button.enabled = false;
            if (_data.IsSecret && !_data.IsBought)
            {
                var upgItem = _data.Id > 0 ? upgradeItems.Upgrades.First(x => _data.IsBought && x.Id == _data.Id - 1) : null;
                if (upgItem != null && upgItem.IsBought) _data.Unsecret();
            }
            if (_data.IsSecret)
            {
                _image.color = Color.black;
                priceText.text = "???";
                powerText.text = "???";
            }
            else
            {
                _image.color = Color.white;
                priceText.text = _data.Price.ToString();
                powerText.text = _data.Power.ToString();
            }
        } else if (scoreValue.Value >= _data.Price)
        {
            _button.enabled = true;
        }
        Debug.Log($"Проверяю хватает ли у игрока денег. {scoreValue.Value} а стоит {_data.Price} ну крч {_button.enabled}");
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
    }

    public override void OnBuyEvent()
    {
        if (scoreValue.Value < _data.Price) return;
        
        scoreValue.Value -= _data.Price;
        powerValue.Value = _data.Power;
        upgradeItems.Upgrades.First(u => u.Id == _data.Id).Buy();

        UpdateStateButton();
    }
}
