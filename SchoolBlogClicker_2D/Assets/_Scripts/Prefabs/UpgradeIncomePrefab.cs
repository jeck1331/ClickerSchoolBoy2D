using _Scripts.Helpers;
using _Scripts.Models.Upgrade;
using TMPro;
using UnityEngine;

namespace _Scripts.Prefabs
{
    public class UpgradeIncomePrefab : UpgradeBaseItem<UpgradeIncomeField, ShopManager>
    {
        [SerializeField] private UIntValue incomeValue;
        [SerializeField] private ObserverSO incomeObserver;

        [SerializeField] private TMP_Text incomeText;
        [SerializeField] private TMP_Text ownedText;

        public override void Initialize(UpgradeIncomeField data, UpgradeStateItem stateItem, UpgradeStateItem previousStateItem, ShopManager shopManager)
        {
            _data = data;
            state = stateItem;
            previousState = previousStateItem;
            manager = shopManager;

            RefreshPurchaseState();
            scoreObserver.OnValueChanged += UpdateStateButton;
            incomeObserver.OnValueChanged += RefreshPurchaseState;
            SettingsService.LanguageChanged += RefreshPurchaseState;
        }

        protected override void UpdateStateButton()
        {
            _button.interactable = UpgradeEconomyService.CanBuyIncomeUpgrade(_data, state, scoreValue.Value);
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
            long? price = _data.GetPrice(state.Count);

            var localized = GameText.Upgrade(_data.Id, true);
            titleText.text = localized?.Title ?? _data.Title;
            priceText.text = isSecret ? GameText.Choose("Locked", "Закрыто")
                : price.HasValue ? GameText.Buy(price.Value.ScoreViewFromNumber()) : GameText.Choose("Max", "Макс.");
            incomeText.text = isSecret ? string.Empty
                : $"+{GameText.Number(UIntValue.ToCoins(_data.Income))}{GameText.Choose(" / sec", " / сек")}";
            descriptionText.text = isSecret
                ? GameText.Choose("Buy the previous source", "Купи предыдущий источник")
                : localized?.Description ?? _data.Description;
            if (ownedText != null)
                ownedText.text = isSecret ? string.Empty : GameText.Choose($"Owned {state.Count}", $"Куплено {state.Count}");
        }

        // private void OnDestroy()
        // {
        //     Debug.Log($"INCOME {name}: Открепил событие к scoreObserver");
        //     scoreObserver.OnValueChanged -= UpdateStateButton;
        // }

        private void OnDisable()
        {
            scoreObserver.OnValueChanged -= UpdateStateButton;
            incomeObserver.OnValueChanged -= RefreshPurchaseState;
            SettingsService.LanguageChanged -= RefreshPurchaseState;
        }

        public override void OnBuyEvent()
        {
            if (UpgradeEconomyService.TryBuyIncomeUpgrade(_data, state, scoreValue, incomeValue, manager))
                AudioManager.Instance?.PlaySfx(SfxType.UpgradeBuy);
        }
    }
}