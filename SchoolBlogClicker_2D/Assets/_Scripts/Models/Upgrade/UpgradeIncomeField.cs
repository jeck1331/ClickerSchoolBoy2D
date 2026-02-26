using System;
using UnityEngine;

namespace _Scripts.Models.Upgrade
{
    [Serializable]
    public class UpgradeIncomeField: UpgradeBaseField
    {
        [Header("Income Value")] [SerializeField] private uint income;
        public uint Income => income;
    }
}