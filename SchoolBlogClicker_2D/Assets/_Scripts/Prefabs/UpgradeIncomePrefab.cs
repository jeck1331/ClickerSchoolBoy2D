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

        public override void Initialize(UpgradeIncomeField data, UpgradeStateItem stateItem, UpgradeStateItem previousStateItem, ShopManager shopManager)
        {
            _data = data;
            state = stateItem;
            previousState = previousStateItem;
            manager = shopManager;

            titleText.text = data.Title;
            descriptionText.text = data.Description;
            incomeText.text = data.Income.ToString();

            CheckSecretState();
            UpdateStateButton();
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
            if (state.IsSecret)
                UpgradeEconomyService.TryUnlockByPreviousId(state, previousState);

            UpdateStateTexts();
        }

        private void UpdateStateTexts()
        {
            bool isSecret = state.IsSecret;
            bool isBought = state.IsBought;

            _image.material = isSecret ? matSecretField : null;
            priceText.text = isSecret ? "???" : isBought ? "Куплено" : _data.Price.ToString();
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
            scoreObserver.OnValueChanged -= UpdateStateButton;
            incomeObserver.OnValueChanged -= UpdateStateButton;
            incomeObserver.OnValueChanged -= CheckSecretState;
        }

        public override void OnBuyEvent()
        {
            if (UpgradeEconomyService.TryBuyIncomeUpgrade(_data, state, scoreValue, incomeValue, manager))
            {
                AudioManager.Instance?.PlaySfx(SfxType.UpgradeBuy);
                UpdateStateButton();
            }
        }
    }
}