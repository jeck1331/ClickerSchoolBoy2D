using System;
using UnityEngine;

namespace _Scripts.Models.Upgrade
{
    [Serializable]
    public class UpgradeIncomeField : UpgradeBaseField
    {
        private const double PriceMultiplier = 1.15;

        [Header("Income per second (hundredths of a coin)")]
        [SerializeField] private int income;

        public int Income => income;

        public long? GetPrice(int owned)
        {
            double price = Math.Round(Price * Math.Pow(PriceMultiplier, owned), MidpointRounding.AwayFromZero);
            return price < (double)long.MaxValue ? (long)price : null;
        }
    }
}
