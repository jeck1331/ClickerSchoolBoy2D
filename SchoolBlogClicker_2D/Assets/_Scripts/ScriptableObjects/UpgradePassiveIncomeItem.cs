using System.Collections.Generic;
using _Scripts.Utilities;
using UnityEngine;

namespace _Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Upgrade/PassiveIncome Item Upgrade")]
    public class UpgradePassiveIncomeItem : BaseUpgradeItem_SO
    {
        [Header("Passive income")][SerializeField] private float passiveIncomeValue = 1f;
        public float PassiveIncomeValue => passiveIncomeValue;
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/StaticData/PassiveIncome Items Upgrade")]
    public class StateUpgradePassiveIncomeItems : ScriptableObject
    {
        [Header("Улучшения пассвиного дохода")][SerializeField] private List<UpgradePassiveIncomeItem> items;
    }
}