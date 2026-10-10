using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Models.Upgrade
{
    public abstract class UpgradeBaseItem<TData, TManager> : MonoBehaviour where TData : class
    {
        [SerializeField] protected ULongValue scoreValue;
        [SerializeField] protected ObserverSO scoreObserver;

        [SerializeField] protected TMP_Text titleText;
        [SerializeField] protected TMP_Text priceText;
        [SerializeField] protected TMP_Text descriptionText;
        [SerializeField] public Material matSecretField;
        
        [SerializeField] protected UpgradeStateItem state;

        protected Button _button;
        protected Image _image;

        protected TData _data;
        protected UpgradeStateItem previousState;
        protected TManager manager;

        public abstract void Initialize(TData data, UpgradeStateItem stateItem, UpgradeStateItem previousStateItem, TManager manager);

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        protected abstract void UpdateStateButton();

        public abstract void OnBuyEvent();
    }
}