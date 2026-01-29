using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeIncomePrefab: UpgradeBaseItem<UpgradeField>
{
    [SerializeField] private UIntValue powerValue;
    [SerializeField] private UIntValue incomeValue;
    
    [SerializeField] private UpgradeItems_SO upgradeItems;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private UpgradeMenuUI upgradeMenuUIParent;

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
     }

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    protected override void UpdateStateButton()
    { 
        _button.enabled = scoreValue.Value >= _data.Price;
        Debug.Log($"Проверяю хватает ли у игрока денег. {scoreValue.Value} а стоит {_data.Price} ну крч {_button.enabled}");
    }

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

    public override void OnBuyEvent()
    {
        if (scoreValue.Value < _data.Price) return;
        
        scoreValue.Value -= _data.Price;
        powerValue.Value = _data.Power;
        upgradeItems.Upgrades.First(u => u.Id == _data.Id).Buy();

        UpdateStateButton();
    }
}