using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Models.Upgrade
{
    public abstract class UpgradeBaseItem<TData> : MonoBehaviour where TData : class
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

        public abstract void Initialize(TData data, UpgradeStateItem stateItem);

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        protected abstract void UpdateStateButton();

        public abstract void OnBuyEvent();
    }
}