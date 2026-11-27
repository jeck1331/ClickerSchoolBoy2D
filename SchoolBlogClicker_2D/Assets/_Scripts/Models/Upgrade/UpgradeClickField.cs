using System;
using _Scripts.Models.Enums;
using UnityEngine;

namespace _Scripts.Models.Upgrade
{
    [Serializable]
    public class UpgradeClickField: UpgradeBaseField
    {
        [Header("Click bonus (hundredths of a coin) or multiplier")] [SerializeField] private int power;
        [Header("Type Calculation")] [SerializeField] private ClickCalcType calcType;
        
        public ClickCalcType CalcType => calcType;
        public int Power => power;
    }
}