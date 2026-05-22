using System;
using UnityEngine;

namespace _Scripts.Models.Upgrade
{
    [Serializable]
    public class UpgradeIncomeField: UpgradeBaseField
    {
        [Header("Income Value")] [SerializeField] private int income;
        public int Income => income;
    }
}