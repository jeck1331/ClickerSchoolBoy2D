using _Scripts.Core;
using _Scripts.Models.Upgrade;
using _Scripts.Models.Enums;
using TMPro;
using UnityEngine;

namespace _Scripts.Prefabs
{
    public class UpgradeClickPrefab : UpgradeBaseItem<UpgradeClickField, UpgradeManager>
    {
        [SerializeField] private UIntValue powerValue;
        [SerializeField] private ObserverSO powerObserver;

        [SerializeField] private TMP_Text powerText;

        public override void Initialize(UpgradeClickField data, UpgradeStateItem stateItem, UpgradeStateItem previousStateItem, UpgradeManager upgradeManager)
        {
            _data = data;
            state = stateItem;
            previousState = previousStateItem;
            manager = upgradeManager;

            RefreshPurchaseState();
            scoreObserver.OnValueChanged += UpdateStateButton;
            powerObserver.OnValueChanged += RefreshPurchaseState;
            SettingsService.LanguageChanged += RefreshPurchaseState;
        }


        protected override void UpdateStateButton()
        {
            _button.interactable = UpgradeEconomyService.CanBuyClickUpgrade(_data, state, scoreValue.Value);
        }

        private void RefreshPurchaseState()
        {
            if (state.IsSecret)
                UpgradeEconomyService.TryUnlockByPreviousId(state, previousState);

            UpdateStateTexts();
            UpdateStateButton();
        }

        private void UpdateStateTexts()
        {
            bool isSecret = state.IsSecret;
            var localized = GameText.Upgrade(_data.Id, false);
            titleText.text = localized?.Title ?? _data.Title;
            priceText.text = isSecret ? GameText.Choose("Locked", "Закрыто") : GameText.Buy(_data.Price.ToString());
            int current = powerValue.Value;
            int next = _data.CalcType switch
            {
                ClickCalcType.Add => current + _data.Power,
                ClickCalcType.Multiply => current * _data.Power,
                _ => CalcCacheHelper.StartingPower + _data.Power
            };
            powerText.text = isSecret ? string.Empty
                : $"{GameText.Number(UIntValue.ToCoins(current))} → {GameText.Number(UIntValue.ToCoins(next))}{GameText.Choose(" / tap", " / клик")}";
            descriptionText.text = isSecret
                ? GameText.Choose("Buy the previous upgrade", "Купи предыдущее улучшение")
                : localized?.Description ?? _data.Description;
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
            powerObserver.OnValueChanged -= RefreshPurchaseState;
            SettingsService.LanguageChanged -= RefreshPurchaseState;
        }

        public override void OnBuyEvent()
        {
            if (!UpgradeEconomyService.TryBuyClickUpgrade(_data, state, scoreValue, powerValue, manager))
                return;

            gameObject.SetActive(false);
            AudioManager.Instance?.PlaySfx(SfxType.UpgradeBuy);
        }
    }
}