using System;
using _Scripts.Models.Enums;
using UnityEngine;

namespace _Scripts.Models.Upgrade
{
    [Serializable]
    public class UpgradeClickField: UpgradeBaseField
    {
        [Header("Power")] [SerializeField] private uint power;
        [Header("Type Calculation")] [SerializeField] private ClickCalcType calcType;
        
        public ClickCalcType CalcType => calcType;
        public uint Power => power;
    }
}