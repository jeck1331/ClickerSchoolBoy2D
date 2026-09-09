using _Scripts.Models.Upgrade;
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

            titleText.text = data.Title;
            descriptionText.text = data.Description;
            powerText.text = data.Power.ToString();

            CheckSecretState();
            UpdateStateButton();
            scoreObserver.OnValueChanged += UpdateStateButton;
            powerObserver.OnValueChanged += UpdateStateButton;
            powerObserver.OnValueChanged += CheckSecretState;
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
            if (UpgradeEconomyService.TryBuyClickUpgrade(_data, state, scoreValue, powerValue, manager))
                AudioManager.Instance?.PlaySfx(SfxType.UpgradeBuy);
        }
    }
}