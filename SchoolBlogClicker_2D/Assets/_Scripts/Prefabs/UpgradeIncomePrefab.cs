using _Scripts.Models.Upgrade;
using TMPro;
using UnityEngine;

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
            // Debug.Log($"INCOME {name}Прикрепил событие к scoreObserver");
            scoreObserver.OnValueChanged += UpdateStateButton;
            incomeObserver.OnValueChanged += UpdateStateButton;
            incomeObserver.OnValueChanged += CheckSecretState;
        }

        protected override void UpdateStateButton()
        {
            _button.interactable = UpgradeEconomyService.CanBuy(_data, state, scoreValue.Value);
        }
        
        private void CheckSecretState()
        {
            // if (_data.IsBought)
            // {
            //     _button.interactable = false;
            //     UpdateStateTexts();
            //     return;
            // }
            //
            // //If the item is secret, check if the previous item has been with IsBought state
            // if (_data.IsSecret)
            // {
            //     UpgradeEconomyService.TryUnlockByPreviousId(_data, upgradeIncomeItem.Upgrades);
            //     _button.interactable = !_data.IsSecret && !_data.IsBought;
            //     UpdateStateTexts(_data.IsSecret);
            // }
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
            // Debug.Log($"INCOME {name}: Откерпил событие к scoreObserver");
            scoreObserver.OnValueChanged -= UpdateStateButton;
            incomeObserver.OnValueChanged -= UpdateStateButton;
            incomeObserver.OnValueChanged -= CheckSecretState;
        }

        public override void OnBuyEvent()
        {
            if (UpgradeEconomyService.TryBuyIncomeUpgrade(_data, state, scoreValue, incomeValue, upgradeIncomeItem))
            {
                AudioManager.Instance?.PlaySfx(SfxType.UpgradeBuy);
                UpdateStateButton();
            }
        }
    }
}