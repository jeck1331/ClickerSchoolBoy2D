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
            _button.interactable = UpgradeEconomyService.CanBuy(_data, scoreValue.Value);
        }

        private void CheckSecretState()
        {
            // if (_data.IsBought)
            // {
            //     // _button.enabled = false;
            //     _button.interactable = false;
            //     UpdateStateTexts();
            //     return;
            // }
            //
            // //If the item is secret, check if the previous item has been with IsBought state
            // if (_data.IsSecret)
            // {
            //     UpgradeEconomyService.TryUnlockByPreviousId(_data, upgradeClickItem.Upgrades);
            //     _button.interactable = !_data.IsSecret && !_data.IsBought;
            //     UpdateStateTexts(_data.IsSecret);
            // }
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
            if (UpgradeEconomyService.TryBuyClickUpgrade(_data, scoreValue, powerValue, upgradeClickItem))
                AudioManager.Instance?.PlaySfx(SfxType.UpgradeBuy);
        }
    }
}