using _Scripts.Models.Enums;
using _Scripts.Models.Upgrade;
using UnityEngine;

namespace _Scripts.Core
{
    public static class CalcCacheHelper
    {
        public static uint CalcIncomeCache (UpgradeIncomeField[] incomeUpgrades)
        {
            uint res = 0;
            foreach (var upgrade in incomeUpgrades)
            {
                res += upgrade.Income;
            }

            // Debug.Log($"{res} РАССЧИТАН INCOME CACHE");
            return res;
        }
        
        public static uint CalcPowerCache (UpgradeClickField[] powerUpgrades)
        {
            uint res = 1;
            foreach (var upgrade in powerUpgrades)
            {
                switch (upgrade.CalcType)
                {
                    case ClickCalcType.Add:
                        res += upgrade.Power;
                        break;
                    case ClickCalcType.Multiply:
                        res *= upgrade.Power;
                        break;
                }
            }
            // Debug.Log($"{res} РАССЧИТАН POWER CACHE");
            return res;
        }
    }
}