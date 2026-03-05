using System.Linq;
using _Scripts.Core;
using _Scripts.Models.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Prefabs
{
    public class UpgradeIncomePrefab : UpgradeBaseItem<UpgradeIncomeField>
    {
        [SerializeField] private UIntValue incomeValue;
        [SerializeField] private UpgradeIncomeItemSO upgradeIncomeItem;
        [SerializeField] private ObserverSO incomeObserver;
        
        [SerializeField] private TMP_Text incomeText;

        public override void Initialize(UpgradeIncomeField data)
        {
            _data = data;

            titleText.text = data.Title;
            priceText.text = data.Price.ToString();
            descriptionText.text = data.Description;
            incomeText.text = data.Income.ToString();

            CheckSecretState();
            UpdateStateButton();
            Debug.Log($"INCOME {name}Прикрепил событие к scoreObserver");
            scoreObserver.OnValueChanged += UpdateStateButton;
            incomeObserver.OnValueChanged += UpdateStateButton;
            incomeObserver.OnValueChanged += CheckSecretState;
        }

        protected override void UpdateStateButton()
        {
            bool t = !_data.IsBought && !_data.IsSecret && scoreValue.Value >= _data.Price;
            Debug.Log($"INCOME Проверяю хватает ли у игрока денег. {scoreValue.Value} а стоит {_data.Price} ну крч {t}");
            _button.interactable = !_data.IsBought && !_data.IsSecret && scoreValue.Value >= _data.Price;
        }
        
        private void CheckSecretState()
        {
            if (_data.IsBought)
            {
                _button.interactable = false;
                UpdateStateTexts();
                return;
            }

            //If the item is secret, check if the previous item has been with IsBought state
            if (_data.IsSecret)
            {
                var prevItem = upgradeIncomeItem.Upgrades.SingleOrDefault(u => u.Id == _data.Id - 1);
                if (prevItem is not null)
                {
                    if (prevItem.IsBought)
                    {
                        _data.Unsecret();
                        UpdateStateButton();
                    }
                    else
                    {
                        _button.interactable = false;
                    }
                    
                    UpdateStateTexts(_data.IsSecret);
                }
            }
        }
        
        private void UpdateStateTexts(bool isSecret = false)
        {
            _image.material = isSecret ? matSecretField : null;
            priceText.text = isSecret ? "???" : _data.Price.ToString();
            incomeText.text = isSecret ? "???" : _data.Income.ToString();
            descriptionText.text = isSecret ? string.Empty : _data.Description;
        }

        // private void OnDestroy()
        // {
        //     Debug.Log($"INCOME {name}: Открепил событие к scoreObserver");
        //     scoreObserver.OnValueChanged -= UpdateStateButton;
        // }

        private void OnDisable()
        {
            Debug.Log($"INCOME {name}: Откерпил событие к scoreObserver");
            scoreObserver.OnValueChanged -= UpdateStateButton;
        }

        public override void OnBuyEvent()
        {
            if (scoreValue.Value < _data.Price) return;

            scoreValue.Value -= _data.Price;
            upgradeIncomeItem.Upgrades.First(u => u.Id == _data.Id).Buy();
            incomeValue.Value = CalcCacheHelper.CalcIncomeCache(upgradeIncomeItem.Upgrades.Where(x => x.IsBought).ToArray());
            
            UpdateStateButton();
        }
    }
}