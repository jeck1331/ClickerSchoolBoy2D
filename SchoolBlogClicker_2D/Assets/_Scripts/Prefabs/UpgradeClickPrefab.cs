using System.Linq;
using _Scripts.Core;
using _Scripts.Models.Upgrade;
using TMPro;
using UnityEngine;

namespace _Scripts.Prefabs
{
    public class UpgradeClickPrefab : UpgradeBaseItem<UpgradeClickField>
    {
        [SerializeField] private UIntValue powerValue;
        [SerializeField] private UpgradeClickItemSO upgradeClickItem;
        [SerializeField] private ObserverSO powerObserver;

        [SerializeField] private TMP_Text powerText;

        public override void Initialize(UpgradeClickField data)
        {
            _data = data;

            titleText.text = data.Title;
            priceText.text = data.Price.ToString();
            descriptionText.text = data.Description;
            powerText.text = data.Power.ToString();

            CheckSecretState();
            UpdateStateButton();
            // Debug.Log($"CLICK {name}Прикрепил событие к scoreObserver");
            scoreObserver.OnValueChanged += UpdateStateButton;
            powerObserver.OnValueChanged += UpdateStateButton;
            powerObserver.OnValueChanged += CheckSecretState;
        }


        protected override void UpdateStateButton()
        {
            bool t = !_data.IsBought && !_data.IsSecret && scoreValue.Value >= _data.Price;
            // Debug.Log($"CLICK Проверяю хватает ли у игрока денег. {scoreValue.Value} а стоит {_data.Price} ну крч {t}");
            _button.interactable = !_data.IsBought && !_data.IsSecret && scoreValue.Value >= _data.Price;
        }

        private void CheckSecretState()
        {
            if (_data.IsBought)
            {
                // _button.enabled = false;
                _button.interactable = false;
                UpdateStateTexts();
                return;
            }

            //If the item is secret, check if the previous item has been with IsBought state
            if (_data.IsSecret)
            {
                var prevItem = upgradeClickItem.Upgrades.SingleOrDefault(u => u.Id == _data.Id - 1);
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
            powerText.text = isSecret ? "???" : _data.Power.ToString();
            descriptionText.text = isSecret ? string.Empty : _data.Description;
        }

        // private void OnDestroy()
        // {
        //     Debug.Log($"{name}: Открепил события");
        //     scoreObserver.OnValueChanged -= UpdateStateButton;
        //     powerObserver.OnValueChanged -= UpdateStateButton;
        // }
        private void OnDisable()
        {
            // Debug.Log($"CLICK {name}: Откерпил события");
            scoreObserver.OnValueChanged -= UpdateStateButton;
            powerObserver.OnValueChanged -= UpdateStateButton;
            powerObserver.OnValueChanged -= CheckSecretState;
        }

        public override void OnBuyEvent()
        {
            if (scoreValue.Value < _data.Price) return;

            scoreValue.Value -= _data.Price;
            upgradeClickItem.Upgrades.Single(u => u.Id == _data.Id).Buy();

            powerValue.Value = CalcCacheHelper.CalcPowerCache(upgradeClickItem.Upgrades.Where(x => x.IsBought).ToArray());
        }
    }
}