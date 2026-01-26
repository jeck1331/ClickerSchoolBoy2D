// using System;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class UpgradeFieldPrefab : MonoBehaviour
// {
//     [SerializeField] private TMP_Text titleText;
//     [SerializeField] private TMP_Text priceText;
//     [SerializeField] private TMP_Text descriptionText;
//     [SerializeField] private TMP_Text levelText;
//     [SerializeField] private UpgradeMenuUI upgradeMenuUIParent;
//
//     private event Action<ulong> _buyEvent;
//
//     private Button _button;
//
//     private UpgradeFieldModel _data;
//
//     public void Initialize(UpgradeFieldModel data)
//     {
//         _data = data;
//
//         titleText.text = data.Title;
//         priceText.text = data.PriceValue;
//         descriptionText.text = data.Description;
//         levelText.text = data.CurrentValue;
//     }
//
//     private void Awake()
//     {
//         _button = GetComponent<Button>();
//        // _buyEvent = upgradeMenuUIParent;
//     }
//
//     public void OnBuyEvent()
//     {
//         _buyEvent?.Invoke(ulong.Parse(_data.PriceValue));
//     }
// }
